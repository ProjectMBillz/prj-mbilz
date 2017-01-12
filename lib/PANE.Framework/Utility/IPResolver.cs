using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Web;

namespace CBC.Framework.Utility
{
    public class IPResolver
    {
        public static string GetIP4Address(bool returnIP6IfIP4DoesntExist)
        {
            string IP4Address = String.Empty;
            if (HttpContext.Current != null)
            {
                foreach (IPAddress IPA in Dns.GetHostAddresses(HttpContext.Current.Request.UserHostAddress))
                {
                    if (IPA.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        IP4Address = IPA.ToString();
                        break;
                    }
                }

                if (IP4Address != String.Empty)
                {
                    return IP4Address;
                }

                foreach (IPAddress IPA in Dns.GetHostAddresses(Dns.GetHostName()))
                {
                    if (IPA.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        IP4Address = IPA.ToString();
                        break;
                    }
                }

                if (returnIP6IfIP4DoesntExist && String.IsNullOrEmpty(IP4Address))
                {
                    return HttpContext.Current.Request.UserHostAddress;
                }
            }

            return IP4Address;
        }

    }
}
