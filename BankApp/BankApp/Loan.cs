using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankApp
{
    internal class Loan
    {
        public enum LoanStatus

        {
            Activ,
            PaidOff,
            Defaulted,
            Closed,
        }
       
        public decimal PrincipalAmount { get; private set; }
        public decimal OutstandingAmount { get; private set; }
        public float InterestRatePercent { get; private set; }

        public string LoanID { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime DueDate { get; private set; }
        public LoanStatus Status { get; private set; }

        public Loan(decimal principalamount, float interestratepercent, DateTime startdate, DateTime duedate)
        {
            if (principalamount <= 0) throw new ArgumentException("Principal must be positiv.", nameof(principalamount));
            if (interestratepercent < 0) throw new ArgumentException("Interestrate cannot be negative", nameof(interestratepercent));
            if (duedate <= startdate) throw new ArgumentException("Due date must be after Start date", nameof(duedate));

            
            PrincipalAmount = principalamount;
            OutstandingAmount = principalamount;
            InterestRatePercent = interestratepercent;
            StartDate = startdate;
            DueDate = duedate;
            Status = LoanStatus.Activ;
        }
        public void ApplyInterest()
        {
            if (Status != LoanStatus.Activ) return;
            decimal interest = OutstandingAmount * ((decimal)InterestRatePercent / 100m);
            OutstandingAmount += interest;
        }
        public void MakePayment(decimal amount)
        {
            if (Status != LoanStatus.Activ) return;
            OutstandingAmount -= amount;
            if (OutstandingAmount <= 0)
            {
                OutstandingAmount = 0;
                Status = LoanStatus.PaidOff;
            }
        }
        public void CheckDue(DateTime Currentdate)
        {
            if (Status == LoanStatus.Activ && Currentdate < DueDate && OutstandingAmount > 0) 
            Status = LoanStatus.Defaulted;
        }
        public void CloseLoan()
        {
            Status = LoanStatus.Closed;
        }
        public override string ToString()
        {
            return $"Outstanding {OutstandingAmount}, Status {Status}";
        }


        

    } 
        
}
