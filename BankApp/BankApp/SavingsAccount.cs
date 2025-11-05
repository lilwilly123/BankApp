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
       
    }
}
