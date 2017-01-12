using System;
using System.Collections.Generic;
using System.Text;
using CBC.Framework.DTO;


namespace CBC.Framework.ISO8583.DTO
{

    [Serializable]
    public class IsoMessage : DataObject
    {

        private DateTime m_RequestDate;
        private DateTime m_ResponseDate;
        private Byte[] m_Message;
        private Byte[] m_ResponseMessage;
        private Int64 m_STAN;
        private Int32 _Type;
        private MessageEntity m_Sender;
        private MessageEntity m_Receiver;
        private string m_SenderIP;
        private string m_ReceiverIP;
        private string m_OriginalDataElements;
       
       
        public IsoMessage()
        {
        }

        public IsoMessage(long id)
        {
            ID = id;
        }

        public virtual string SenderIP
        {
            get { return m_SenderIP; }
            set { m_SenderIP = value; }
        }

        public virtual string ReceiverIP
        {
            get { return m_ReceiverIP; }
            set { m_ReceiverIP = value; }
        }

        public virtual DateTime RequestDate
        {
            get { return m_RequestDate; }
            set { m_RequestDate = value; }
        }


        public virtual DateTime ResponseDate
        {
            get { return m_ResponseDate; }
            set { m_ResponseDate = value; }
        }

        public virtual Byte[] Message
        {
            get { return m_Message; }
            set { m_Message = value; }
        }

        public virtual Byte[] ResponseMessage
        {
            get { return m_ResponseMessage; }
            set { m_ResponseMessage = value; }
        }

        public virtual Int64 STAN
        {
            get { return m_STAN; }
            set { m_STAN = value; }
        }

        public virtual Int32 Type
        {
            get { return _Type; }
            set { _Type = value; }
        }

        public virtual MessageEntity Sender
        {
            get { return m_Sender; }
            set { m_Sender = value; }
        }


        public virtual MessageEntity Receiver
        {
            get { return m_Receiver; }
            set { m_Receiver = value; }
        }


        public virtual string OriginalDataElements
        {
            get { return m_OriginalDataElements; }
            set { m_OriginalDataElements = value; }
        }

        public virtual string CardPAN { get; set; }

        public virtual long Amount { get; set; }

        public virtual string TransactionTypeCode { get; set; }

        public virtual string CardAcceptorID { get; set; }

        public virtual string TerminalID { get; set; }

    }
}