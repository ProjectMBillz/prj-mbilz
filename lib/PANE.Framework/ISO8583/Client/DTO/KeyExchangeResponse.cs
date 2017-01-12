using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace CBC.Framework.ISO8583.Client.DTO
{
    [DataContract]
    public class KeyExchangeResponse : MessageResponse
    {
        private string _SessionKey;

        
        public KeyExchangeResponse(Trx.Messaging.Message responseMessage)
            : base(responseMessage)
        {
            if (responseMessage.Fields.Contains(53))
            {
                _SessionKey = responseMessage.Fields[53].Value.ToString().Substring(1,16);
            }
        }

        [DataMember]
        public string SessionKey
        {
            get { return _SessionKey; }
            set { _SessionKey = value; }
        }


    }
}
