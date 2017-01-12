using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using CBC.Framework.Functions.DTO;

/// <summary>
/// Summary description for PortalUsers
/// </summary>
public class FunctionsMembershipUser : MembershipUser
{   
    public FunctionsMembershipUser(MembershipUser mu)
        : base(mu.ProviderName, mu.UserName, mu.ProviderUserKey, mu.Email, mu.PasswordQuestion,
            mu.Comment, mu.IsApproved, mu.IsLockedOut, mu.CreationDate, mu.LastLoginDate, mu.LastActivityDate,
            mu.LastPasswordChangedDate, mu.LastLockoutDate)
    {
    }
    public FunctionsMembershipUser(string providerName, string userName, object providerUserKey, string email, string passwordQuestion, string comment,
        bool isApproved, DateTime creationDate, DateTime lastLoginDate, DateTime lastActivityDate, DateTime lastLockoutDate)
        : base(providerName, userName, providerUserKey, email, passwordQuestion, comment,
        isApproved, false, creationDate, lastLoginDate, lastActivityDate, DateTime.Now, lastLockoutDate)
    {
        // This calls a constructor of MembershipUser automatically because of the base reference above
    }



    public IUser UserDetails
    {
        get; set;
    }
}
