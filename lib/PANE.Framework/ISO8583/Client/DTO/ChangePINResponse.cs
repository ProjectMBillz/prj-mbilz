using System;
using System.Collections.Generic;
using System.Text;

namespace CBC.Framework.ISO8583.Client.DTO
{
    public class ChangePINResponse : MessageResponse
    {
        public ChangePINResponse(Trx.Messaging.Message responseMessage)
            : base(responseMessage)
        {
           
        }
    }
}
