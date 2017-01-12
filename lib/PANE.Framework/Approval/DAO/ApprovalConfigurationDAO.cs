using System;
using System.Collections.Generic;
using System.Text;
using CBC.Framework.DAO;
using CBC.Framework.Approval.DTO;
using CBC.Framework.Functions.DTO;
using NHibernate;
using NHibernate.Criterion;

namespace CBC.Framework.Approval.DAO
{
    public class ApprovalConfigurationDAO : CoreDAO<ApprovalConfiguration, long>
    {
        public static List<ApprovalConfiguration> RetrieveByApprovableUserRole(string mfbCode,UserRole userRole)
        {
            IList<ApprovalConfiguration> result = new List<ApprovalConfiguration>();
            ISession session = BuildSession(mfbCode);
            try
            {
                result = session.CreateCriteria(typeof(ApprovalConfiguration)).Add(Expression.Eq("IsApprovable", true)).Add(Expression.Eq("ApprovingRole.ID", userRole.ID)).List<ApprovalConfiguration>();
            }
            catch
            {
                throw;
            }

            return result as List<ApprovalConfiguration>;
        }


        public static ApprovalConfiguration RetrieveByMakerRoleName(string mfbCode, string roleName, long? subUserRoleID)
        {
            ApprovalConfiguration apprConfig = new ApprovalConfiguration();
            ISession session = BuildSession(mfbCode);
            try
            {
                ICriteria criteria = session.CreateCriteria(typeof(ApprovalConfiguration));
                criteria.CreateCriteria("MakerRole").Add(Expression.Like("RoleName", roleName, MatchMode.Anywhere));
                if (subUserRoleID != null)
                {
                    criteria.Add(Expression.Eq("SubUserRole.ID", subUserRoleID));
                }
                else
                {
                    criteria.Add(Expression.IsNull("SubUserRole"));
                }

                apprConfig = criteria.UniqueResult<ApprovalConfiguration>();

            }
            catch
            {
                throw;
            }
            return apprConfig;
        }


    }
}
