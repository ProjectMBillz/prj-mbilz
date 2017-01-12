using System;
using System.Collections.Generic;
using System.Text;
using CBC.Framework.DAO;
using CBC.Framework.Functions.DTO;
using NHibernate;
using NHibernate.Criterion;

namespace CBC.Framework.Functions.DAO
{
    public class UserRoleFunctionDAO : CoreDAO<UserRoleFunction, long>
    {
        public static IList<UserRoleFunction> RetrieveByUserRole(string mfbCode, UserRole role)
        {
            IList<UserRoleFunction> result = new List<UserRoleFunction>();
            ISession session = BuildSession(mfbCode);
            try
            {
                result = session.CreateCriteria(typeof(UserRoleFunction))
                    .Add(Expression.Eq("TheUserRoleID", role.ID))
                    .List<UserRoleFunction>();
            }
            catch
            {
                throw;
            }

            return result;
        }

        public static IList<UserRoleFunction> RetrieveByFunctionItemIDAndRoleID(string mfbCode, long functionID, long roleID)
        {
            IList<UserRoleFunction> result = new List<UserRoleFunction>();
            ISession session = BuildSession(mfbCode);
            try
            {
                result = session.CreateCriteria(typeof(UserRoleFunction))
                    .Add(Expression.Eq("TheUserRoleID", roleID))
                    .Add(Expression.Eq("TheFunctionID", functionID))
                    .List<UserRoleFunction>();
            }
            catch
            {
                throw;
            }

            return result;
        }


    }
}
