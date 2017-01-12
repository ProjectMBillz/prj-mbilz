using System;
using System.Collections.Generic;
using System.Text;
using Trx.Messaging.Iso8583;
using Trx.Messaging;
using CBC.Framework.ISO8583.DTO;
using CBC.Framework.ISO8583.Utility;
using CBC.Framework.ISO8583.Client.Configuration;
using CBC.Framework.Utility;

namespace CBC.Framework.ISO8583.Client.Messages
{
    public abstract class FinancialMessage : CardBasedMessage
    {

        public FinancialMessage(CardAcceptor cardAcceptor, string transactionID, TransactionType transType, CardDetails theCard, string fromAccountType, string toAccountType, Amount transferAmount, bool isRepeat)
            : base(200, cardAcceptor, theCard, transactionID, isRepeat)
        {
            this.Fields.Add(FieldNos.F3_ProcCode, string.Format("{0}{1}{2}", Convert.ToString((int)transType).PadLeft(2, '0'), fromAccountType, toAccountType));
            this.Fields.Add(FieldNos.F4_TransAmount, transferAmount.Balance.ToString());

            this.Fields.Add(FieldNos.F49_TransCurrencyCode, transferAmount.CurrencyCode);

            //this.Fields.Add(100, "100001");

            Trx.Messaging.Message msg = new Trx.Messaging.Message();
            msg.Fields.Add(19, ConfigurationManager.BankDetail);

            this.Fields.Add(127, msg);
        }


    }
}
