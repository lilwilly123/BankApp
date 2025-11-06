using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankApp
{
    internal class Customer : Admin
    {
        // Global lists shared across the system: 
        // TransactionList stores all transactions made in the bank,
        // CurrencyRates holds all defined currency exchange rates.
        public static List<CurrencyRate> CurrencyRates = new List<CurrencyRate>();
        public string Name { get; set; } // Customer's full name    
        public string CustomerID { get; set; } // Unique identifier for the customer

        public string Email { get; set; } // Contact email

        public List<Account> Accounts { get; set; } = new List<Account>(); // List of customer's bank accounts

        public List<Loan> Loans { get; set; } = new List<Loan>(); // List of customer's loans





        public void CreateAccount()
        {
            Console.Write("Choose currency (SEK/EUR/USD): ");
            string curr = Console.ReadLine()?.Trim().ToUpper();

            // List of allowed currencies
            string[] validCurrencies = { "SEK", "EUR", "USD" };

            // Check if the input is valid
            if (!validCurrencies.Contains(curr))
            {
                Console.WriteLine("Invalid currency. Defaulting to SEK.");
                curr = "SEK";
            }

            var newAccount = new Account(currency: curr, initialBalance: 0m);
            Accounts.Add(newAccount);

            Console.WriteLine($"Account created!");
            Console.WriteLine($"Bankgiro: {newAccount.AccountNumber}");
            Console.WriteLine($"Currency: {newAccount.Currency}");
            Console.WriteLine($"Balance:  {newAccount.Balance}");
        }

        //---------------
        //Withdraw Funds
        //---------------
        public void WithdrawFunds()
        {
            if (Accounts.Count == 0)
            {
                Console.WriteLine("You have no accounts.");
                return;
            }

            Console.WriteLine("\nChoose an account to withdraw from:");
            for (int i = 0; i < Accounts.Count; i++)
                Console.WriteLine($"{i + 1}. {Accounts[i].AccountNumber} | {Accounts[i].Currency} | Balance: {Accounts[i].Balance}");

            Console.Write("\nChoose (1 - {0}): ", Accounts.Count);

            if (!int.TryParse(Console.ReadLine(), out int choice) || choice < 1 || choice > Accounts.Count)
            {
                Console.WriteLine("Invalid selection.");
                return;
            }

            var sourceAccount = Accounts[choice - 1];

            Console.Write("Amount: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal amount) || amount <= 0)
            {
                Console.WriteLine("Invalid amount.");
                return;
            }

            SystemOwner.PendingTransactions.Add(new Transaction(
                sourceAccount.AccountNumber,
                "Cash",
                amount,
                Transaction.TransactionType.Withdrawal
            ));

            Console.WriteLine($"Withdrawal scheduled (waiting for Admin to process).");
        }

        //-----------------
        //Deposit funds
        //-----------------
        public void DepositFunds()
        {
            if (Accounts.Count == 0)
            {
                Console.WriteLine("You have no accounts.");
                return;
            }

            Console.WriteLine("\nChoose an account to deposit into:");
            for (int i = 0; i < Accounts.Count; i++)
                Console.WriteLine($"{i + 1}. {Accounts[i].AccountNumber} | {Accounts[i].Currency} | Balance: {Accounts[i].Balance}");

            Console.Write("\nChoose (1 - {0}): ", Accounts.Count);
            if (!int.TryParse(Console.ReadLine(), out int choice) || choice < 1 || choice > Accounts.Count)
            {
                Console.WriteLine("Invalid selection.");
                return;
            }

            var selectedAccount = Accounts[choice - 1];

            Console.Write("Amount: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal amount) || amount <= 0)
            {
                Console.WriteLine("Invalid amount.");
                return;
            }

            // Add to pending (admin will process later)
            SystemOwner.PendingTransactions.Add(new Transaction(
                "Bank",
                selectedAccount.AccountNumber,
                amount,
                Transaction.TransactionType.Deposit
            ));

            Console.WriteLine($"Deposit scheduled and placed in Pending Transactions.");
        }


        //---------------------
        //List accounts
        //---------------------
        public void ListAccounts()
        {
            Console.WriteLine("=== Your Accounts ===\n");

            if (Accounts.Count == 0)
            {
                Console.WriteLine("You currently have no accounts.");
                return;
            }

            foreach (var account in Accounts)
                Console.WriteLine($"[{account.AccountNumber}] {account.Currency} | Balance: {account.Balance} | Status: {account.Status}");
        }

        //----------------
        //Transaction own
        //----------------
        public void TransferBetweenOwnAccounts()
        {
            if (Accounts.Count < 2)
            {
                Console.WriteLine("You need at least two accounts to transfer between them.");
                return;
            }

            Console.WriteLine("\nYour accounts:");
            for (int i = 0; i < Accounts.Count; i++)
                Console.WriteLine($"{i + 1}. {Accounts[i].AccountNumber} | {Accounts[i].Currency} | Balance: {Accounts[i].Balance}");

            Console.Write("\nSelect SOURCE account (1 - {0}): ", Accounts.Count);
            if (!int.TryParse(Console.ReadLine(), out int srcIndex) || srcIndex < 1 || srcIndex > Accounts.Count)
            {
                Console.WriteLine("Invalid selection.");
                return;
            }

            Console.Write("Select TARGET account (1 - {0}): ", Accounts.Count);
            if (!int.TryParse(Console.ReadLine(), out int tgtIndex) || tgtIndex < 1 || tgtIndex > Accounts.Count)
            {
                Console.WriteLine("Invalid selection.");
                return;
            }

            if (srcIndex == tgtIndex)
            {
                Console.WriteLine("You cannot transfer to the same account.");
                return;
            }

            var source = Accounts[srcIndex - 1];
            var target = Accounts[tgtIndex - 1];

            Console.Write("Amount: ");
            decimal amount;
            if (!decimal.TryParse(Console.ReadLine(), out amount) || amount <= 0)
            {
                Console.WriteLine("Invalid amount.");
                return;
            }

            SystemOwner.PendingTransactions.Add(new Transaction(
                source.AccountNumber,
                target.AccountNumber,
                amount,
                Transaction.TransactionType.Transfer
            ));

            Console.WriteLine($"Transfer scheduled (waiting for Admin to process).");
        }

        //--------------------
        //Transaction Other
        //--------------------
        public void TransferToOtherCustomer(Customer targetCustomer)
        {
            if (Accounts.Count == 0)
            {
                Console.WriteLine("You have no accounts.");
                return;
            }

            Console.WriteLine("\nYour accounts:");
            for (int i = 0; i < Accounts.Count; i++)
                Console.WriteLine($"{i + 1}. {Accounts[i].AccountNumber} | {Accounts[i].Currency} | Balance: {Accounts[i].Balance}");

            Console.Write("\nSelect SOURCE account (1 - {0}): ", Accounts.Count);
            if (!int.TryParse(Console.ReadLine(), out int srcIndex) || srcIndex < 1 || srcIndex > Accounts.Count)
            {
                Console.WriteLine("Invalid selection.");
                return;
            }

            var source = Accounts[srcIndex - 1];

            Console.Write("Recipient account number: ");
            string targetAccountNumber = Console.ReadLine()?.Trim();

            var target = targetCustomer?.Accounts?.FirstOrDefault(a => a.AccountNumber == targetAccountNumber);
            if (target == null)
            {
                Console.WriteLine("Target account not found.");
                return;
            }

            Console.Write("Amount: ");
            decimal amount;
            if (!decimal.TryParse(Console.ReadLine(), out amount) || amount <= 0)
            {
                Console.WriteLine("Invalid amount.");
                return;
            }

            SystemOwner.PendingTransactions.Add(new Transaction(
                source.AccountNumber,
                target.AccountNumber,
                amount,
                Transaction.TransactionType.Transfer
            ));

            Console.WriteLine("Transfer scheduled (waiting for Admin to process).");
        }
        //-----------------
        //Apply loan
        //-----------------
        public void ApplyForLoan(decimal loanAmount, SystemOwner owner)
        {
            
           decimal totalBalance = 0; 

            foreach (var account in Accounts)
            {
                totalBalance += account.Balance;
            }

            if (loanAmount <= totalBalance * owner.MaxLoanMultiplier)
            {
                Console.WriteLine("Which account to deposit the loan into?");
                string accountNumber = Console.ReadLine();
                var loanAccount = GetAccountByNumber(accountNumber);
                if (loanAccount != null)
                {
                    Loan loan = new Loan(loanAmount, 5f, DateTime.Now, DateTime.Now.AddYears(1)); // Create loan object from Loan class constructor
                    Loans.Add(loan); // Loans list object was created globally in this class
                    loanAccount.Balance += loanAmount; // Add the loan amount to the account in question 
                }
                else
                {
                    Console.WriteLine($"Account {accountNumber} not found. Please try again.");
                }
            }
            else
            {
                Console.WriteLine("Loan amount is too high.");
            }

        }
        //----------------------
        //Trasaction History
        //----------------------
        public void TransactionHistory()
        {
            Console.WriteLine("=== Your Transaction History ===\n");

            // Get list of customer's account numbers
            var myAccounts = Accounts.Select(a => a.AccountNumber).ToList();

            // Filter global transaction list to only show relevant ones
            var myTransactions = SystemOwner.TransactionList
                .Where(t => myAccounts.Contains(t.Sender) || myAccounts.Contains(t.Target))
                .ToList();

            if (myTransactions.Count == 0)
            {
                Console.WriteLine("No transactions yet.");
                return;
            }

            foreach (var t in myTransactions)
                t.PrintTransaction();
        }

        // Loops through all available currency rates and prints each conversion pair 
        // (e.g., USD → EUR) along with its current exchange rate.
        public void ViewCurrencyExchangeRates()
        {
            foreach (var rate in CurrencyRate.rates)
            {
                Console.WriteLine($"{rate.Key.From} {rate.Key.To} {rate.Value}");
            }
        }

        public Account GetAccountByNumber(string accountNumber) // Helper method for finding accounts
        {
            foreach (var account in Accounts)
            {
                if (account.AccountNumber == accountNumber) // Find the matching account
                {
                    return account; // Account object was found 
                }             

            }
            return null; // Return null if no account object was found
        }

        //----------------
        //Transaction Log
        //----------------
        public void LogTransaction(string sender, string target, decimal amount, Transaction.TransactionType type)
        {
            var t = new Transaction(sender, target, amount, type);
            SystemOwner.PendingTransactions.Add(t);

            Console.WriteLine("Transaction scheduled and will complete after the transfer delay.");
        }
    }
}
