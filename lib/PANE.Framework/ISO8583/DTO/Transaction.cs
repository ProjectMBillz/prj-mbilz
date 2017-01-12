using System;
using System.Collections.Generic;
using System.Text;
using CBC.Framework.ISO8583.Utility;
using System.Runtime.Serialization;

namespace CBC.Framework.ISO8583.DTO
{
    /// <summary>
    /// Represents a Transaction
    /// </summary>
    [DataContract]
    public class Transaction
    {
        [DataMember]
        public int SeqNo { get; set; }
        [DataMember]
        public TransactionType Type { get; set; }
        [DataMember]
        public DateTime Date { get; set; }
        [DataMember]
        public Account FromAccount { get; set; }
        [DataMember]
        public Account ToAccount { get; set; }
        [DataMember]
        public Amount TheAmount { get; set; }
        [DataMember]
        public long Surcharge { get; set; }
        [DataMember]
        public string TerminalID { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Transaction"/> class.
        /// </summary>
        public Transaction()
        {

        }


    }
}
