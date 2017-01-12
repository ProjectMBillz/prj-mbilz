using System;
using System.Collections.Generic;
using System.Text;
using CBC.Framework.ISO8583.DTO;
using System.Runtime.Serialization;

namespace CBC.Framework.ISO8583.Client.DTO
{
    [DataContract]
    public class BalanceEnquiryResponse : MessageResponse
    {
        private string _AccountName;    
        private Account _AccountDetails;
        private Amount _AvailableBalance;
        private Amount _LedgerBalance;

        public BalanceEnquiryResponse(Trx.Messaging.Message responseMessage)
            : base(responseMessage)
        {
            if (responseMessage.Fields.Contains(54))
            {
                this._LedgerBalance = new Amount(responseMessage.Fields[54].Value.ToString().Substring(0, 20));
                this._AvailableBalance = new Amount(responseMessage.Fields[54].Value.ToString().Substring(20,20));
            }
            if (responseMessage.Fields.Contains(102) && responseMessage.Fields.Contains(3))
            {
                this._AccountDetails = new Account(responseMessage.Fields[102].Value.ToString(), responseMessage.Fields[3].Value.ToString().Substring(2,2));
            }
            
            
        }

        [DataMember]
        public Amount AvailableBalance
        {
            get
            {
                return this._AvailableBalance;
            }
            set
            {
                this._AvailableBalance = value;
            }
        }

        [DataMember]
        public Account AccountDetails
        {
            get
            {
                return this._AccountDetails;
            }
            set
            {
                this._AccountDetails = value;
            }
        }

        [DataMember]
        public string AccountName
        {
            get
            {
                return this._AccountName;
            }
            set
            {
                this._AccountName = value;
            }
        }

        [DataMember]
        public Amount LedgerBalance
        {
            get
            {
                return this._LedgerBalance;
            }
            set
            {
                this._LedgerBalance = value;
            }
        }
    }
}
