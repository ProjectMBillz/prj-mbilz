using System;
using System.Collections.Generic;
using System.Text;
using CBC.Framework.ISO8583.DTO;
using CBC.Framework.ISO8583.Utility;
using CBC.Framework.Utility;

namespace CBC.Framework.ISO8583.Client.Messages
{
    public class Reversal : Message
    {
        public Reversal(CardAcceptor terminal, string transactionID, string stan, FundsTransfer ftMsg, bool isRepeat)
            : base(420, transactionID, isRepeat)
        {
            foreach (Trx.Messaging.Field field in ftMsg.Fields)
            {
                if (!this.Fields.Contains(field.FieldNumber))
                {
                    this.Fields.Add(field);
                }
            }
            this.Fields.Add(FieldNos.F11_Trace, stan);
            /*this.Fields.Add(ftMsg.Fields[FieldNos.F2_PAN]);


            this.Fields.Add(ftMsg.Fields[FieldNos.F3_ProcCode]);

            this.Fields.Add(ftMsg.Fields[FieldNos.F102_Account1]);
            this.Fields.Add(ftMsg.Fields[FieldNos.F103_Account2]);

            this.Fields.Add(ftMsg.Fields[FieldNos.F4_TransAmount]);
            this.Fields.Add(ftMsg.Fields[FieldNos.F49_TransCurrencyCode]);*/



            /*this.set(Fields.ORIGINAL_AMOUNT, ft.get(Fields.TRANSACTION_AMOUNT).ToString().PadLeft(32, '0'));
            //this.set(Fields.RESERVED_FIELD_127, ft.get(Fields.RESERVED_FIELD_127).ToString().Replace("PYT", "REV"));
            String str = ft.get(Fields.ACQUIRING_INSTITUTION_ID).ToString();
            String orgdata = orgmti + ft.get(Fields.SYSTEM_TRACE_AUDIT).ToString() + ft.get(Fields.LOCAL_TRANSACTION_DATETIME) + "11" + str.PadLeft(11, '0');
            this.set(56, orgdata);
            this.set(Fields.FUNCTION_CODE, "400");*/
        }
    }
}
