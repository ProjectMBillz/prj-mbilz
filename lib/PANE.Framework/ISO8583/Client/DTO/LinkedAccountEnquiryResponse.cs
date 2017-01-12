using System;
using System.Collections.Generic;
using System.Text;
using CBC.Framework.ISO8583.DTO;
using CBC.Framework.Utility;
using CBC.Framework.ISO8583.Utility;
using System.Runtime.Serialization;

namespace CBC.Framework.ISO8583.Client.DTO
{
    [DataContract]
    public class LinkedAccountEnquiryResponse : MessageResponse
    {   
        private Dictionary<Account,Amount> _Accounts;

        public LinkedAccountEnquiryResponse(Trx.Messaging.Message responseMessage)
            : base(responseMessage)
        {
            _Accounts = new Dictionary<Account, Amount>();
            if (responseMessage.Fields.Contains(FieldNos.F48_AdditionalData))
            {
                string result = responseMessage.Fields[FieldNos.F48_AdditionalData].Value.ToString();
                while (!String.IsNullOrEmpty(result) && result.Length >= 46)
                {
                    Account theAccount = new Account(result.Substring(0, 28), result.Substring(28, 2));
                    Amount theAmount = new Amount((result.Substring(33, 1).Equals("D") ? -1 : 1) * Convert.ToInt64(result.Substring(34, 12)), result.Substring(30, 3), AmountType.LedgerBalance);
                    _Accounts.Add(theAccount, theAmount);
                }
            }          
            
        }

        [DataMember]
        public Dictionary<Account, Amount> Accounts
        {
            get
            {
                return this._Accounts;
            }
            set
            {
                this._Accounts = value;
            }
        }

    }
}
