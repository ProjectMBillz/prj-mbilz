using System;
using System.Collections.Generic;
using System.Text;
using CBC.Framework.ISO8583.DTO;
using CBC.Framework.ISO8583.Utility;
using CBC.Framework.ISO8583.Client.Configuration;
using CBC.Framework.Utility;

namespace CBC.Framework.ISO8583.Client.Messages
{
    public class BalanceEnquiry : FinancialMessage
    {
        public BalanceEnquiry(CardAcceptor cardAcceptor, Account acct, CardDetails theCard, string transactionID, bool isRepeat)
            : base(cardAcceptor, transactionID, TransactionType.BalanceEnquiry, theCard,acct.Type, "00", new Amount(0, "566", AmountType.AvailableBalance), isRepeat)
        {
            this.Fields.Add(FieldNos.F102_Account1, acct.Number);

        }
    }
}
