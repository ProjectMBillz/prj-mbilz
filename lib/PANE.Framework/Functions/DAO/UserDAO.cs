using System;
using System.Web;
using System.Collections.Generic;
using System.Text;
using CBC.Framework.DAO;
using CBC.Framework.Functions.DTO;
using NHibernate;
using NHibernate.Criterion;
using System.Collections;

namespace CBC.Framework.Functions.DAO
{
    public class UserDAO : CoreDAO<IUser, long>
    {

        public static IUser RetrieveByUsername(string mfbCode, string username)
        {
            IUser result = null;
            ISession session = BuildSession(mfbCode);

            try
            {
                result = session.CreateCriteria(typeof(IUser)).Add(Expression.Eq("UserName", username)).UniqueResult<IUser>();
            }
            catch
            {
                throw;
            }
            return result;
        }

       
        public static IUser AuthenticateUser(string mfbCode, string username, string password)
        {
            IUser result = null;
            ISession session = BuildSession(mfbCode);
            try
            {
                result = session.CreateCriteria(typeof(IUser)).Add(Expression.Eq("Password", password)).Add(Expression.Eq("UserName", username)).UniqueResult<IUser>();
            }
            catch
            {
                throw;
            }
            return result;
        }


        public static List<IUser> RetrieveAll(string username, string firstName, string lastname, string branchcode, UserStatus? status, bool isSuperUser, long roleID, string sort, string direction, int page, int pageSize, out int ItemsCount)
        {
            List<IUser> result = new List<IUser>();
            
            ISession session = BuildSession("");
            try
            {
                ICriteria criteria = session.CreateCriteria(typeof(IUser));

                //Order in Ascending order of Name
                if (!String.IsNullOrEmpty(firstName) && !String.IsNullOrEmpty(firstName.Trim()))
                {
                    criteria.Add(Expression.Like("FIRSTNAME", firstName.Trim(), MatchMode.Anywhere));
                }

                if (!String.IsNullOrEmpty(lastname) && !String.IsNullOrEmpty(lastname.Trim()))
                {
                    criteria.Add(Expression.Like("LASTNAME", lastname.Trim(), MatchMode.Anywhere));
                }
                if (roleID > 0)
                {
                    criteria.Add(Expression.Eq("ROLE", roleID));
                }
                if (!String.IsNullOrEmpty(branchcode) && !String.IsNullOrEmpty(branchcode.Trim()))
                {
                    criteria.Add(Expression.Like("BRANCHCODE", lastname.Trim(), MatchMode.Anywhere));
                }
                if (isSuperUser)
                {
                    criteria.Add(Expression.Eq("ISSUPERUSER", isSuperUser));
                }
                if (status.HasValue)
                {
                    criteria.Add(Expression.Eq("Status", status.Value));
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
                    result.Add((IUser)o);
                }

                ItemsCount = Convert.ToInt32((long)((IList)resultCount)[0]);
            }
            catch
            {
                throw;
            }

            return result;
        }
    }



}
