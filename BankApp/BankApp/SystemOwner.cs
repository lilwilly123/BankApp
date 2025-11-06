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
        public static List<Transaction> PendingTransactions = new List<Transaction>();

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

        //----------------------------
        //Process pending transactions
        //----------------------------
        public void ProcessPendingTransactions(List<Customer> customers)
        {
            var transactionsToFinish = PendingTransactions.ToList(); //Process ALL pending now

            foreach (var tx in transactionsToFinish)
            {
                var senderAcc = customers.SelectMany(c => c.Accounts)
                                         .FirstOrDefault(a => a.AccountNumber == tx.Sender);
                var targetAcc = customers.SelectMany(c => c.Accounts)
                                         .FirstOrDefault(a => a.AccountNumber == tx.Target);

                bool applied = false;

                switch (tx.TypeOfTransaction)
                {
                    case Transaction.TransactionType.Deposit:
                        applied = (targetAcc != null) && targetAcc.Deposit(tx.Amount);
                        break;
                    case Transaction.TransactionType.Withdrawal:
                        applied = (senderAcc != null) && senderAcc.Withdraw(tx.Amount);
                        break;
                    case Transaction.TransactionType.Transfer:
                        bool w = (senderAcc != null) && senderAcc.Withdraw(tx.Amount);
                        bool d = (targetAcc != null) && targetAcc.Deposit(tx.Amount);
                        applied = w && d;
                        break;
                }

                tx.TransactionStatus = applied ? Transaction.Status.Completed : Transaction.Status.Failed;

                TransactionList.Add(tx);
                PendingTransactions.Remove(tx);
            }

            Console.WriteLine("All pending transactions have been processed by Admin.");
        }

    }
}
