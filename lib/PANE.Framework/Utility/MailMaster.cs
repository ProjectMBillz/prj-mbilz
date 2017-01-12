using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.IO;

namespace CBC.Framework.Utility
{
   public class MailMaster
    {
        public bool SendEmail(string toemail,string body)
        {

            try
            {
                MailMessage msg = new MailMessage();

                if (!String.IsNullOrEmpty(body) && !String.IsNullOrEmpty(body.Trim()))
                    msg.Body = body;
                if (!String.IsNullOrEmpty(toemail) && !String.IsNullOrEmpty(toemail.Trim()))
                    msg.To.Add(toemail);

                SmtpClient client = new SmtpClient();
                client.Send(msg);
            }
            catch
            {
                throw;
               
            }
            return true; 
        }


        public static string ConstructMailBody(string path, Dictionary<string, string> keyValuePairs)
        {
            string mailBody = File.ReadAllText(path);
            foreach (KeyValuePair<string, string> key in keyValuePairs)
            {
                mailBody = mailBody.Replace(string.Format("<%{0}%>", key.Key), key.Value);
            }
            return mailBody;
            
        }

        public bool SendEmail(string path,Dictionary<string,string> KeyValuePairs,string Toemail)
        {  
            try
            {
               //string mailbody = File.ReadAllText(path);
               // foreach (KeyValuePair<string, string> key in KeyValuePairs)
               // {
               //     mailbody.Replace(string.Format("<%", key.Key, "%>"), key.Value);      
               // }
                string mailbody = ConstructMailBody(path, KeyValuePairs);
                SendEmail(Toemail, mailbody);   
               
            }
            catch
            {
                throw; 
            }
            return true; 
        }

        public bool SendEmail(MailMessage msg)
        {

            try
            {
                if (msg != null)
                {
                    SmtpClient client = new SmtpClient();
                    client.Send(msg);
                }
                else
                    return false; 
            }
            catch
            {
               throw;

            }
            return true;
        }
    }
}
