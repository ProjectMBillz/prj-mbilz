using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace CBC.Framework.Utility
{
    [DataContract]
    [Serializable]
    public class ApprovalResponse
    {
        public ApprovalResponse()
        {
            this.Successful = true;
        }

        [DataMember]
        public bool DisplayCustomMessage { get; set; }

        [DataMember]
        public bool Successful { get; set; }

        /// <summary>
        /// Only applies if DisplayCustomMessage == true.
        /// </summary>
        [DataMember]
        public string CustomMessage { get; set; }

        [DataMember]
        public object ResponseObject { get; set; }
    }
}
