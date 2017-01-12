using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using CBC.Framework.Utility;

namespace CBC.Framework.ISO8583.Client.Configuration
{
    public class ConfigurationManager
    {
        private static NameValueCollection ISO8583ClientConfig
        {
            get
            {
                return System.Configuration.ConfigurationManager.GetSection("ISO8583.Client") as NameValueCollection;
            }
        }

        public static string ISO8583ServerHostname
        {
            get
            {
                return ISO8583ClientConfig["ISO8583ServerHostname"];
            }
        }

        public static string TerminalID
        {
            get
            {
                return ISO8583ClientConfig["TerminalID"];
            }
        }

        public static string CardAcceptorID
        {
            get
            {
                return ISO8583ClientConfig["CardAcceptorID"];
            }
        }


        public static int ISO8583ServerPort
        {
            get
            {
                return Convert.ToInt32(ISO8583ClientConfig["ISO8583ServerPort"]);
            }
        }

        public static string ISO8583CardPAN
        {
            get
            {
                return ISO8583ClientConfig["ISO8583CardPAN"];
            }
        }

        public static string AcquiringInstitutionIDCode
        {
            get
            {
                return ISO8583ClientConfig["AcquirerCode"];
            }
        }

        public static string TMK
        {
            get
            {
                return ISO8583ClientConfig["TMK"];
            }
        }
        public static int  HsmHeaderLength
        {
            get
            {
                return Convert.ToInt32( ISO8583ClientConfig["HsmHeaderLength"]);
            }
        }
        public static string BankDetail
        {
            get
            {
                string bankID = "";
                if (ISO8583ClientConfig != null)
                {
                  bankID =  ISO8583ClientConfig["BANKID"];
                }
                string bankName = "";
                if (ISO8583ClientConfig != null)
                {
                    bankName = ISO8583ClientConfig["BRANCHID"];
                }

                if (!string.IsNullOrEmpty(bankID))
                {
                    bankID = bankID.PadRight(9, ' ').Substring(0, 9);
                }

                if (!string.IsNullOrEmpty(bankName))
                {
                    bankName = bankName.PadRight(21, ' ').Substring(0, 21);
                }
                return string.Format("{0}{1}", bankID, bankName);
            }
        }

    }
}
