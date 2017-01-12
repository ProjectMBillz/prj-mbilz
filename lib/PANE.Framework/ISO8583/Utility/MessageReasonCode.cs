using System;
using System.Collections.Generic;
using System.Text;

namespace CBC.Framework.ISO8583.Utility
{
    public class MessageReasonCode
    {
        public const string LostCard = "3000";
        public const string StolenCard = "3001";
        public const string UndeliveredCard = "3002";
        public const string CounterfeitCard = "3003";
        public const string LostPIN = "3700";

    }
}
