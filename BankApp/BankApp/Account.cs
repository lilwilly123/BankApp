using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BankApp.Transaction;

namespace BankApp
{
    internal class Account
    {
        public enum AccountStatus
        {
            Active,
            Frozen,
            Closed
        }

        //Has a list of all the current accountnumbers so there cant be two of the same
        private static HashSet<string> _usedNumbers = new HashSet<string>();
        private static Random _rng = new Random();

        public string AccountNumber { get; set; }
        public string OwnerID { get; set; }
        public string AccountName { get; set; }
        public decimal Balance { get; set; }

        public string Currency { get; set; }
        public AccountStatus Status { get; set; }


        public Account(string currency = "SEK", decimal initialBalance = 0m)
        {
            AccountNumber = GenerateUniqueAccountNumber();
            Currency = currency.ToUpper();
            Balance = initialBalance;
            Status = AccountStatus.Active;
        }


        // ----------------------------------------------------------
        // Deposit
        // ----------------------------------------------------------
        public bool Deposit(decimal amount)
        {
            if (Status != AccountStatus.Active)
            {
                Console.WriteLine("Cannot deposit — account is not active.");
                return false;
            }

            if (amount <= 0)
            {
                Console.WriteLine("Deposit amount must be greater than zero.");
                return false;
            }

            Balance += amount;
            return true;
        }

        // ----------------------------------------------------------
        // Withdraw
        // ----------------------------------------------------------
        public bool Withdraw(decimal amount)
        {
            if (Status != AccountStatus.Active)
            {
                Console.WriteLine("Cannot withdraw — account is not active.");
                return false;
            }

            if (amount <= 0)
            {
                Console.WriteLine("Withdrawal amount must be greater than zero.");
                return false;
            }

            if (amount > Balance)
            {
                Console.WriteLine("Insufficient funds.");
                return false;
            }

            Balance -= amount;
            return true;
        }

        // Method: change the account's status (Active, Frozen, Closed)
        public void ChangeStatus(AccountStatus newStatus)
        {
            {
                Status = newStatus;
            }
        }

        // ----------------------------------------------------------
        // Generate account number
        // ----------------------------------------------------------
        private static string GenerateUniqueAccountNumber()
        {
            // Format: 7–3 (t.ex. 1234567-123)
            while (true)
            {
                string n1 = _rng.Next(1000000, 9999999).ToString();
                string n2 = _rng.Next(1000, 9999).ToString();
                string candidate = $"{n1}-{n2}";
                if (_usedNumbers.Add(candidate))
                    return candidate;
            }
        }

    }
}