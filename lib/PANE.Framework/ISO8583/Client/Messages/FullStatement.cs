using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CBC.Framework.ISO8583.Utility;
using CBC.Framework.Utility;
using CBC.Framework.ISO8583.DTO;
using Trx.Messaging;

namespace CBC.Framework.ISO8583.Client.Messages
{
    internal class FullStatement : AuthorizationMessage
    {
        public FullStatement(CardAcceptor cardAcceptor, Account acct, CardDetails theCard, string transactionID, string request, bool isRepeat)
            : base(cardAcceptor, transactionID, TransactionType.FullStatement, theCard,acct.Type, "00", new Amount(0, "566", AmountType.AvailableBalance), isRepeat)
        {
            this.Fields.Add(FieldNos.F102_Account1, acct.Number);

            Trx.Messaging.Message inner = new Trx.Messaging.Message();
            inner.Fields.Add(22,request);
            this.Fields.Add(127, inner);

        }
    }
}
