using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankApp
{
    internal class SavingsAccount : Account
    {
        public decimal InterestRate { get; set; } // Annual interest rate as a percentage (e.g., 3.5 for 3.5%)

            // Apply interest to balance
            public void ApplyInterest()
            {
                if (Status != AccountStatus.Active)
                    throw new InvalidOperationException("Cannot apply interest to a non-active account.");

                decimal interest = Balance * InterestRate / 100;
                Balance += interest;
            }
        }
    }
