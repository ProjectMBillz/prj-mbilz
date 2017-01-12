using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace CBC.Framework.Utility
{

    /// <summary>
    /// Returns the Current Users' Institution Code for use as a <see cref="Parameter"/>.
    /// </summary>
    public class InstitutionCodeParameter : Parameter
    {
        private const string SS_MFBCODE = "::SS_INSTITUTION_CODE::";

        //protected override object Evaluate(HttpContext context, System.Web.UI.Control control)
        //{
        //    if (HttpContext.Current.Session[SS_MFBCODE] == null)
        //    {
        //        FunctionsMembershipUser user = System.Web.Security.Membership.GetUser() as FunctionsMembershipUser;
        //        if (user != null)
        //        {
        //            HttpContext.Current.Session[SS_MFBCODE] = user.UserName.Split(':')[1];
        //        }
        //        else
        //        {
        //            return "";
        //        }
        //    }
        //    return Convert.ToString(HttpContext.Current.Session[SS_MFBCODE]);
        //}
        protected override object Evaluate(HttpContext context, System.Web.UI.Control control)
        {
            if (HttpContext.Current != null && HttpContext.Current.User != null &&  HttpContext.Current.User.Identity != null)
            {
                HttpContext.Current.Session[SS_MFBCODE] = HttpContext.Current.User.Identity.Name.Split(':')[1];
            }

            return Convert.ToString(HttpContext.Current.Session[SS_MFBCODE]);
        }


    } 
}
