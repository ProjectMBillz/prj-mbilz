using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CBC.Framework.ISO8583.DTO;
using Trx.Messaging;
using CBC.Framework.ISO8583.Client.Utility;
using System.Globalization;
using CBC.Framework.ISO8583.Utility;

namespace CBC.Framework.ISO8583.Client.DTO
{
    public class FullStatementResponse : MessageResponse
    {
        public IList<FullTransaction> Transactions { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TransactionCount { get; set; }
        public string CurrencyCode { get; set; }
        public string ResponseXML { get; set; }
        public string StucturedData { get; set; }

        public FullStatementResponse(Trx.Messaging.Message responseMessage)
            : base(responseMessage)
        {

            Transactions = new List<FullTransaction>();

            if (responseMessage.Fields.Contains(127))
            {
                Message outer = responseMessage.Fields[127].Value as Message;
                if (outer.Fields.Contains(22))
                {
                    string structData = outer.Fields[22].Value.ToString();
                    StucturedData = structData;
                    if (!String.IsNullOrEmpty(structData))
                    {
                        try
                        {
                            int keyLengthLength = Convert.ToInt32(structData.Substring(0, 1));
                            int keyLength = Convert.ToInt32(structData.Substring(1, keyLengthLength));
                            string key = structData.Substring(keyLengthLength + 1, keyLength);
                            if (key.Equals("StatementData", StringComparison.InvariantCultureIgnoreCase))
                            {
                                int valLengthLength = Convert.ToInt32(structData.Substring(keyLength + keyLengthLength + 1, 1));
                                int valLength = Convert.ToInt32(structData.Substring(keyLength + keyLengthLength + 2, valLengthLength));
                                ResponseXML = structData.Substring(keyLength + keyLengthLength + valLengthLength + 2, valLength);
                                Transactions = FullStatementXmlParser.GetFullStatement(ResponseXML);
                                foreach (FullTransaction trans in Transactions)
                                {
                                    try
                                    {
                                        trans.TransactionType = ((TransactionType)Enum.ToObject(typeof(TransactionType), Convert.ToInt32(trans.TransactionType))).ToString();
                                    }
                                    catch
                                    {
                                        //trans.TransactionType 
                                    }
                                }

                                TransactionCount = Transactions.Count;
                                StartDate = DateTime.ParseExact(FullStatementXmlParser.GetXmlElement(ResponseXML, "RequestRange.StartDate"), "yyyyMMdd", DateTimeFormatInfo.InvariantInfo);
                                EndDate = DateTime.ParseExact(FullStatementXmlParser.GetXmlElement(ResponseXML, "RequestRange.EndDate"), "yyyyMMdd", DateTimeFormatInfo.InvariantInfo);
                                CurrencyCode = FullStatementXmlParser.GetXmlElement(ResponseXML, "RequestRange.CurrencyCode");
                            }
                        }
                        catch
                        {

                            //Just forget about it
                        }
                    }
                    }
            }




        }

    }
}
