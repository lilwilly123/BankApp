using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public string AccountNumber { get; set; }
        public string OwnerID { get; set; }
        public string AccountName { get; set; }
        public decimal Balance { get; set; }

        public string Currency { get; set; }    
        public AccountStatus Status { get; set; }

            // Method: Deposit money 
            public void Deposit(decimal amount)
            {
                if (Status != AccountStatus.Active)
                    throw new InvalidOperationException("Cannot deposit to a non-active account.");

                if (amount <= 0)
                    throw new ArgumentException("Deposit amount must be positive.");

                Balance += amount;
            }
            // Method: withdraw money
            public void Withdraw(decimal amount)
            {
                if (Status != AccountStatus.Active)
                    throw new InvalidOperationException("Cannot withdraw from a non-active account.");

                if (amount <= 0)
                    throw new ArgumentException("Withdrawal amount must be positive.");

                if (amount > Balance)
                    throw new InvalidOperationException("Insufficient funds.");

                Balance -= amount;
            }
        // Method: change the account's status (Active, Frozen, Closed)
        public void ChangeStatus(AccountStatus newStatus)
            {
                Status = newStatus;
            }
        }
    }
