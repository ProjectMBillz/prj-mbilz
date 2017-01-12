using System;
using System.Collections.Generic;
using System.Text;

namespace CBC.Framework.ISO8583.Utility
{
    public class AmountType
    {
        public const string LedgerBalance = "01";
        public const string AvailableBalance = "02";
        public const string RemainCycle = "20";
        public const string Cash = "40";
        public const string Approved = "53"; 
        public const string AvailableCredit = "90";
        public const string CreditLimit = "91";
    }
}
