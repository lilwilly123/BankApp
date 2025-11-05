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
            Account newAccount = new Account();
            Accounts.Add(newAccount); // Add the new account to the customer's account list
        }

        public void WithdrawFunds(string accountNumber, decimal amount)
        {
            foreach (var account in Accounts)
            {
                if (account.AccountNumber == accountNumber) // Find the matching account
                {
                    if (account.Balance >= amount)
                    {
                        account.Balance -= amount;
                        Console.WriteLine($"Withdrawal of {amount} successful. New balance: {account.Balance}");
                    }
                    else
                    {
                        Console.WriteLine("Insufficient funds for this withdrawal.");
                    }
                    return; // Exit after processing the account
                }
            }

            Console.WriteLine("Account not found."); // If no matching account is found
        }

        public void DepositFunds(string accountNumber, decimal amount)
        {
            foreach (var account in Accounts)
            {
                if (account.AccountNumber == accountNumber)
                {
                        account.Balance += amount;
                        Console.WriteLine($"Deposit of {amount} successful. New balance: {account.Balance}");
                   
                        return; 
                }
            }

            Console.WriteLine("Account not found."); 
        }

        public void ListAccounts()
        {
            foreach (var account in Accounts)
            {
                Console.WriteLine($"Account Number: {account.AccountNumber}, Balance: {account.Balance}, Status: {account.Status}");
            }
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
            
            // TODO: If allowed, create a new Loan object (use Loan constructor) and add it to Loans list
            // TODO: Otherwise, display a message that the loan request exceeds allowed limit

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
                    loanAccount.Balance += loanAmount;
                }
                else
                {
                    Console.WriteLine($"Account {accountNumber} not found. Please try again.");
                }

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
