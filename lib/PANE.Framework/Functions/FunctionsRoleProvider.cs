using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text;
using System.Collections;
using CBC.Framework.DTO;
using CBC.Framework.Approval.DTO;
using CBC.Framework.Functions.DAO;
using CBC.Framework.Functions.DTO;
using System.Collections.Generic;
using CBC.Framework.Approval.DAO;
using System.Linq;

namespace CBC.Framework.Functions
{
    /// <summary>
    /// Summary description for FunctionsRoleProvider
    /// </summary>
    public class FunctionsRoleProvider : RoleProvider
    {
        public const string MFBCode = "::SS_MFBCODE::";
        public FunctionsRoleProvider()
        {

        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override string ApplicationName
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        public override void CreateRole(string roleName)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override string[] GetAllRoles()
        {
            string mfbCode = "";
            if (HttpContext.Current.Session[FunctionsRoleProvider.MFBCode] != null)
            {
                mfbCode = HttpContext.Current.Session[FunctionsRoleProvider.MFBCode].ToString();
            }
            else
            {
                return new string[] { };
            }
            List<string> allRoles = new List<string>();
            foreach (Function theFunction in FunctionDAO.RetrieveAll(mfbCode))
            {
                allRoles.Add(theFunction.RoleName);
            }
            return allRoles.ToArray();
        }

        public override string[] GetRolesForUser(string username)
        {
            string[] usernames = username.Split(':');
            string mfbCode = usernames[1];
            string impersonateCode = mfbCode;
            if (usernames.Length > 3 && usernames[3] == "*")
            {
                mfbCode = "";
            }
            List<string> userRoles = new List<string>();
            FunctionsMembershipUser theUser = Membership.GetUser(username) as FunctionsMembershipUser;
            if (theUser != null)
            {
                IList<UserRoleFunction> userRoleFuncs = UserRoleFunctionDAO.RetrieveByUserRole(mfbCode, theUser.UserDetails.Role);
                if (userRoleFuncs != null && userRoleFuncs.Count > 0)
                {
                    //Retrieve all the functions for that particular User Category.
                    List<Function> functionsForCurrentUserRole = FunctionDAO.GetByIDsAndUserCategory(mfbCode,
                                    userRoleFuncs.Select(urf => urf.TheFunctionID).ToArray(), theUser.UserDetails.Role.UserCategory);

                    if (functionsForCurrentUserRole != null && functionsForCurrentUserRole.Count > 0)
                    {
                        //Get the functions Role Names
                        userRoles.AddRange(functionsForCurrentUserRole.Select(f => f.RoleName));
                    }
                }

                //If its the first user(the super user) in the system, then give the user Configure Approval
                //and Manage User Role rights. 
                if (theUser.UserDetails.IsSuperUser)
                {
                    userRoles.AddRange(new string[] { "ConfigureApprovals", "ManageUserRoles" });
                }


                ////TODO: Framework Check out this part.
                //List<ApprovalConfiguration> list = ApprovalConfigurationDAO.RetrieveByApprovableUserRole(mfbCode, theUser.UserDetails.Role);
                //foreach (ApprovalConfiguration app in list)
                //{
                //    Function f =  FunctionDAO.GetFunctionByID(mfbCode, app.MakerRole);
                //    if (f != null)
                //    {
                //        userRoles.Add(string.Format("Approve{0}", f.RoleName));
                //    }
                //}
                //if (list.Count > 0)
                //{
                //    userRoles.Add("Approvals");
                //}

            }
         
            return userRoles.ToArray();
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            try
            {
                string[] usernames = username.Split(':');
                string mfbCode = usernames[1];
                string impersonateCode = mfbCode;
                if (usernames.Length > 3 && usernames[3] == "*")
                {
                    mfbCode = "";
                }
                List<string> userRoles = new List<string>();
                FunctionsMembershipUser theUser = Membership.GetUser(username) as FunctionsMembershipUser;
                if (theUser != null)
                {

                    Function function = FunctionDAO.RetrieveByRoleName(roleName, theUser.UserDetails.Role.UserCategory);
                    if (function == null) return false;

                    IList<UserRoleFunction> userRoleFuncs = UserRoleFunctionDAO.RetrieveByFunctionItemIDAndRoleID(mfbCode, function.ID, theUser.UserDetails.Role.ID);
                    if (userRoleFuncs != null && userRoleFuncs.Count > 0)
                    {
                        return true;
                    }
                }
            }
            catch
            {
                //return false;
            }
            return false;
        }


        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override bool RoleExists(string roleName)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }

}
