using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CBC.Framework.DTO;
using CBC.Framework.Utility;
using CBC.Framework.ISO8583.Utility;

namespace CBC.Framework.ISO8583.DTO
{
    public class FullTransaction : DataObject
    {
        public virtual string TransactionType { get; set; }
        public virtual string FromAccountType { get; set; }
        public virtual string FromAccountID { get; set; }
        public virtual string ToAccountType { get; set; }
        public virtual string ToAccountID { get; set; }
        public virtual DateTime TransactionDate { get; set; }
        public virtual DateTime PostDate { get; set; }
        public virtual string Description { get; set; }
        public virtual string ReferenceNumber { get; set; }
        public virtual string IsComplete { get; set; }
        

        //Other objects
        [XMLName("TransactionAmount")]
        public TransAmount TransAmount { get; set; }
        [XMLName("PostAmount")]
        public TransAmount PostAmt { get; set; }
        [XMLName("Surcharge")]
        public TransAmount SurCharge { get; set; }
        [XMLName("Fee")]
        public Fee fee { get; set; }
        [XMLName("Balance")]
        public Balance balance { get; set; }
        [XMLName("OpeningBalance")]
        public List<Balance> Openingbalance { get; set; }
        [XMLName("ClosingBalance")]
        public List<Balance> Closingbalance { get; set; }
    }
}
