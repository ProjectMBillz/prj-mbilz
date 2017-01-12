using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;
using CBC.Framework.Utility;
using CBC.Framework.Functions.DAO;
using CBC.Framework.Functions.DTO;
using CBC.Framework.DTO;
using System.Web;

namespace CBC.Framework.Functions
{
    public class FunctionsMembershipProvider : MembershipProvider
    {
        private bool _isOnline;
        private string _Name;
        private int _MaxLoginAttempts = 3;
        private int _MaxOnlineUsers = -1;
        private int _MinPasswordLength = 0;
        private int _MinRequiredNonAlphanumericCharacters = 0;
        private string _ApplicationName;

        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            _Name = name;

            try
            {
                if (config["minPasswordLength"] != null)
                    _MinPasswordLength = Convert.ToInt32(config["minPasswordLength"]);
                if (config["applicationName"] != null)
                    _ApplicationName = config["applicationName"];
                if (config["maxOnlineUsers"] != null)
                    _MaxOnlineUsers = Convert.ToInt32(config["maxOnlineUsers"]);
                if (config["minRequiredNonAlphanumericCharacters"] != null)
                    _MinRequiredNonAlphanumericCharacters = Convert.ToInt32(config["minRequiredNonAlphanumericCharacters"]);
            }
            catch (Exception Ex)
            {
                throw new System.Configuration.ConfigurationErrorsException("There was an error reading the membership configuration settings", Ex);
            }
        }

        public override string ApplicationName
        {
            get
            {
                return _ApplicationName;
            }
            set
            {
                _ApplicationName = value;
            }
        }

        public override bool ChangePassword( string username, string oldPassword, string newPassword)
        {
            string[] usernames = username.Split(':');
            string institutionCode = usernames.Length > 1 ? usernames[1] : null;
           
            //string mfbCode = "";
            //if (HttpContext.Current.Session["::SS_MFBCODE::"] != null)
            //{
            //    mfbCode = HttpContext.Current.Session["::SS_MFBCODE::"].ToString();
            //}
            //else
            //{
            //    return false;
            //}

            bool isSuccessful = false;
            oldPassword = new MD5Password().CreateSecurePassword(oldPassword);
            IUser usr = UserDAO.RetrieveByUsername(institutionCode, usernames[0]);
            if (usr != null && usr.Password == oldPassword)
            {
                newPassword = new MD5Password().CreateSecurePassword(newPassword);
                if (usr.Password != newPassword)
                {
                    usr.Password = newPassword;
                    usr.MFBCode = institutionCode;
                    UserDAO.Update(usr,true);
                    UserDAO.CommitChanges(institutionCode);
                    isSuccessful = true;
                }
            }
            return isSuccessful;
        }

        public override string Name
        {
            get
            {
                return _Name;
            }
        }

        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {
            MembershipUser newUser = null;
            IUser userInfo = null;
            status = MembershipCreateStatus.UserRejected;
            try
            {
                userInfo.UserName = username;
                userInfo.Password = password;
               
                UserDAO.Save(userInfo);
                UserDAO.CommitChanges(userInfo.MFBCode);
            }
            catch (Exception Ex)
            {
                status = MembershipCreateStatus.ProviderError;
                newUser = null;
            }
            return newUser as MembershipUser;
        }

       
        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override bool EnablePasswordReset
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public override bool EnablePasswordRetrieval
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            //ArrayList allUsers = (ArrayList)new UserSystem().RetrieveAllUsers();
            //MembershipUserCollection col = new MembershipUserCollection();
            //foreach (User user in allUsers)
            //{
            //    try
            //    {
            //        PortalUsers xchangeUser = new PortalUsers(this.Name, user.UserName, user.ID, user.UserName, "", "", user.Status == CMS.Common.Utility.Status.Active, user.LastLoginDate, user.LastLoginDate, user.LastLoginDate, user.LastLoginDate);
            //        col.Add(xchangeUser as MembershipUser);
            //    }
            //    catch { }
            //}
            //totalRecords = col.Count;
            //return col;
            throw new Exception("The method or operation is not implemented."); 
       
        }

        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override int GetNumberOfUsersOnline()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override string GetPassword(string username, string answer)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public IUser ContextCurrentUser
        {
            get
            {
                return (IUser)HttpContext.Current.Items["::CURRENT_USER::"];
            }
            set
            {
                HttpContext.Current.Items["::CURRENT_USER::"] = value;
            }
        }

        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            string[] usernames = username.Split(':');
            string institutionCode = usernames.Length > 1 ? usernames[1] : null;
            string impersonateCode = institutionCode;
            if (usernames.Length > 3 && usernames[3] == "*")
            {
                institutionCode = "";
            }

            IUser user = null;
            if (HttpContext.Current.User.Identity.Name == username)
            {
                user = ContextCurrentUser;
                if (user == null)
                {
                    user = UserDAO.RetrieveByUsername(institutionCode, usernames[0]);
                    ContextCurrentUser = user;
                }
            }
            else
            {
                user = UserDAO.RetrieveByUsername(institutionCode, usernames[0]);
            }

            if (user != null)
            {
                string userKey = string.Format("{0}", user.UserName);
                if (institutionCode != null)
                {
                    userKey += string.Format(":{0}", impersonateCode);
                }
                FunctionsMembershipUser theUser = new FunctionsMembershipUser(this.Name, userKey, impersonateCode, user.UserName, "", "", user.Status == UserStatus.Active, user.CreationDate, user.LastLoginDate, user.LastLoginDate, user.CreationDate);
                theUser.UserDetails = user;
                return theUser as MembershipUser;
            }
            else
                return null;
        }

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            string[] userids = Convert.ToString(providerUserKey).Split(':');
            string institutionCode = userids.Length > 1 ? userids[1] : null;
            if (institutionCode == null) return null;
            IUser user = UserDAO.Retrieve(institutionCode, Convert.ToInt64(userids[0]));
            if (user != null)
            {
                string userKey = string.Format("{0}", user.ID);
                if (institutionCode != null)
                {
                    userKey += string.Format(":{0}", institutionCode);
                }

                FunctionsMembershipUser theUser = new FunctionsMembershipUser(this.Name, user.UserName, userKey, user.UserName, "", "", user.Status == UserStatus.Active, user.CreationDate, user.LastLoginDate, user.LastLoginDate, user.CreationDate);
                theUser.UserDetails = user;
                return theUser as MembershipUser;
            }
            else
                return null;
        }

        public override string GetUserNameByEmail(string email)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override int MaxInvalidPasswordAttempts
        {
            get { return _MaxLoginAttempts; }
        }

        public override int MinRequiredNonAlphanumericCharacters
        {
            get { return _MinRequiredNonAlphanumericCharacters; }
        }

        public override int MinRequiredPasswordLength
        {
            get { return _MinPasswordLength; }
        }

        public override int PasswordAttemptWindow
        {
            get
            {
                return 1;
            }
        }

        public override MembershipPasswordFormat PasswordFormat
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public override string PasswordStrengthRegularExpression
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public override bool RequiresQuestionAndAnswer
        {
            get
            {
                return true;
            }
        }

        public override bool RequiresUniqueEmail
        {
            get { return true; }
        }

        public override string ResetPassword(string username, string answer)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override bool UnlockUser(string userName)
        {
            string mfbCode = "";
            if (HttpContext.Current.Session["::SS_MFBCODE::"] != null)
            {
                mfbCode = HttpContext.Current.Session["::SS_MFBCODE::"].ToString();
            }
            else
            {
                return false;
            }
            IUser user = UserDAO.RetrieveByUsername(mfbCode, userName);

            if (user == null)
            {
                return false;
            }

            user.IsLockedOut = false;
            user.Status = UserStatus.Active;
            user.DoesntNeedAudit = true;
            UserDAO.Update(user,true);
            return true;

        }

        public override void UpdateUser(MembershipUser user)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override bool ValidateUser(string username, string password)
        {
            bool result = false;
            
            string[] usernames = username.Split(':');
            string institutionCode = usernames.Length > 1 ? usernames[1] : null;
            if (institutionCode == null) return false;
            password = new MD5Password().CreateSecurePassword(password);
            IUser user = null;
            try
            {
                //UserDAO.RetrieveByUsername(institutionCode, usernames[0]);
                user = UserDAO.AuthenticateUser(institutionCode, usernames[0], password);
            }
            catch (ArgumentNullException)
            {
                return false;
            }
            if (user != null)
            {
                if (user.Status != UserStatus.Active)  //Invalid User
                {
                    return false;
                }
                else
                {
                    user.FailedPasswordAttemptCount = 0; //Reset
                    result = true;
                }
            }
            else
            {
                user = UserDAO.RetrieveByUsername(institutionCode, usernames[0]);

                if (user == null) return false;

                user.FailedPasswordAttemptCount++;
                //Check whether user is locked out.
                user.IsLockedOut = this.IsLockedOut(user.FailedPasswordAttemptCount);
                if (user.IsLockedOut)
                {
                    user.Status = UserStatus.InActive;
                }
                result = false;
            }
            user.DoesntNeedAudit = true;
            user.MFBCode = institutionCode;
            UserDAO.Update(user,true);
            UserDAO.CommitChanges(institutionCode);

            if (HttpContext.Current != null)
            {
                FormsAuthentication.SetAuthCookie(username, false);
            }

            return result;
        }

        private bool IsLockedOut(int digit)
        {
            return digit >= this.MaxInvalidPasswordAttempts;
        }

        public int MaxOnlineUsers
        {
            get { return _MaxOnlineUsers; }
        }

    }
}
