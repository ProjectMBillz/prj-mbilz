using System;
using System.Collections.Generic;
using System.Text;
using CBC.Framework.DAO;
using CBC.Framework.Functions.DTO;
using NHibernate;
using NHibernate.Criterion;
using System.Collections;

namespace CBC.Framework.Functions.DAO
{
    public class UserRoleDAO : CoreDAO<UserRole, long>
    {
        public static List<UserRole> GetByIDs(string mfbCode, long[] userRoleIDs)
        {
            List<UserRole> result = new List<UserRole>();
            ISession session = BuildSession(mfbCode);

            try
            {
                ICriteria criteria = session.CreateCriteria(typeof(UserRole));
                criteria.Add(Expression.In("ID", userRoleIDs));

                result = criteria.List<UserRole>() as List<UserRole>;
            }
            catch
            {
                throw;
            }
            return result;
        }

        public static UserRole GetUserRoleByName(string mfbCode, string name)
        {
            UserRole userRole = new UserRole();
            ISession session = BuildSession(mfbCode);
            try
            {
                userRole = session.CreateCriteria(typeof(UserRole)).Add(Expression.Eq("Name", name)).UniqueResult<UserRole>();
            }
            catch
            {
                throw;
            }
            return userRole;
        }

        public static UserRole GetUserRoleById(string mfbCode,long Id)
        {
            return Retrieve(mfbCode, Id);
        
        }

        public static List<UserRole> SearchBy(string mfbCode,string name, UserRoleScope? scope, Status? status)
        {
            
            List<UserRole> userRoles = new List<UserRole>();
            ISession session = BuildSession(mfbCode);
            try
            {
                ICriteria criteria = session.CreateCriteria(typeof(UserRole));
                criteria.AddOrder(Order.Asc("Name"));
                if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(name.Trim()))
                {
                    criteria.Add(Expression.Like("Name", name.Trim(), MatchMode.Anywhere));
                }
                if (scope != null)
                {
                    criteria.Add(Expression.Eq("Scope", scope));
                }
                if (status != null)
                {
                    criteria.Add(Expression.Eq("Status", status));
                }
                userRoles = criteria.List<UserRole>() as List<UserRole>;
            }
            catch
            {
                throw;
            }
            return userRoles;


        }

        public static List<UserRole> Find(string mfbCode, string name, string code, UserRoleScope? scope, Status? status, int page, int pageSize, string sort, string direction, out int totalCount)
        {
          
            List<UserRole> result = new List<UserRole>();
            ISession session = BuildSession(mfbCode);
            try
            {
                ICriteria criteria = session.CreateCriteria(typeof(UserRole));

                //Order in Ascending order of Name
                if (!String.IsNullOrEmpty(name) && !String.IsNullOrEmpty(name.Trim()))
                {
                    criteria.Add(Expression.Like("Name", name.Trim(), MatchMode.Anywhere));
                }

                if (!String.IsNullOrEmpty(code) && !String.IsNullOrEmpty(code.Trim()))
                {
                    criteria.Add(Expression.Like("Code", name.Trim(), MatchMode.Anywhere));
                }

                if (status.HasValue)
                {
                    criteria.Add(Expression.Eq("Status", status.Value));
                }
                if (scope.HasValue)
                {
                    criteria.Add(Expression.Eq("Scope", scope.Value));
                }

                //This part is for the paging.
                //Bugzz. Wont work over here.
                //criteria.SetFirstResult(page * pageSize).SetMaxResults(pageSize);


                //Before doing the sorting, i get a count Criteria so that it doesnt crash.
                ICriteria countCriteria = CriteriaTransformer.Clone(criteria).SetProjection(Projections.RowCountInt64());
                ICriteria listCriteria = CriteriaTransformer.Clone(criteria).SetFirstResult(page * pageSize).SetMaxResults(pageSize);


                //This section then performs the sort operations on the list. Sorting defaults to the Name column
                if (direction == "Default")
                {
                    listCriteria.AddOrder(Order.Desc("ID"));
                }
                else
                {
                    if (direction == "DESC")
                    {
                        listCriteria.AddOrder(Order.Desc(sort));
                    }
                    else
                    {
                        listCriteria.AddOrder(Order.Asc(sort));
                    }
                }
                //Add the two criteria to the session and retrieve their result.
                //IList allResults = session.CreateMultiCriteria().Add(listCriteria).Add(countCriteria).List();

                //Get the results from my criteria
                IList resultCount = countCriteria.List();


                //Get the Count
                IList allResults = listCriteria.List();
                foreach (var o in (IList)allResults)
                {
                    result.Add((UserRole)o);
                }

                totalCount = Convert.ToInt32((long)((IList)resultCount)[0]);
            }
            catch
            {
                throw;
            }

             
            return result;

        }

        public static List<UserRole> RetrieveAll(string name, Status? status, int page, int pageSize, string sort, string dir, out int ItemsCount, bool isStatic = true)
        {
            return Find(null, name, null, null, status, page, pageSize, sort, dir, out ItemsCount);
        }
        public List<UserRole> RetrieveAll(string name, Status? status, int page, int pageSize, string sort, string dir, out int totalCount)
        {
            return Find(null, name, null, null, status, page, pageSize, sort, dir, out totalCount);
        }
        public List<UserRole> ListAll()
        {
            return RetrieveAll();
        }
        public static List<UserRole> RetrieveAll(string sort, string direction, int page, int pageSize, out int ItemsCount)
        {
            return Find(null, null, null, null, null, page, pageSize, sort, direction, out ItemsCount);
        }
    }
}
