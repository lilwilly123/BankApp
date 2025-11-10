using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankApp
{
    internal class Transaction
    {
        public enum TransactionType
        {
            Deposit,
            Withdrawal,
            Transfer
        }

        public enum Status
        {
            Pending,
            Completed,
            Failed
        }
        //properties
        public string Sender { get; set; }
        public string Target { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; private set; }


        public TransactionType TypeOfTransaction { get; set; }
        public Status TransactionStatus { get; set; }



        // Constructor with default transaction status
        public Transaction(string sender, string target, decimal amount, TransactionType type)
        {
            Sender = sender;
            Target = target;
            Amount = amount;
            TransactionDate = DateTime.Now; // get the current date ane time off of your computer expressed as local time
            TypeOfTransaction = type;
            TransactionStatus = Status.Pending; // default when transaction is created   
        }

        // Displaying transaction details
        public void PrintTransaction()
        {
            // Resolve currency from sender to target
            string currency = "";
            var senderAcc = SystemOwner.AllAccounts.FirstOrDefault(a => a.AccountNumber == Sender);
            var targetAcc = SystemOwner.AllAccounts.FirstOrDefault(a => a.AccountNumber == Target);
            if (senderAcc != null) currency = senderAcc.Currency;
            else if (targetAcc != null) currency = targetAcc.Currency;

            //Pick color
            ConsoleColor typeColor = TypeOfTransaction switch
            {
                TransactionType.Deposit => ConsoleColor.DarkGreen,
                TransactionType.Withdrawal => ConsoleColor.DarkRed
            };

            Console.WriteLine("────────────────────────────────────────");   
            Console.Write($"{TransactionDate:g} | ");
            var prev = Console.ForegroundColor;
            Console.ForegroundColor = typeColor;
            Console.Write(TypeOfTransaction);
            Console.ForegroundColor = prev;
            Console.WriteLine($" | {TransactionStatus}");

            
            Console.WriteLine($"From: {Sender}");
            Console.WriteLine($"To:   {Target}");
            Console.WriteLine($"Amount: {Amount} {currency}");
        }

    }
}