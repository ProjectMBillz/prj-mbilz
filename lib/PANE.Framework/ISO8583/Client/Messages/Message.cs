using System;
using System.Collections.Generic;
using System.Text;
using Trx.Messaging.Iso8583;
using Trx.Messaging;
using CBC.Framework.ISO8583.DTO;
using CBC.Framework.ISO8583.Utility;
using CBC.Framework.Utility;
using CBC.Framework.ISO8583.Client.Utility;

namespace CBC.Framework.ISO8583.Client.Messages
{
    public abstract class Message : Iso8583Message
    {
        private string _transactionID;

        public bool IsRepeat { get; set; }

        public string TransactionID
        {
            get { return _transactionID; }
            set { _transactionID = value; }
        }

        internal IsolatedStanSequencer _sequencer;

        public Message(int messageTypeIdentifier, string transactionID, bool isRepeat)
            : base(isRepeat ? messageTypeIdentifier + 1 : messageTypeIdentifier)
        {
            _transactionID = transactionID;
            _sequencer = new IsolatedStanSequencer();
            DateTime transmissionDate = DateTime.Now;
            this.Fields.Add(FieldNos.F7_TransDateTime, string.Format("{0}{1}",
                string.Format("{0:00}{1:00}", transmissionDate.Month, transmissionDate.Day),
                string.Format("{0:00}{1:00}{2:00}", transmissionDate.Hour,
                transmissionDate.Minute, transmissionDate.Second)));
            this.Fields.Add(FieldNos.F11_Trace, _sequencer.Increment().ToString());
            this.Fields.Add(FieldNos.F12_TransLocalTime, string.Format("{0:00}{1:00}{2:00}", transmissionDate.Hour,
                transmissionDate.Minute, transmissionDate.Second));
            this.Fields.Add(FieldNos.F13_TransLocalDate, string.Format("{0:00}{1:00}", transmissionDate.Month, transmissionDate.Day));

           /* if (!String.IsNullOrEmpty(Configuration.ConfigurationManager.AcquiringInstitutionIDCode.Trim()))
            {
                this.Fields.Add(FieldNos.F32_AcquiringInstitutionIDCode, Configuration.ConfigurationManager.AcquiringInstitutionIDCode);
            }
            */
            this.Fields.Add(FieldNos.F37_RetrievalReference, _sequencer.CurrentValue().ToString());
        }


    }
}
