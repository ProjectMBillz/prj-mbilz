using System;
using System.Collections.Generic;
using System.Text;
using Trx.Messaging.Iso8583;

namespace CBC.Framework.ISO8583.Client.Exceptions
{
    public class ConnectionTimedOutException : Exception
    {
        private Iso8583Message _message;

        public Iso8583Message MessageSent
        {
            get
            {
                return _message;
            }
        }

        public ConnectionTimedOutException (Iso8583Message messsageSent) : base("Connection Timed Out.")
        {
            _message = messsageSent;
        }
    }
}
