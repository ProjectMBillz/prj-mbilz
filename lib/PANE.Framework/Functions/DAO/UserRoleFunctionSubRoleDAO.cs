using System;
using System.Collections.Generic;
using System.Text;
using CBC.Framework.DAO;
using CBC.Framework.Functions.DTO;
using NHibernate;
using NHibernate.Criterion;

namespace CBC.Framework.Functions.DAO
{
    public class UserRoleFunctionSubRoleDAO : CoreDAO<UserRoleFunctionSubRole, long>
    {
        public static IList<UserRoleFunctionSubRole> RetrieveByUserRole(string mfbCode,long roleId)
        {
            IList<UserRoleFunctionSubRole> result = new List<UserRoleFunctionSubRole>();
            ISession session = BuildSession(mfbCode);
            try
            {
                result = session.CreateCriteria(typeof(UserRoleFunctionSubRole)).CreateCriteria("TheUserRoleFunction").Add(Expression.Eq("TheUserRoleID", roleId)).List<UserRoleFunctionSubRole>();
            }
            catch
            {
                throw;
            }

            return result;
        }

        public static IList<UserRoleFunctionSubRole> RetrieveByUserRoleFunction(string mfbCode, long theUserRoleFunctionId)
        {
            IList<UserRoleFunctionSubRole> result = new List<UserRoleFunctionSubRole>();
            ISession session = BuildSession(mfbCode);
            try
            {
                result = session.CreateCriteria(typeof(UserRoleFunctionSubRole)).Add(Expression.Eq("TheUserRoleFunctionID", theUserRoleFunctionId)).List<UserRoleFunctionSubRole>();
            }
            catch
            {
                throw;
            }

            return result;
        } 
    }
}
