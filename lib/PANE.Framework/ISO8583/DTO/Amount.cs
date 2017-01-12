using System;
using System.Collections.Generic;
using System.Text;

namespace CBC.Framework.ISO8583.DTO
{
    public class Amount
    {
        private long _Balance;
        private string _CurrencyCode;
        private string _AmountType;

        /// <summary>
        /// Initializes a new instance of the <see cref="Amount"/> class.
        /// </summary>
        /// <param name="balance">The balance.</param>
        /// <param name="currencyCode">The currency code.</param>
        public Amount(long balance, string currencyCode, string amountType)
        {
            this._Balance = balance;
            this._AmountType = amountType;
            this._CurrencyCode = currencyCode;
        }

        internal Amount(string isoAmount)
        {
            if (!String.IsNullOrEmpty(isoAmount) && isoAmount.Length == 20)
            {
                this._AmountType = isoAmount.Substring(2, 2);
                this._CurrencyCode = isoAmount.Substring(4, 3);
                this._Balance = Convert.ToInt64(isoAmount.Substring(8, 12));
                this._Balance *= isoAmount.Substring(7, 1).Equals("C", StringComparison.CurrentCultureIgnoreCase) ? 1 : -1;
            }
        }

        /// <summary>
        /// Gets or sets the balance.
        /// </summary>
        /// <value>The balance.</value>
        public long Balance
        {
            get
            {
                return this._Balance;
            }
            set
            {
                this._Balance = value;
            }
        }

        /// <summary>
        /// Gets or sets the currency code.
        /// </summary>
        /// <value>The currency code.</value>
        public string CurrencyCode
        {
            get
            {
                return this._CurrencyCode;
            }
            set
            {
                this._CurrencyCode = value;
            }
        }

        public string AmountType
        {
            get
            {
                return this._AmountType;
            }
            set
            {
                this._AmountType = value;
            }
        }

    }
}
