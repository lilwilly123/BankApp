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
        public List<Account> Accounts { get; set; } = new List<Account>(); // List of customer's bank accounts

        public List<Loan> Loans { get; set; } = new List<Loan>(); // List of customer's loans


        //------------------------
        //Create (Regular) Account
        //-------------------------
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
            SystemOwner.AllAccounts.Add(newAccount);

            UiStyle.Success("Account created successfully.");
            Console.WriteLine($"Bankgiro: {newAccount.AccountNumber}");
            Console.WriteLine($"Currency: {newAccount.Currency}");
            Console.WriteLine($"Balance:  {newAccount.Balance}");
        }

        //----------------------
        //Create Savings Account
        //-----------------------

        public void CreateSavingsAccount()
        {
            
            SavingsAccount newAccount = new SavingsAccount();
            newAccount.ApplyInterest(); // This applies interest from SavingsAccount class
            Accounts.Add(newAccount);
            SystemOwner.AllAccounts.Add(newAccount);

            UiStyle.Success("Account created!");
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
                UiStyle.Error("Invalid selection.");
                return;
            }

            var sourceAccount = Accounts[choice - 1];

            UiStyle.Prompt("Amount: ");
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
                UiStyle.Error("Invalid selection.");
                return;
            }

            var selectedAccount = Accounts[choice - 1];

            UiStyle.Prompt("Amount: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal amount) || amount <= 0)
            {
                UiStyle.Error("Invalid amount.");
                return;
            }

            // Add to pending (admin will process later)
            SystemOwner.PendingTransactions.Add(new Transaction(
                "?",
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
                UiStyle.Error("You currently have no accounts.");
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
            // User must have at least two accounts to transfer between them
            if (Accounts.Count < 2)
            {
                UiStyle.Error("You need at least two accounts to perform a transfer.");
                return;
            }

            Console.WriteLine("\nYour Accounts:");
            for (int i = 0; i < Accounts.Count; i++)
                Console.WriteLine($"{i + 1}. {Accounts[i].AccountNumber} | {Accounts[i].Currency} | Balance: {Accounts[i].Balance}");

            Console.Write("\nSelect SOURCE account (1 - {0}): ", Accounts.Count);
            if (!int.TryParse(Console.ReadLine(), out int srcIndex) || srcIndex < 1 || srcIndex > Accounts.Count)
            {
                UiStyle.Error("Invalid selection.");
                return;
            }

            Console.Write("Select TARGET account (1 - {0}): ", Accounts.Count);
            if (!int.TryParse(Console.ReadLine(), out int tgtIndex) || tgtIndex < 1 || tgtIndex > Accounts.Count)
            {
                UiStyle.Error("Invalid selection.");
                return;
            }

            if (srcIndex == tgtIndex)
            {
                Console.WriteLine("Source and target accounts cannot be the same.");
                return;
            }

            var source = Accounts[srcIndex - 1];
            var target = Accounts[tgtIndex - 1];

            UiStyle.Prompt("Amount: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal amount) || amount <= 0)
            {
                Console.WriteLine("Invalid amount entered.");
                return;
            }

            // Check available balance including pending withdrawals
            decimal available = GetAvailableBalance(source);
            if (amount > available)
            {
                Console.WriteLine($"Insufficient funds. Available: {available} {source.Currency}");
                return;
            }

            // Convert and round
            decimal withdrawAmount = RoundCurrency(amount, source.Currency);
            decimal convertedAmount = RoundCurrency(ConvertCurrency(amount, source.Currency, target.Currency), target.Currency);

            // Withdrawal transaction: shows "money left SOURCE to go to TARGET"
            SystemOwner.PendingTransactions.Add(new Transaction(
                source.AccountNumber,
                target.AccountNumber,
                withdrawAmount,
                Transaction.TransactionType.Withdrawal
            ));

            // Deposit transaction: shows "money came from SOURCE to TARGET"
            SystemOwner.PendingTransactions.Add(new Transaction(
                source.AccountNumber,
                target.AccountNumber,
                convertedAmount,
                Transaction.TransactionType.Deposit
            ));

            Console.WriteLine("Transfer scheduled (awaiting Admin approval).");
            Console.WriteLine($"Will withdraw {withdrawAmount} {source.Currency} and deposit {convertedAmount} {target.Currency}.");
        }


        //--------------------
        //Transaction Other
        //--------------------
        public void TransferToOtherCustomer(Customer targetCustomer)
        {
            if (Accounts.Count == 0)
            {
                Console.WriteLine("You have no accounts to transfer from.");
                return;
            }

            // Display sender's accounts (balance is okay for sender)
            Console.WriteLine("\nYour Accounts:");
            for (int i = 0; i < Accounts.Count; i++)
                Console.WriteLine($"{i + 1}. {Accounts[i].AccountNumber} | {Accounts[i].Currency} | Balance: {Accounts[i].Balance}");

            Console.Write("\nSelect SOURCE account (1 - {0}): ", Accounts.Count);
            if (!int.TryParse(Console.ReadLine(), out int srcIndex) || srcIndex < 1 || srcIndex > Accounts.Count)
            {
                UiStyle.Error("Invalid selection.");
                return;
            }

            var source = Accounts[srcIndex - 1];

            // show recipient accounts without sensitive info
            if (targetCustomer.Accounts.Count == 0) 
            {
                Console.WriteLine("Recipient has no accounts to receive transfers.");
                return;
            }

            Console.WriteLine("\nRecipient Accounts:");
            for (int i = 0; i < targetCustomer.Accounts.Count; i++)
                Console.WriteLine($"{i + 1}. {targetCustomer.Accounts[i].AccountNumber} | {targetCustomer.Accounts[i].Currency}");

            Console.Write("\nSelect RECIPIENT account (1 - {0}): ", targetCustomer.Accounts.Count);
            if (!int.TryParse(Console.ReadLine(), out int tgtIndex) || tgtIndex < 1 || tgtIndex > targetCustomer.Accounts.Count)
            {
                UiStyle.Error("Invalid selection.");
                return;
            }

            var target = targetCustomer.Accounts[tgtIndex - 1];

            UiStyle.Prompt("Amount: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal amount) || amount <= 0)
            {
                Console.WriteLine("Invalid amount.");
                return;
            }

            // Balance check with pending transactions
            decimal available = GetAvailableBalance(source);
            if (amount > available)
            {
                Console.WriteLine($"Insufficient funds. Available: {available} {source.Currency}");
                return;
            }

            // Convert & round
            decimal withdrawAmount = RoundCurrency(amount, source.Currency);
            decimal convertedAmount = RoundCurrency(ConvertCurrency(amount, source.Currency, target.Currency), target.Currency);

            // Withdrawal transaction: shows "money left SOURCE to go to TARGET"
            SystemOwner.PendingTransactions.Add(new Transaction(
                source.AccountNumber,
                target.AccountNumber,
                withdrawAmount,
                Transaction.TransactionType.Withdrawal
            ));

            // Deposit transaction: shows "money came from SOURCE to TARGET"
            SystemOwner.PendingTransactions.Add(new Transaction(
                source.AccountNumber,
                target.AccountNumber,
                convertedAmount,
                Transaction.TransactionType.Deposit
            ));



            Console.WriteLine("Transfer scheduled (pending Admin approval).");
            Console.WriteLine($"Will withdraw {withdrawAmount} {source.Currency} and deposit {convertedAmount} {target.Currency}.");
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
            UiStyle.Header("Your Transaction History");

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
        //-----------------------
        //Apply for loan
        //-----------------------
        public void ApplyLoan(SystemOwner owner)
        {
            // Must have at least one account to take a loan
            if (Accounts == null || Accounts.Count == 0)
            {
                Console.WriteLine("You need an account before you can take a loan.");
                return;
            }

            Console.WriteLine("=== Apply for Loan ===");

            // Pick the payout account
            Console.WriteLine("\nSelect the account to receive the loan (Enter to quit):");
            for (int i = 0; i < Accounts.Count; i++)
                Console.WriteLine($"{i + 1}. {Accounts[i].AccountNumber} | {Accounts[i].Currency} | Balance: {Accounts[i].Balance}");

            Console.Write($"\nChoose (1 - {Accounts.Count}): ");
            if (!int.TryParse(Console.ReadLine(), out int accChoice) || accChoice < 1 || accChoice > Accounts.Count)
            {
                UiStyle.Error("Invalid selection.");
                return;
            }

            var payoutAccount = Accounts[accChoice - 1];

            // Max amount is 5x total on all accounts for the user
            var totalBalance = Accounts.Sum(a => a.Balance);
            var maxLoan = totalBalance * owner.MaxLoanMultiplier;

            //Chenks if the user has more than 0 on the account
            if (maxLoan <= 0)
            {
                Console.WriteLine("You don't have enough money to make a loan");
                return;
            }

            Console.WriteLine($"Max loan for this account is: {maxLoan}");
            decimal loanAmount;
            while (true)
            {
                Console.Write("Amount to loan: ");
                if (decimal.TryParse(Console.ReadLine(), out loanAmount) && loanAmount > 0)
                {
                    if (loanAmount <= maxLoan) break;
                    Console.WriteLine($"Amount exceeds the allowed maximum ({maxLoan}). Try a lower amount.");
                }
                else
                {
                    Console.WriteLine("Invalid amount.");
                    return;
                }
            }
            // Choose (1/2/3 years)
            int termYears = 0;
            while (true)
            {
                Console.Write("Choose term (1, 2, or 3 years): ");
                var termInput = Console.ReadLine();
                if (termInput is "1" or "2" or "3")
                {
                    termYears = int.Parse(termInput);
                    break;
                }
                Console.WriteLine("Invalid input, choose 1, 2 or 3.");
            }

            // Create loan object rate + due date are automatic
            DateTime startDate = DateTime.Now;
            var loan = new Loan(loanAmount, termYears, startDate);
            Loans.Add(loan);

            // Payout: schedule a deposit into the chosen account 
            SystemOwner.PendingTransactions.Add(new Transaction(
                "Bank",                              
                payoutAccount.AccountNumber,         
                loanAmount,
                Transaction.TransactionType.Deposit 
            ));

            Console.WriteLine();
            Console.WriteLine("Loan approved and payout scheduled (pending Admin processing).");
            Console.WriteLine($"Principal: {loan.PrincipalAmount}");
            Console.WriteLine($"Rate: {loan.InterestRatePercent:0.##}%");
            Console.WriteLine($"Term: {loan.TermYears} years");
            Console.WriteLine($"Start: {loan.StartDate:d}");
            Console.WriteLine($"Due: {loan.DueDate:d}");
            Console.WriteLine($"Outstanding amount: {loan.OutstandingAmount}");
        }

        //------------------------
        //Convert currency
        //------------------------
        private decimal ConvertCurrency(decimal amount, string fromCurrency, string toCurrency)
        {
            // If same currency, no conversion needed
            if (fromCurrency == toCurrency)
                return amount;

            // Try to convert string -> CurrencyType enum
            if (!Enum.TryParse(fromCurrency, out CurrencyType fromType) ||
                !Enum.TryParse(toCurrency, out CurrencyType toType))
            {
                throw new Exception("Currency type not recognized.");
            }

            // Look up conversion rate
            if (!CurrencyRate.rates.TryGetValue((fromType, toType), out var rate))
            {
                throw new Exception($"No conversion rate defined for {fromCurrency} -> {toCurrency}");
            }

            // Convert amount
            return amount * rate;
        }

        //--------------------------
        // Currency rounding helpers
        //--------------------------
        private static int CurrencyDecimals(string currency) => currency.ToUpper() switch
        {
            "JPY" => 0,
            _ => 2, // Default to 2 decimals for most currencies we support
        };

        // Uses "AwayFromZero" so values don't get rounded weirdly when converting.
        private static decimal RoundCurrency(decimal amount, string currency)
        {
            int decimals = CurrencyDecimals(currency);
            return Math.Round(amount, decimals, MidpointRounding.AwayFromZero);
        }

        // Returns how much money the user can *actually* spend right now.
        // This includes pending transfers that haven't been processed yet,
        private decimal GetAvailableBalance(Account account)
        {
            // Total amount currently waiting to leave this account
            decimal pendingOut = SystemOwner.PendingTransactions
                .Where(t => t.Sender == account.AccountNumber)
                .Sum(t => t.Amount);

            // Total amount currently waiting to come in
            decimal pendingIn = SystemOwner.PendingTransactions
                .Where(t => t.Target == account.AccountNumber)
                .Sum(t => t.Amount);

            // Real available money = Current balance - outgoing pending + incoming pending
            return account.Balance - pendingOut + pendingIn;
        }
    }
}
