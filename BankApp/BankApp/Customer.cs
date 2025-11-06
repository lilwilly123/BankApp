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

        public static List<Transaction> TransactionList = new List<Transaction>();

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


        public void WithdrawFunds(string accountNumber, decimal amount)
        {
            var acc = GetAccountByNumber(accountNumber);
            if (acc == null)
            {
                Console.WriteLine("Account not found.");
                return;
            }

            acc.Withdraw(amount);
        }


        public void DepositFunds(string accountNumber, decimal amount)
        {
            var acc = GetAccountByNumber(accountNumber);
            if (acc == null)
            {
                Console.WriteLine("Account not found.");
                return;
            }

            acc.Deposit(amount); // All validation happens in the account class
        }


        public void ListAccounts()
        {
            if (Accounts.Count == 0)
            {
                Console.WriteLine("No accounts yet.");
                return;
            }

            Console.WriteLine("Your accounts:");
            foreach (var account in Accounts)
                Console.WriteLine($"Bankgiro: {account.AccountNumber} | {account.Currency} | Balance: {account.Balance} | Status: {account.Status}");
        }

        public void TransferBetweenOwnAccounts(decimal amount)
        {
            Console.Write("Please enter source account number: "); 
            string sourceAccount = Console.ReadLine();

            Console.Write("Please enter target account number: ");
            string targetAccount = Console.ReadLine(); 

            foreach (var source in Accounts)
            {
                if (source.AccountNumber == sourceAccount) 
                {
                    if (source.Balance >= amount)
                    {
                        foreach (var target in Accounts)
                        {
                            if (target.AccountNumber == targetAccount)
                            {
                                target.Balance += amount;
                                source.Balance -= amount; 
                                Console.WriteLine($"Withdrawal of {amount} successful. New balance: {target.Balance}");
                            }
                        }                        
                    }
                    else
                    {
                        Console.WriteLine("Insufficient funds for this withdrawal.");
                    }
                    return; 
                }
            }

            Console.WriteLine("Accounts not found.");

        }

        public void TransferToOtherCustomer(Customer targetCustomer, decimal amount) 
        {
            Console.Write("Please enter your account number to send from: ");
            string sourceAccount = Console.ReadLine();

            Console.Write("Please enter the customer account number to send to: ");
            string targetAccount = Console.ReadLine();

            foreach (var source in Accounts)
            {
                if (source.AccountNumber == sourceAccount)
                {
                    if (source.Balance >= amount)
                    {
                        foreach (var target in targetCustomer.Accounts) // Goes through the accounts for the target customer and not the customer's own accounts
                        {
                            if (target.AccountNumber == targetAccount)
                            {
                                target.Balance += amount;
                                source.Balance -= amount;
                                Console.WriteLine($"Withdrawal of {amount} successful. New balance: {target.Balance}");
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Insufficient funds for this withdrawal.");
                    }
                    return;

                }
            }

            Console.WriteLine("Accounts not found.");
        }
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

        public void TransactionHistory()
        {
            foreach (var transaction in TransactionList) // uses the shared list.
            {
                transaction.PrintTransaction(); // Leverages the existing PrintTransaction() method from Transaction class
            }
        }

        // Loops through all available currency rates and prints each conversion pair 
        // (e.g., USD → EUR) along with its current exchange rate.
        public void ViewCurrencyExchangeRates()
        {
            foreach (var rate in CurrencyRates)
            {
                Console.WriteLine($"{rate.ExchangeRate} {rate.FromCurrency} {rate.ToCurrency}");
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
    }


}
