using System;
using System.Collections.Generic;
using System.Text;
using CBC.Framework.ISO8583.DTO;
using System.Globalization;
using CBC.Framework.ISO8583.Utility;

namespace CBC.Framework.ISO8583.Client.DTO
{

    public class MiniStatementResponse : MessageResponse
    {
        public Account AccountDetails { get; set; }
        public List<Transaction> Transactions { get; set; }

        public MiniStatementResponse(Trx.Messaging.Message responseMessage)
            : base(responseMessage)
        {
            Transactions = new List<Transaction>();
            if (responseMessage.Fields.Contains(48))
            {
                string[] trans = responseMessage.Fields[48].Value.ToString().Split('~');

                //Process Header
                string tranHeader = trans[0];
                string[] tags = tranHeader.Split('|');

                //Extract tags
                Dictionary<string, int> tagIndexs = new Dictionary<string, int>();
                for (int i = 0; i < tags.Length; i++)
                {
                    tagIndexs.Add(tags[i], i);
                }

                //Process Transaction List
                for (int i = 1; i < trans.Length; i++)
                {
                    string[] tranDetails = trans[i].Split('|');

                    if (tranDetails.Length != tagIndexs.Count) continue;

                    Transaction theTransaction = new Transaction();

                    int testIndex;

                    //Sequence Number
                    if (tagIndexs.TryGetValue("SEQ_NR", out testIndex))
                    {
                        theTransaction.SeqNo = Convert.ToInt32(tranDetails[testIndex]);
                    }

                    //Date
                    if (tagIndexs.TryGetValue("DATE_TIME", out testIndex))
                    {
                        theTransaction.Date = DateTime.ParseExact(tranDetails[testIndex],"yyyyMMddHHmmss",CultureInfo.InvariantCulture);
                    }

                    //Type
                    if (tagIndexs.TryGetValue("TRAN_TYPE", out testIndex))
                    {
                        theTransaction.Type =(TransactionType) Enum.ToObject(typeof(TransactionType), Convert.ToInt32(tranDetails[testIndex]));
                    }

                    //From Account
                    if (tagIndexs.TryGetValue("ACC_ID1", out testIndex))
                    {
                        theTransaction.FromAccount = new Account(tranDetails[testIndex], "20");
                    }

                    //To Account
                    if (tagIndexs.TryGetValue("ACC_ID2", out testIndex))
                    {
                        theTransaction.ToAccount = new Account(tranDetails[testIndex], "20");
                    }

                    //Amount
                    if (tagIndexs.TryGetValue("TRAN_AMOUNT", out testIndex))
                    {
                        theTransaction.TheAmount = new Amount(Convert.ToInt32(tranDetails[testIndex]),"566",AmountType.Approved);
                    }

                    //Surcharge
                    if (tagIndexs.TryGetValue("SURCHARGE", out testIndex))
                    {
                        theTransaction.Surcharge = Convert.ToInt64(tranDetails[testIndex]);
                    }

                    //Terminal ID
                    if (tagIndexs.TryGetValue("TERM_ID", out testIndex))
                    {
                        theTransaction.TerminalID = tranDetails[testIndex];
                    }

                    Transactions.Add(theTransaction);
                }

            }
            if (responseMessage.Fields.Contains(102) && responseMessage.Fields.Contains(3))
            {
                AccountDetails = new Account(responseMessage.Fields[102].Value.ToString(), responseMessage.Fields[3].Value.ToString().Substring(2, 2));
            }


        }

    }

}
