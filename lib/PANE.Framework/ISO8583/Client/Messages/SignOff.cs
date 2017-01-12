using System;
using System.Collections.Generic;
using System.Text;
using CBC.Framework.ISO8583.DTO;
using CBC.Framework.ISO8583.Utility;
using CBC.Framework.Utility;

namespace CBC.Framework.ISO8583.Client.Messages
{
    public class SignOff : Message
    {
        public SignOff(CardAcceptor terminal, string transactionID, bool isRepeat)
            : base(800, transactionID, isRepeat)
        {
            this.Fields.Add(FieldNos.F70_NetworkMgtInfoCode, "002");
        }
    }
}
