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
        public static Dictionary<(CurrencyType From, CurrencyType To), decimal> rates =
        new Dictionary<(CurrencyType, CurrencyType), decimal>()
        {
            // ===== Base Currency: SEK =====
            { (CurrencyType.SEK, CurrencyType.USD), 0.091m },
            { (CurrencyType.SEK, CurrencyType.EUR), 0.083m },
            { (CurrencyType.SEK, CurrencyType.JPY), 16.30m },

            // Reverse conversions
            { (CurrencyType.USD, CurrencyType.SEK), 1 / 0.091m },
            { (CurrencyType.EUR, CurrencyType.SEK), 1 / 0.083m },
            { (CurrencyType.JPY, CurrencyType.SEK), 1 / 16.30m },

            // Cross conversions via SEK
            { (CurrencyType.USD, CurrencyType.EUR), (1 / 0.091m) * 0.083m },
            { (CurrencyType.EUR, CurrencyType.USD), (1 / 0.083m) * 0.091m },

            { (CurrencyType.USD, CurrencyType.JPY), (1 / 0.091m) * 16.30m },
            { (CurrencyType.JPY, CurrencyType.USD), (1 / 16.30m) * 0.091m },

            { (CurrencyType.EUR, CurrencyType.JPY), (1 / 0.083m) * 16.30m },
            { (CurrencyType.JPY, CurrencyType.EUR), (1 / 16.30m) * 0.083m }
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
            if (rates.TryGetValue((fromCurrency, toCurrency), out var rate))
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