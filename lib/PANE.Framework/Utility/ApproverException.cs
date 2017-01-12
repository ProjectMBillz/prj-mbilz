using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace CBC.Framework.Utility
{
    public class ApproverException : Exception
    {
        public ApproverException(string message) :
            base(message)
        {
        }

        public ApproverException(string message, Exception ex)
            : base(message, ex)
        {
        }
    }

    [DataContract]
    public class ApproverWCFException
    {
        [DataMember]
        public string Message { get; set; }

        [DataMember]
        public string Code { get; set; }

        public ApproverWCFException(string message)
        {
            this.Message = message;
        }
    }
    
}
