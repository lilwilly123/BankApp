using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankApp
{
    internal enum CurrencyType
    {
        USD,
        EUR,
        SEK,
        JPY
    }
    internal class CurrencyRate
    {
        public static Dictionary<(CurrencyType From, CurrencyType To), decimal> _rates =
         new Dictionary<(CurrencyType, CurrencyType), decimal>
         {
            {(CurrencyType.USD, CurrencyType.EUR), 0.92m}, // the "m" or "M" suffix defines the decimal number as an actual decimal (its by default a double)
            {(CurrencyType.EUR, CurrencyType.USD), 1.09m},
            {(CurrencyType.USD, CurrencyType.SEK), 11.0m},
            {(CurrencyType.SEK, CurrencyType.USD), 0.091m},
            {(CurrencyType.USD, CurrencyType.JPY), 150.0m},
            {(CurrencyType.JPY, CurrencyType.USD), 0.0067m},
            // we may keep on adding more pairs if needed!
         };
        // properties 
        public CurrencyType FromCurrency { get; private set; }
        public CurrencyType ToCurrency { get; private set; }
        public decimal ExchangeRate { get; private set; }
        public DateTime LastUpdated { get; private set; }
        // constructor
        public CurrencyRate(CurrencyType fromCurrency, CurrencyType toCurrency)
        {
            FromCurrency = fromCurrency;
            ToCurrency = toCurrency;

            //small error handling
            if (_rates.TryGetValue((fromCurrency, toCurrency), out var rate))
            {
                ExchangeRate = rate;
                LastUpdated = DateTime.Now;
            }
            else
            {
                throw new ArgumentException($"Exchange rate not defined for {fromCurrency} -> {toCurrency}");
            }
        }
        //method for conversion return a value
        public decimal Convert(decimal amount)
        {
            return amount * ExchangeRate;
        }
    }
}