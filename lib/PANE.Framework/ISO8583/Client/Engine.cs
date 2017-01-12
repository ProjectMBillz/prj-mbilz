using System;
using System.Collections.Generic;
using System.Text;
using Trx.Messaging.FlowControl;
using Trx.Messaging;
using Trx.Messaging.Channels;
using Trx.Messaging.Iso8583;
using System.Threading;
using CBC.Framework.ISO8583.Client.DTO;
using CBC.Framework.ISO8583.Client.Messages;
using CBC.Framework.ISO8583.Client.Utility;
using CBC.Framework.ISO8583.Client.Configuration;
using System.Data.SqlTypes;
using CBC.Framework.Utility;
using CBC.Framework.ISO8583.Client.Exceptions;
using CBC.Framework.ISO8583.DTO;
using CBC.Framework.ISO8583.DAO;


namespace CBC.Framework.ISO8583.Client
{
    public class Engine
    {
		private Peer _clientPeer;
        
		private string _hostname;
        private const string Client_NAME = "ISO8583.Client";
        private string _mfbcode;
        public int maxNoRetries = 3;
        public int serverTimeout = 180000;
        private int _port;
        private string _transactionID;
        private CardAcceptor _terminal;
        private CardDetails _theCard;
        private Iso8583Message _lastMessageSent;

        public Iso8583Message LastMessageSent
        {
            get { return _lastMessageSent; }
            set { _lastMessageSent = value; }
        }

        public CardDetails TheCard
        {
            get { return _theCard; }
            set { _theCard = value; }
        }
        public CardAcceptor TheTerminal
        {
            get { return _terminal; }
            set { _terminal = value; }
        }
        public string TransactionID
        {
            get
            {
                return _transactionID;
            }
            set
            {
                _transactionID = value;
            }
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="Engine"/> class. 
        /// </summary>
        public Engine(string mfbCode, CardDetails theCard, string transactionID)
            : this(mfbCode, ConfigurationManager.ISO8583ServerHostname, ConfigurationManager.ISO8583ServerPort, new CardAcceptor(ConfigurationManager.CardAcceptorID, ConfigurationManager.TerminalID), theCard, transactionID)
        {

        }
        /// <summary>
        /// Initializes a new instance of the <see cref="Engine"/> class.
        /// </summary>
        /// <param name="ISO8583ServerHostName">Host Name of the ISO8583 server.</param>
        /// <param name="ISO8583ServerPort">The ISO8583 server port.</param>
        /// <param name="terminal">The terminal.</param>
        public Engine(string mfbCode, string ISO8583ServerHostName, int ISO8583ServerPort, CardAcceptor terminal, CardDetails theCard, string transactionID)
        {
            _mfbcode = mfbCode;
            _hostname = ISO8583ServerHostName;
            _port = ISO8583ServerPort;
            _terminal = terminal;
            _theCard = theCard;
            _transactionID = transactionID;
        }

        public Engine(string mfbCode, Peer clientPeer, CardDetails theCard, string transactionID)
        {
            _mfbcode = mfbCode;
            _clientPeer = clientPeer;
            _terminal = new CardAcceptor(ConfigurationManager.CardAcceptorID, ConfigurationManager.TerminalID);
            _theCard = theCard;
            _transactionID = transactionID;
        }

        public bool Connect()
        {
            // Create a client peer to connect to remote system. The messages
            // will be matched using fields 41 and 11.
            _clientPeer = new ClientPeer(Client_NAME, new TwoBytesNboHeaderChannel(
                new Iso8583Ascii1987BinaryBitmapMessageFormatter(), _hostname, _port),
                new BasicMessagesIdentifier(FieldNos.F41_CardAcceptorTerminalCode, FieldNos.F11_Trace));

            _clientPeer.Connect();
            Thread.Sleep(1000);

            int retries = 0;
            while (retries < maxNoRetries)
            {
                lock (this)
                {
                    if (_clientPeer.IsConnected)
                    {
                        break;
                    }
                    else
                    {
                        _clientPeer.Close();
                        retries++;
                        _clientPeer.Connect();
                    }
                }
                Thread.Sleep(2000);
            }

            return _clientPeer.IsConnected;
        }

        /// <summary>
        /// Does the echo.
        /// </summary>
        /// <returns></returns>
        public MessageResponse DoEcho()
        {
            MessageResponse response = null;
            Connect();
            lock (this)
            {
                if (_clientPeer.IsConnected)
                {

                    // Build echo test message.
                    Echo echoMsg = new Echo(_terminal, _transactionID, false);

                    Trx.Messaging.Message responseMessage = ProcessRequest(echoMsg);
                    response = new BalanceEnquiryResponse(responseMessage);
                    response.TransactionID = _transactionID;
                }
            }
            Close();
            return response;
        }

        public SignOnResponse DoSignOn()
        {
            SignOnResponse response = null;
            lock (this)
            {
                if (_clientPeer.IsConnected)
                {

                    // Build echo test message.
                    SignOn soMsg = new SignOn(_terminal, _transactionID, false);

                    Trx.Messaging.Message responseMessage = ProcessRequest(soMsg);
                    response = new SignOnResponse(responseMessage);
                    response.TransactionID = _transactionID;
                }
            }
            return response;
        }

        public SignOffResponse DoSignOff()
        {
            SignOffResponse response = null;
            lock (this)
            {
                if (_clientPeer.IsConnected)
                {

                    // Build echo test message.
                    SignOff soMsg = new SignOff(_terminal, _transactionID, false);

                    Trx.Messaging.Message responseMessage = ProcessRequest(soMsg);
                    response = new SignOffResponse(responseMessage);
                    response.TransactionID = _transactionID;
                }
            }
            return response;
        }

        public KeyExchangeResponse DoKeyExchange()
        {
            KeyExchangeResponse response = null;
            lock (this)
            {
                if (_clientPeer.IsConnected)
                {

                    // Build echo test message.
                    KeyExchange signOnMsg = new KeyExchange(_terminal, _transactionID, false);

                    Trx.Messaging.Message responseMessage = ProcessRequest(signOnMsg);
                    response = new KeyExchangeResponse(responseMessage);
                    response.TransactionID = _transactionID;
                }
            }
            return response;
        }

        /// <summary>
        /// Does the balance enquiry.
        /// </summary>
        /// <param name="acct">The account.</param>
        /// <returns></returns>
        public BalanceEnquiryResponse DoBalanceEnquiry(Account acct)
        {
            BalanceEnquiryResponse response = null;
            lock (this)
            {
                if (_clientPeer.IsConnected)
                {
                    
                    // Build Balance Enquiry message.
                    BalanceEnquiry beMsg = new BalanceEnquiry(_terminal, acct, _theCard, _transactionID, false);

                    Trx.Messaging.Message responseMessage = ProcessRequest(beMsg);
                    response = new BalanceEnquiryResponse(responseMessage);
                    response.TransactionID = _transactionID;
                }
            }
            return response;
        }

        public LinkedAccountEnquiryResponse DoLinkedAccountEnquiry()
        {
            LinkedAccountEnquiryResponse response = null;
            lock (this)
            {
                if (_clientPeer.IsConnected)
                {

                    // Build Balance Enquiry message.
                    LinkedAccountEnquiry beMsg = new LinkedAccountEnquiry(_terminal, _theCard, _transactionID, false);

                    Trx.Messaging.Message responseMessage = ProcessRequest(beMsg);
                    response = new LinkedAccountEnquiryResponse(responseMessage);
                    response.TransactionID = _transactionID;
                }
            }
            return response;
        }

        public MiniStatementResponse DoMiniStatement(Account acct)
        {
            MiniStatementResponse response = null;
            lock (this)
            {
                if (_clientPeer.IsConnected)
                {

                    // Build Balance Enquiry message.
                    MiniStatement beMsg = new MiniStatement(_terminal, acct, _theCard, _transactionID, false);

                    Trx.Messaging.Message responseMessage = ProcessRequest(beMsg);
                    response = new MiniStatementResponse(responseMessage);
                    response.TransactionID = _transactionID;
                }
            }
            return response;
        }

        public FullStatementResponse DoFullStatement(Account acct, DateTime dtFrom, DateTime dtTo)
        {
            FullStatementResponse response = null;
            lock (this)
            {
                if (_clientPeer.IsConnected)
                {
                    string request = string.Format(@"
                        <StatementData>
                                    <Request>
                                        <RequestRange>
                                            <StartDate>{0:yyyyMMdd}</StartDate>
                                            <EndDate>{1:yyyyMMdd}</EndDate>
                                        </RequestRange>
                                    </Request>
                               </StatementData>", dtFrom, dtTo);
                    request = request.Trim().Replace("\r\n", "");
                    string key = "StatementData";
                    string structuredData = string.Format("{0}{1}{2}{3}{4}{5}", key.Length.ToString().Length, key.Length, key, request.Length.ToString().Length, request.Length, request);
                    // Build Balance Enquiry message.
                    FullStatement feMsg = new FullStatement(_terminal, acct, _theCard, _transactionID, structuredData, false);

                    Trx.Messaging.Message responseMessage = ProcessRequest(feMsg);
                    response = new FullStatementResponse(responseMessage);
                    response.TransactionID = _transactionID;
                }
            }
            return response;
        }

        public HotListCardResponse DoHotListCard(Account acct, string messageReasonCode)
        {
            HotListCardResponse response = null;
            lock (this)
            {
                if (_clientPeer.IsConnected)
                {
                    // Build HotList Card message.
                    HotListCard beMsg = new HotListCard(_terminal, acct, _theCard, messageReasonCode,_transactionID, false);

                    Trx.Messaging.Message responseMessage = ProcessRequest(beMsg);
                    response = new HotListCardResponse(responseMessage);
                    response.TransactionID = _transactionID;
                }
            }
            return response;
        }

        /// <summary>
        /// Does the funds transfer.
        /// </summary>
        /// <param name="fromAccount">From account.</param>
        /// <param name="toAccount">To account.</param>
        /// <param name="transferAmount">The transfer amount.</param>
        /// <returns></returns>
        public FundsTransferResponse DoFundsTransfer(Account fromAccount, Account toAccount, Amount transferAmount)
        {
            FundsTransferResponse response = null;
            lock (this)
            {
                if (_clientPeer.IsConnected)
                {
                    
                    // Build Funds Transfer message.
                    FundsTransfer ftMsg = new FundsTransfer(_terminal, fromAccount, toAccount, transferAmount, _theCard, _transactionID, false);

                    Trx.Messaging.Message responseMessage = ProcessRequest(ftMsg);
                    response = new FundsTransferResponse(responseMessage);
                    response.TransactionID = _transactionID;
                }
            }
            return response;
        }

        public PaymentResponse DoPayment(Account fromAccount, Account toAccount, Amount amount)
        {
            PaymentResponse response = null;
            lock (this)
            {
                if (_clientPeer.IsConnected)
                {
                    // Build Funds Transfer message.
                    Payment pytMsg = new Payment(_terminal, fromAccount, toAccount, amount, _theCard, _transactionID, false);

                    Trx.Messaging.Message responseMessage = ProcessRequest(pytMsg);
                    response = new PaymentResponse(responseMessage);
                    response.TransactionID = _transactionID;
                }
            }
            return response;
        }

        public PaymentResponse DoAppZoneSwitchPayment(Account fromAccount, Account toAccount, Amount amount, string receivingInstitutionID)
        {
            PaymentResponse response = null;
            lock (this)
            {
                if (_clientPeer.IsConnected)
                {
                    // Build Funds Transfer message.
                    Payment pytMsg = new Payment(_terminal, fromAccount, toAccount, amount, _theCard, _transactionID, false);
                    pytMsg.Fields.Add(FieldNos.F100_ReceivingInstitutionID, receivingInstitutionID);

                    Trx.Messaging.Message responseMessage = ProcessRequest(pytMsg);
                    response = new PaymentResponse(responseMessage);
                    response.TransactionID = _transactionID;
                }
            }
            return response;
        }

        public PaymentResponse DoCipgPayment(Account fromAccount, Account toAccount, Amount amount)
        {
            PaymentResponse response = null;
            lock (this)
            {
                if (_clientPeer.IsConnected)
                {
                    // Build Funds Transfer message.
                    Payment pytMsg = new Payment(_terminal, fromAccount, toAccount, amount, _theCard, _transactionID, false);
                    pytMsg.Fields.Remove(FieldNos.F102_Account1);

                    Trx.Messaging.Message responseMessage = ProcessRequest(pytMsg);
                    response = new PaymentResponse(responseMessage);
                    response.TransactionID = _transactionID;
                }
            }
            return response;
        }

        /// <summary>
        /// Does the reversal.
        /// </summary>
        /// <param name="stan">The STAN.</param>
        /// <param name="fromAccount">From account.</param>
        /// <param name="toAccount">To account.</param>
        /// <param name="transferAmount">The transfer amount.</param>
        /// <returns></returns>
        public MessageResponse DoReversal(string stan, string transactionId, Account fromAccount, Account toAccount, Amount transferAmount)
        {
            ReversalResponse response = null;
            lock (this)
            {
                if (_clientPeer.IsConnected)
                {

                    // Build Original Funds Transfer message.
                    FundsTransfer ftMsg = new FundsTransfer(_terminal, fromAccount, toAccount, transferAmount, _theCard, transactionId, false);
                    

                    // Build Reversal message.
                    ISO8583.Client.Messages.Reversal rvMsg = new ISO8583.Client.Messages.Reversal(_terminal, _transactionID, stan, ftMsg, false);

                    Trx.Messaging.Message responseMessage = ProcessRequest(rvMsg);
                    response = new ReversalResponse(responseMessage);
                    response.TransactionID = _transactionID;
                }
            }
            return response;
        }

        public TransactionReversalResponse DoTransactionReversal(Iso8583Message message)
        {
            TransactionReversalResponse response = null;
            lock (this)
            {
                if (_clientPeer.IsConnected)
                {
                    TransactionReversal stMsg = new TransactionReversal(message, _transactionID, false);

                    Trx.Messaging.Message responseMessage = ProcessRequest(stMsg);
                    response = new TransactionReversalResponse(responseMessage);
                }
            }
            return response;
        }

        private Trx.Messaging.Message ProcessRequest(Iso8583Message msg)
        {
            //TODO: Framework Log IsoMessage
            IsoMessage msgLog = new IsoMessage();
            msgLog.Type = msg.MessageTypeIdentifier;
            msgLog.STAN = Convert.ToInt64(msg.Fields[FieldNos.F11_Trace].Value);
            msgLog.Sender = MessageEntity.Client;
            msgLog.Receiver = MessageEntity.ExternalEntity;
            msgLog.RequestDate = DateTime.Now;
            msgLog.Message = CBC.Framework.Utility.BinarySerializer.SerializeObject(msg.Fields);
            msgLog.ResponseDate = (DateTime)SqlDateTime.Null;
            ISO8583DataExtractor ide = new ISO8583DataExtractor(msg);
            msgLog.OriginalDataElements = ide.OriginalDataElements;
            msgLog.TerminalID = ide.TerminalID;
            msgLog.CardAcceptorID = ide.CardAcceptorID;
            msgLog.CardPAN = ide.CardPAN;
            msgLog.Amount = ide.Amount;
            msgLog.TransactionTypeCode = ide.TransactionType;
            msgLog.MFBCode = _mfbcode;
            IsoMessageDAO.Save(msgLog);                  

            LastMessageSent = msg;

            PeerRequest request = new PeerRequest(_clientPeer, msg);
            request.Send();
            request.WaitResponse(serverTimeout);

            if (request.Expired)
            {
                //if (msg is FundsTransfer)
                //{
                //    ////ISO8583.Client.Reversal.Engine.ReversalSystem reversalSystem = new ISO8583.Client.Reversal.Engine.ReversalSystem();
                //    ////reversalSystem.LogReversal(new ISO8583.Client.Reversal.Engine.DTO.Reversal(msg.TransactionID, ISO8583.Client.Reversal.Engine.DTO.ReversalType.ISO8583));          
                //}
                throw new ConnectionTimedOutException(msg);
            }
            else
            {
                Trx.Messaging.Message clo = request.ResponseMessage.Clone() as Trx.Messaging.Message;
                if (clo.Fields.Contains(127))
                {
                    (clo.Fields[127].Value as Trx.Messaging.Message).Parent = null;
                }
                msgLog.ResponseMessage = CBC.Framework.Utility.BinarySerializer.SerializeObject(clo.Fields);
                msgLog.ResponseDate = DateTime.Now;
               // ViaCard.Base.Ninject.Interceptor.Instance.Get<CMS.TransactionProcessor.Engine>().LogMessage(msgLog);
            }
            return request.ResponseMessage;
        }
       
        public void Close()
        {
            lock (this)
            {
                _clientPeer.Close();
            }
        }

        public ChangePINResponse DoChangePIN(Account acct, byte[] newPinBlock)
        {
            ChangePINResponse response = null;
            lock (this)
            {
                if (_clientPeer.IsConnected)
                {
                    // Build HotList Card message.
                    ChangePIN beMsg = new ChangePIN(_terminal, acct, _theCard, newPinBlock, _transactionID, false);

                    Trx.Messaging.Message responseMessage = ProcessRequest(beMsg);
                    response = new ChangePINResponse(responseMessage);
                    response.TransactionID = _transactionID;
                }
            }
            return response;
        }
    }
}
