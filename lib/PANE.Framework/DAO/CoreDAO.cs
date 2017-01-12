using System;
using System.Collections.Generic;
using NHibernate;
using System.Collections;
using CBC.Framework.NHibernateManager;
using NHibernate.Criterion;
using CBC.Framework.DTO;
using CBC.Framework.Functions.DTO;
using CBC.Framework.NHibernateManager.Configuration;
using CBC.Framework.AuditTrail.DTO;
using System.Web.Security;
using CBC.Framework.Utility;

namespace CBC.Framework.DAO
{
    public class CoreDAO<T, idT> where T : class, IDataObject 
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="CoreDAO&lt;T, idT&gt;"/> class.
        /// </summary>
        public CoreDAO()
        {

        }


        /// <summary>
        /// Merges the specified obj
        /// </summary>
        /// <param name="obj">The obj.</param>
        public static void Merge(T obj, DatabaseSource dbSource)
        {
            ISession session = BuildSession(obj.MFBCode, dbSource);
            ITransaction tran = BuildTransaction(session);
            try
            {
                if (System.Web.HttpContext.Current != null && !Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["IgnoreHttpContext"]))
                {
                    IAuditable auditable = obj as IAuditable;
                    if (auditable != null && auditable.AuditableUser == null)
                    {
                        try
                        {
                            FunctionsMembershipUser memUser = Membership.GetUser() as FunctionsMembershipUser;
                            if (memUser != null)
                            {
                                auditable.AuditableUser = memUser.UserDetails;
                            }
                        }
                        catch { }
                    }
                }
                session.Merge(obj);
            }
            catch (Exception ex)
            {
                tran.Rollback();
                throw ex;
            }
        }


        /// <summary>
        /// Merges the specified obj.
        /// </summary>
        /// <param name="obj">The obj.</param>
        public static void Merge(T obj)
        {
            Merge(obj, DatabaseSource.Local);
        }

        /// <summary>
        /// Updates the specified obj(From Local/Remote DB).
        /// </summary>
        /// <param name="obj">The obj.</param>
        public static void Update(T obj, DatabaseSource dbSource)
        {
            Update(obj, dbSource, false);

        }

        /// <summary>
        /// Updates the specified obj(From Local/Remote DB).
        /// </summary>
        /// <param name="obj">The obj.</param>
        public static void Update(T obj, DatabaseSource dbSource, bool skipCheck)
        {
            ISession session = BuildSession(obj.MFBCode, dbSource);
            ITransaction tran = BuildTransaction(session);
            try
            {
                if (!skipCheck)
                {
                    //if (!Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["IsAWindowsService"]))
                    if (System.Web.HttpContext.Current != null && !Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["IgnoreHttpContext"]))
                    {

                        IAuditable auditable = obj as IAuditable;
                        if (auditable != null && auditable.AuditableUser == null)
                        {
                            try
                            {
                                //FunctionsMembershipUser memUser = Membership.GetUser() as FunctionsMembershipUser;
                                IUser memUser = System.Web.HttpContext.Current.Session["::USER::"] as IUser;
                                if (memUser != null)
                                {
                                    auditable.AuditableUser = memUser;
                                }
                            }
                            catch { }
                        }
                    }
                }
                session.Update(obj);
            }
            catch (Exception ex)
            {
                tran.Rollback();
                throw ex;
            }
        }

        /// <summary>
        /// Updates the specified obj.
        /// </summary>
        /// <param name="obj">The obj.</param>
        public static void Update(T obj)
        {
            Update(obj, DatabaseSource.Local);
        }

        /// <summary>
        /// Updates the specified obj.
        /// </summary>
        /// <param name="obj">The obj.</param>
        public static void Update(T obj, bool skipCheck)
        {
            Update(obj, DatabaseSource.Local, skipCheck);
        }

        /// <summary>
        /// Saves the specified obj.
        /// </summary>
        /// <param name="obj">The obj.</param>
        public static void Save(T obj, DatabaseSource dbSource)
        {
            ISession session = BuildSession(obj.MFBCode, dbSource);
            ITransaction tran = BuildTransaction(session);
            try
            {
                if (System.Web.HttpContext.Current != null && !Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["IgnoreHttpContext"]))
                {
                    IAuditable auditable = obj as IAuditable;
                    if (auditable != null && auditable.AuditableUser == null)
                    {
                        try
                        {
                            //FunctionsMembershipUser memUser = Membership.GetUser() as FunctionsMembershipUser;
                            IUser memUser = System.Web.HttpContext.Current.Session["::USER::"] as IUser;
                            if (memUser != null)
                            {
                                auditable.AuditableUser = memUser;
                            }
                        }
                        catch { }
                    }
                }
                session.Save(obj);
            }
            catch (Exception ex)
            {
                tran.Rollback();
                throw ex;
            }
        }

        /// <summary>
        /// Saves the specified obj(From Local/Remote DB).
        /// </summary>
        /// <param name="obj">The obj.</param>
        public static void Save(T obj)
        {
            Save(obj, DatabaseSource.Local);
        }


        /// <summary>
        /// Evicts the specified obj(From Local/Remote DB).
        /// </summary>
        /// <param name="obj">The obj.</param>
        public static void Evict(T obj, DatabaseSource dbSource)
        {
            ISession session = BuildSession(obj.MFBCode, dbSource);
            try
            {
                session.Evict(obj);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Evicts the specified obj.
        /// </summary>
        /// <param name="obj">The obj.</param>
        public static void Evict(T obj)
        {
            Evict(obj, DatabaseSource.Local);
        }

        /// <summary>
        /// Deletes the specified obj(From Local/Remote DB).
        /// </summary>
        /// <param name="obj">The obj.</param>
        public static void Delete(T obj)
        {
            Delete(obj, DatabaseSource.Local);
        }

        /// <summary>
        /// Deletes the specified obj.
        /// </summary>
        /// <param name="obj">The obj.</param>
        public static void Delete(T obj, DatabaseSource dbSource)
        {
            ISession session = BuildSession(obj.MFBCode, dbSource);
            ITransaction tran = BuildTransaction(session);
            try
            {
                //if (!Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["IsAWindowsService"]))
                if (System.Web.HttpContext.Current != null && !Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["IgnoreHttpContext"]))
                {
                    IAuditable auditable = obj as IAuditable;
                    if (auditable != null && auditable.AuditableUser == null)
                    {
                        try
                        {
                            FunctionsMembershipUser memUser = Membership.GetUser() as FunctionsMembershipUser;
                            if (memUser != null)
                            {
                                auditable.AuditableUser = memUser.UserDetails;
                            }
                        }
                        catch { }
                    }
                }
                session.Delete(obj);
            }
            catch (Exception ex)
            {
                tran.Rollback();
                throw ex;
            }
        }


        /// <summary>
        /// Retrieves the specified id(From Local/Remote DB).
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public static T Retrieve(string mfbCode, idT id)
        {
            return Retrieve(mfbCode, id, DatabaseSource.Local);
        }


        /// <summary>
        /// Retrieves the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public static T Retrieve(string mfbCode, idT id, DatabaseSource dbSource)
        {
            T result = default(T);
            ISession session = BuildSession(mfbCode, dbSource);
            try
            {
                result = session.Get<T>(id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        public static List<T> RetrieveAll()
        {
            return RetrieveAll("");
        }
        /// <summary>
        /// Retrieves all.
        /// </summary>
        /// <returns></returns>
        public static List<T> RetrieveAll(string mfbCode)
        {
            return RetrieveAll(mfbCode, DatabaseSource.Local);
        }

        /// <summary>
        /// Retrieves all (From Local/Remote DB).
        /// </summary>
        /// <returns></returns>
        public static List<T> RetrieveAll(string mfbCode, DatabaseSource dbSource)
        {
            List<T> results = new List<T>();
            ISession session = BuildSession(mfbCode, dbSource);
            try
            {
                results = session.CreateCriteria(typeof(T)).List<T>() as List<T>;
            }
            catch
            {
                throw;
            }
            return results;
        }

        /// <summary>
        /// Retrieves all but this time it implements paging.
        /// </summary>
        /// <param name="page">The page you are currently on.</param>
        /// <param name="pageSize">The size of the page the Grid/Data source needs.</param>
        /// <returns>A subset of a list of data.</returns>
        public static List<T> RetrieveAll(string mfbCode, int page, int pageSize, out int totalItemsCount)
        {
            return RetrieveAll(mfbCode, page, pageSize, out totalItemsCount);
        }

        /// <summary>
        /// Retrieves all but this time it implements paging(From Local/Remote DB).
        /// </summary>
        /// <param name="page">The page you are currently on.</param>
        /// <param name="pageSize">The size of the page the Grid/Data source needs.</param>
        /// <returns>A subset of a list of data.</returns>
        public static List<T> RetrieveAll(string mfbCode, int page, int pageSize, out int totalItemsCount, DatabaseSource dbSource)
        {
            List<T> results = new List<T>();

            try
            {

                ISession s = BuildSession(mfbCode, dbSource);
                IList allResults = s.CreateMultiCriteria()
                                    .Add(s.CreateCriteria(typeof(T)).SetFirstResult(page * pageSize).SetMaxResults(pageSize))
                                    .Add(s.CreateCriteria(typeof(T)).SetProjection(Projections.RowCountInt64()))
                                    .List();

                foreach (var o in (IList)allResults[0])
                {
                    results.Add((T)o);
                }
                totalItemsCount = Convert.ToInt32((long)((IList)allResults[1])[0]);
            }
            catch
            {
                throw;
            }
            return results;
        }

        /// <summary>
        /// Retrieves all.
        /// </summary>
        /// <param name="withDeleted">if set to <c>true</c> [with deleted].</param>
        /// <returns></returns>
        public static List<T> RetrieveAll(string mfbCode, bool withDeleted)
        {
            return RetrieveAll(mfbCode, withDeleted, DatabaseSource.Local);
        }


        /// <summary>
        /// Retrieves all(From Local/Remote DB).
        /// </summary>
        /// <param name="withDeleted">if set to <c>true</c> [with deleted].</param>
        /// <returns></returns>
        public static List<T> RetrieveAll(string mfbCode, bool withDeleted, DatabaseSource dbSource)
        {
            List<T> results = new List<T>();
            ISession session = BuildSession(mfbCode, dbSource);
            try
            {
                if (withDeleted)
                {
                    results = session.CreateCriteria(typeof(T)).List<T>() as List<T>;
                }
                else
                {
                    results = session.CreateCriteria(typeof(T)).Add(Expression.Eq("IsDeleted", false)).List<T>() as List<T>;
                }
            }
            catch
            {
                throw;
            }

            return results;
        }

        /// <summary>
        /// Builds the session.
        /// </summary>
        /// <returns></returns>
        public static ISession BuildSession(string mfbCode, DatabaseSource dbSource)
        {
            
            return NHibernateSessionManager.Instance.GetSession(mfbCode, dbSource);
        }

        public static ISession BuildSession(string mfbCode)
        {
            return BuildSession(mfbCode, DatabaseSource.Local);
        }


        /// <summary>
        /// Builds the transaction.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <returns></returns>
        protected static ITransaction BuildTransaction(ISession session)
        {
            if (session.Transaction == null || !session.Transaction.IsActive)
            {
                return session.BeginTransaction();
            }
            return session.Transaction;
        }
        /// <summary>
        /// Commits the changes.
        /// </summary>
        public static void CommitChanges(string mfbCode)
        {
            CommitChanges(mfbCode, DatabaseSource.Local);
        }

        /// <summary>
        /// Commits the changes(From Local/Remote DB).
        /// </summary>
        public static void CommitChanges(string mfbCode, DatabaseSource dbSource)
        {
            ISession session = BuildSession(mfbCode, dbSource);
            if (session.Transaction != null && session.Transaction.IsActive)
            {
                try
                {
                    session.Transaction.Commit();
                }
                catch(Exception e)
                {
                    throw;
                }
            }
        }
        /// <summary>
        /// Rollbacks the changes.
        /// </summary>
        public static void RollbackChanges(string mfbCode)
        {
            RollbackChanges(mfbCode, DatabaseSource.Local);
        }

        /// <summary>
        /// Rollbacks the changes(From Local/Remote DB).
        /// </summary>
        public static void RollbackChanges(string mfbCode, DatabaseSource dbSource)
        {
            ISession session = BuildSession(mfbCode, dbSource);
            if (session.Transaction != null && session.Transaction.IsActive)
            {
                session.Transaction.Rollback();
            }
        }

        /// <summary>
        /// Clears the current session.
        /// </summary>
        public static void ClearCurrentSession(string mfbCode)
        {
            ClearCurrentSession(mfbCode, DatabaseSource.Local);
        }

        /// <summary>
        /// Clears the current session(From Local/Remote DB).
        /// </summary>
        public static void ClearCurrentSession(string mfbCode, DatabaseSource dbSource)
        {
            BuildSession(mfbCode, dbSource).Clear();
        }


        /// <summary>
        /// Refresh the specified obj(From Local/Remote DB).
        /// </summary>
        /// <param name="obj">The obj to refresh.</param>
        public static void Refresh(T obj, DatabaseSource dbSource)
        {
            ISession session = BuildSession(obj.MFBCode, dbSource);
            ITransaction tran = BuildTransaction(session);
            try
            {
                session.Refresh(obj);
            }
            catch (Exception ex)
            {
                tran.Rollback();
                throw ex;
            }

        }

        /// <summary>
        /// Refresh the specified obj(From Local/Remote DB).
        /// </summary>
        /// <param name="obj">The obj to refresh.</param>
        public static void Refresh(T obj)
        {
            Refresh(obj, DatabaseSource.Local);
        }

        public static T Get(string mfbCode, long objID, DatabaseSource dbSource)
        {
            ISession session = BuildSession(mfbCode, dbSource);
            ITransaction tran = BuildTransaction(session);
            try
            {
                return session.Get<T>(objID);
            }
            catch (Exception ex)
            {
                tran.Rollback();
                throw ex;
            }
        }

        public static T Get(string mfbCode, long objID)
        {
            return Get(mfbCode, objID, DatabaseSource.Local);
        }
    }

}
