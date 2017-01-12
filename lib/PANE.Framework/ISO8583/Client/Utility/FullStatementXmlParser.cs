using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using CBC.Framework.ISO8583.DTO;
using System.Reflection;
using System.Globalization;
using CBC.Framework.Utility;
using CBC.Framework.ISO8583.Utility;

namespace CBC.Framework.ISO8583.Client.Utility
{
    
    public class FullStatementXmlParser
    {
        public static string GetXmlElement(string XML, string elementName)
        {

            string element_value = string.Empty;

            var parentXml = from c in XElement.Parse(XML).Elements() select c;
            try
            {
                string[] elements = elementName.Split('.');
                XElement xElement = parentXml.Nodes().Cast<XElement>().SingleOrDefault(x => x.Name == elements[0]);

                for (int i = 1; i < elements.Length; i++)
                {
                    xElement = xElement.Nodes().Cast<XElement>().SingleOrDefault(x => x.Name == elements[i]);
                }
                element_value = xElement.Value;
            }
            catch { }

            return element_value;
        }

        public static IList<FullTransaction> GetFullStatement(string xmlResponse)
        {
            return GetTransactions(xmlResponse);
        }

        protected static List<FullTransaction> GetTransactions(string xml)
        {
            // step 1: extract the transactions list in xml first
            string strTransList = GetXmlElementBlock(xml, "ResponseRange.TransactionList");

            // step 2: remove unwanted elements in the content
            strTransList = strTransList.Replace("<TransactionList>", string.Empty);
            strTransList = strTransList.Replace("</TransactionList>", string.Empty);

            // step 3: split the transactions list into individual transactions (still in xml string though)
            string[] del = new string[1];
            del[0] = "</Transaction>";
            string[] list = strTransList.Split(del, StringSplitOptions.None);

            // step 4: parse the transaction xml into transaction object
            List<FullTransaction> transactions = new List<FullTransaction>();
            string[] innerTypes = { "Balance", "Fee", "TransAmount", "List`1" };
            string currentXmlTrans = string.Empty;
            object prop_value = null;
            DateTime date_prop = DateTime.MinValue;
            foreach (string item in list)
            {
                if (item.Contains("Transaction"))
                {
                    currentXmlTrans = string.Format("<StatementData> <Response> {0} </Transaction> </Response>  </StatementData>", item);
                    FullTransaction tran = new FullTransaction();
                    PropertyInfo[] properties = typeof(FullTransaction).GetProperties();
                    foreach (PropertyInfo property in properties)
                    {
                        try
                        {
                            if (innerTypes.Contains(property.PropertyType.Name))
                            {
                                string elemen_name = string.Empty;
                                // get the elementName from the objType's XMLName attribute
                                try
                                {

                                    Attribute[] element_attrib = (Attribute[])property.GetCustomAttributes(typeof(XMLNameAttribute), false);
                                    if (element_attrib[0] != null)
                                    {
                                        PropertyInfo pi = element_attrib[0].GetType().GetProperty("Name");
                                        elemen_name = pi.GetValue(element_attrib[0], null) == null ? "" : pi.GetValue(element_attrib[0], null).ToString();
                                    }
                                }
                                catch { }

                                property.SetValue(tran, CreateInnerObject(property.PropertyType, currentXmlTrans, elemen_name), null);
                            }
                            else if (property.PropertyType.Name == "DateTime")
                            {
                                prop_value = GetXmlElement(currentXmlTrans, string.Format("Transaction.{0}", property.Name));
                                date_prop = DateTime.ParseExact(prop_value.ToString(), "yyyyMMdd", DateTimeFormatInfo.InvariantInfo);
                                property.SetValue(tran, date_prop, null);
                            }
                            else
                            {
                                prop_value = GetXmlElement(currentXmlTrans, string.Format("Transaction.{0}", property.Name));
                                property.SetValue(tran, prop_value, null);
                            }
                        }
                        catch { }
                    }
                    transactions.Add(tran);
                }
            }
            return transactions;
        }
        protected static string GetXmlElementBlock(string reqXML, string elementName)
        {

            string element_value = string.Empty;

            var parentXml = from c in XElement.Parse(reqXML).Elements() select c;
            try
            {
                string[] elements = elementName.Split('.');
                XElement xElement = parentXml.Nodes().Cast<XElement>().SingleOrDefault(x => x.Name == elements[0]);

                for (int i = 1; i < elements.Length; i++)
                {
                    xElement = xElement.Nodes().Cast<XElement>().SingleOrDefault(x => x.Name == elements[i]);
                }
                element_value = xElement.ToString();
            }
            catch { }

            return element_value;
        }
        protected static object CreateInnerObject(Type objType, string xml, string xmlElementName)
        {
            object entity = Activator.CreateInstance(objType);
            if (xmlElementName == "OpeningBalance" || xmlElementName == "ClosingBalance")
            {
                entity = CreateInnerBalanceList(xml, xmlElementName);
            }
            else
            {
                PropertyInfo[] properties = objType.GetProperties();
                object prop_value = string.Empty;
                foreach (PropertyInfo property in properties)
                {
                    try
                    {
                        prop_value = GetXmlElement(xml, string.Format("Transaction.{0}.{1}", xmlElementName, property.Name));
                        property.SetValue(entity, prop_value, null);
                    }
                    catch { }
                }
            }
            return entity;
        }
        private static List<Balance> CreateInnerBalanceList(string xml, string xmlElementName)
        {
            string strBalList = GetXmlElementBlock(xml, string.Format("Transaction.{0}", xmlElementName));

            // step 2: remove unwanted elements in the content
            strBalList = strBalList.Replace(string.Format("<{0}>", xmlElementName), string.Empty);
            strBalList = strBalList.Replace(string.Format("</{0}>", xmlElementName), string.Empty);

            // step 3: split the transactions list into individual transactions (still in xml string though)
            string[] del = new string[1];
            del[0] = "</Balance>";
            string[] list = strBalList.Split(del, StringSplitOptions.None);

            List<Balance> balances = new List<Balance>();
            string currentXmlTrans = string.Empty;
            object prop_value = null;
            foreach (string item in list)
            {
                if (item.Contains("Balance"))
                {
                    currentXmlTrans = string.Format("<StatementData> <Response> {0} </Balance> </Response>  </StatementData>", item);
                    Balance tran = new Balance();
                    PropertyInfo[] properties = typeof(Balance).GetProperties();
                    foreach (PropertyInfo property in properties)
                    {
                        try
                        {
                            prop_value = GetXmlElement(currentXmlTrans, string.Format("Balance.{0}", property.Name));
                            property.SetValue(tran, prop_value, null);

                        }
                        catch { }
                    }
                    balances.Add(tran);
                }
            }

            return balances;
        }
    }
}