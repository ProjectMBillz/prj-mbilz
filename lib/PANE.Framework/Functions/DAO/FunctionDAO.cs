using System;
using System.Collections.Generic;
using System.Text;
using CBC.Framework.DAO;
using CBC.Framework.Functions.DTO;
using NHibernate;
using NHibernate.Criterion;
using System.Linq;

namespace CBC.Framework.Functions.DAO
{
    public class FunctionDAO : CoreDAO<Function, long>
    {
        public static List<Function> RetrieveFunctions(UserCategory userCategory)
        {
            List<Function> result = new List<Function>();
            ISession session = BuildSession("");
            try
            {
                ICriteria criteria = session.CreateCriteria(typeof(Function));

                //criteria.Add(Expression.Eq("IsDefault", true));
                criteria.Add(Expression.Or(Expression.Eq("UserCategory", userCategory),
                                           Expression.IsNull("UserCategory")));
                criteria.AddOrder(Order.Asc("ParentFunction")).AddOrder(Order.Asc("Name"));
                result = criteria.List<Function>() as List<Function>;
            }
            catch
            {
                throw;
            }

            return result;
        }

        public static List<Function> RetrieveDefaultFunctions(UserCategory userCategory)
        {
            List<Function> result = new List<Function>();
            ISession session = BuildSession("");
            try
            {
                ICriteria criteria = session.CreateCriteria(typeof(Function));

                criteria.Add(Expression.Eq("IsDefault", true));
                criteria.Add(Expression.Or(Expression.Eq("UserCategory", userCategory),
                                           Expression.IsNull("UserCategory")));
                
                result = criteria.List<Function>() as List<Function>;
            }
            catch
            {
                throw;
            }

            return result;
        }


        //Parameter mfbCode not needed unlike the other classes.
        public static Function RetrieveByRoleName(string roleName, UserCategory userCategory)
        {
            Function result = new Function();
            ISession session = BuildSession("");
            try
            {
                result = session.CreateCriteria(typeof(Function))
                    .Add(Expression.Eq("RoleName", roleName))
                    .Add(Expression.Eq("UserCategory", userCategory))
                    .UniqueResult<Function>();
            }
            catch
            {
                throw;
            }

            return result;
        }

        /*
        public static Function RetrieveByRoleName(string mfbCode, string roleName)
        {
            Function result = new Function();
            ISession session = BuildSession(mfbCode);
            try
            {
                result = session.CreateCriteria(typeof(Function)).Add(Expression.Eq("RoleName", roleName)).UniqueResult<Function>();
            }
            catch
            {
                throw;
            } 

            return result;
        }
          */

        public static List<Function> GetFunctionUsingApplicationID(string mfbCode, long appID)
        {
            List<Function> results = new List<Function>();
            ISession session = BuildSession("");
            try
            {
                results = session.CreateCriteria(typeof(Function)).Add(Expression.Eq("ApplicationID", appID)).List<Function>().ToList();
            }
            catch
            {
                throw;
            }

            return results;
        }

        public static List<Function> RetrieveFunctionsForApprovalConfiguration(string mfbCode)
        {
            List<Function> functions = new List<Function>();
            ISession session = BuildSession("");
            try
            {
                IList<Function> allFunctions = session.CreateCriteria(typeof(Function)).Add(Expression.Eq("IsApprovable", true)).List<Function>() as List<Function>;

                List<Function> repFunctions = allFunctions.Where(theFunc => theFunc.HasSubRoles).ToList();
                functions = allFunctions.Where(theFunc => !theFunc.HasSubRoles).ToList();
                foreach (UserRole subRoles in UserRoleDAO.RetrieveAll(mfbCode))
                {
                    foreach (Function fun in repFunctions)
                    {
                        Function fc = new Function()
                        {
                            ID = fun.ID,
                            RoleName = fun.RoleName,
                            Description = fun.Description,
                            Name = string.Format("{0} [{1}]", fun.Name, subRoles.Name),
                            HasSubRoles = fun.HasSubRoles,
                            SubUserRoleUpdate = subRoles
                        };
                        fc.TheApprovalConfigurations = fun.TheApprovalConfigurations.Where(theApprov => theApprov.SubUserRole == subRoles).ToList();
                        functions.Add(fc);
                    }

                }
            }
            catch
            {
                throw;
            }

            return functions;
        }

        public static IEnumerable<Function> RetrieveFunctionsForUserRole(string mfbCode)
        {
            ISession session = BuildSession("");
            return session.CreateCriteria(typeof(Function)).AddOrder(Order.Asc("ParentFunction")).List<Function>();

        }

        public static List<Function> AllFunctions(string mfbCode)
        {
            ISession session = BuildSession("");
            return session.CreateCriteria(typeof(Function)).AddOrder(Order.Asc("ParentFunction")).List<Function>().ToList();

        }

        public static Function GetFunctionByID(string mfbCode, long id) {

            return Retrieve("", id);
        }


        public static List<Function> GetByIDsAndUserCategory(string mfbCode, long[] ids, UserCategory userCategory)
        {
            List<Function> result = new List<Function>();

            ISession session = BuildSession("");
            try
            {
                ICriteria criteria = session.CreateCriteria(typeof(Function));
                criteria.Add(Expression.In("ID", ids));
                criteria.Add(Expression.Eq("UserCategory", userCategory));

                result = criteria.List<Function>() as List<Function>;
            }
            catch
            {
                throw;
            }

            return result;
        }

        //public static List<Function> RetrieveFunctionsForApprovalViaRoleName(string makerRolerName)
        //{
        //    List<Function> functions = new List<Function>();
        //    ISession session = BuildSession();
        //    try
        //    {
        //        IList<Function> allFunctions = session.CreateCriteria(typeof(Function)).Add(Expression.Eq("IsApprovable", true))
        //            .CreateCriteria("TheApprovalConfigurations").CreateCriteria("ApprovingRole")
        //            .Add(Expression.Eq("Name", makerRolerName)).List<Function>() as List<Function>;

        //        List<Function> repFunctions = allFunctions.Where(theFunc => theFunc.HasSubRoles).ToList();
        //        functions = allFunctions.Where(theFunc => !theFunc.HasSubRoles).ToList();
        //        foreach (UserRole subRoles in UserRoleDAO.RetrieveAll())
        //        {
        //            foreach (Function fun in repFunctions)
        //            {
        //                Function fc = new Function()
        //                {
        //                    ID = fun.ID,
        //                    RoleName = fun.RoleName,
        //                    Description = fun.Description,
        //                    Name = string.Format("{0} [{1}]", fun.Name, subRoles.Name),
        //                    HasSubRoles = fun.HasSubRoles,
        //                    SubUserRoleUpdate = subRoles
        //                };
        //                fc.TheApprovalConfigurations = fun.TheApprovalConfigurations.Where(theApprov => theApprov.SubUserRole == subRoles).ToList();
        //                functions.Add(fc);
        //            }

        //        }
        //    }
        //    catch
        //    {
        //        throw;
        //    }

        //    return functions;
        //}
    }

}