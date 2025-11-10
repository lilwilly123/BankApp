using System;
using System.Collections.Generic;
using System.Linq;

namespace BankApp
{
    internal class SystemOwner : Admin
    {
        public static List<Account> AllAccounts = new List<Account>();       // ✅ RESTORED
        public static List<Transaction> TransactionList = new List<Transaction>();
        public static List<Transaction> PendingTransactions = new List<Transaction>();

        public string OwnerID { get; set; }
        public string Name { get; set; }
        public int MaxLoanMultiplier { get; set; } = 5;
        public int TransferDelayMinutes { get; set; } = 15;

        // Show all accounts
        public void ViewAllAccounts()
        {
            if (AllAccounts.Count == 0)
            {
                Console.WriteLine("No accounts in the system.");
                return;
            }

            Console.WriteLine("=== All Accounts ===");
            foreach (var account in AllAccounts)
            {
                Console.WriteLine($"{account.AccountNumber} | {account.Currency} | Balance: {account.Balance} | Status: {account.Status}");
            }
        }

        // Show all transactions
        public void ViewAllTranscations()
        {
            if (TransactionList.Count == 0)
            {
                Console.WriteLine("No completed transactions yet.");
                return;
            }

            foreach (var transaction in TransactionList)
                transaction.PrintTransaction();
        }

        // Change loan policy
        public void SetLoanPolicy(int maxLoanMultiplier)
        {
            MaxLoanMultiplier = maxLoanMultiplier;
        }

        // Change transfer delay
        public void SetTransferDelayPolicy(int transferDelayMinutes)
        {
            TransferDelayMinutes = transferDelayMinutes;
        }

        // Process pending transactions
        public void ProcessPendingTransactions(List<Customer> customers)
        {
            var transactionsToFinish = PendingTransactions.ToList(); // copy to avoid modification during loop

            foreach (var tx in transactionsToFinish)
            {
                var senderAcc = AllAccounts.FirstOrDefault(a => a.AccountNumber == tx.Sender);
                var targetAcc = AllAccounts.FirstOrDefault(a => a.AccountNumber == tx.Target);

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

            Console.WriteLine("All pending transactions have been processed.");
        }
    }
}
