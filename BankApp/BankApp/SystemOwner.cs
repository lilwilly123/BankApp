using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankApp
{
    internal class SystemOwner : Admin
    {
        public static List<Transaction> TransactionList = new List<Transaction>(); // Static so all transactions are shared globally
        public string OwnerID { get; set; } // Unique identifier for the system owner
        public string Name { get; set; } // System owner's full name
        public int MaxLoanMultiplier { get; set; } = 5; // e.g., 5 means max loan = 5 × total deposits

        public int TransferDelayMinutes { get; set; } = 15; // 15 minute wait/delay time before transfer goes through 


        // List<Account> is the collection of all accounts to loop through
        public void ViewAllAccounts(List<Account> accounts)
        {
            foreach (var account in accounts)
            {
                Console.WriteLine($"{account.AccountNumber} {account.OwnerID} {account.AccountName}");
            }
       
        }

        public void ViewAllTranscations()
        {
            foreach (var transaction in TransactionList) // uses the shared list.
            {
                transaction.PrintTransaction(); // Leverages the existing PrintTransaction() method from Transaction class
            }

        }

        // e.g., “max loan = 5× total deposits”
        public void SetLoanPolicy(int maxLoanMultiplier)
        {
            MaxLoanMultiplier = maxLoanMultiplier;
        }

        // Set or change that 15-minute wait
        public void SetTransferDelayPolicy(int transferDelayMinutes) // Parameter lets the owner adjust both policy dynamically
        {
            TransferDelayMinutes = transferDelayMinutes;
        }

    }
}
