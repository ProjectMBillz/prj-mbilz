using System;
using System.Collections.Generic;
using System.Text;
using CBC.Framework.ISO8583.DTO;
using CBC.Framework.ISO8583.Utility;
using CBC.Framework.ISO8583.Client.Configuration;
using CBC.Framework.Utility;

namespace CBC.Framework.ISO8583.Client.Messages
{
    public class Payment : FinancialMessage
    {
        public Payment(CardAcceptor cardAcceptor, Account fromAccount, Account toAccount, Amount transferAmount, CardDetails theCard, string transactionID, bool isRepeat)
            : base(cardAcceptor, transactionID, TransactionType.Payment, theCard, fromAccount.Type, toAccount.Type, transferAmount, isRepeat)
        {
            if (fromAccount != null)
            {
                this.Fields.Add(FieldNos.F102_Account1, fromAccount.Number);
            }
            if (toAccount != null)
            {
                this.Fields.Add(FieldNos.F103_Account2, toAccount.Number);
            }
            this.Fields.Add(FieldNos.F100_ReceivingInstitutionID, "100002");
//            this.Fields.Add(FieldNos.F101_InstitutionCode, "40");
        }
    }
}
