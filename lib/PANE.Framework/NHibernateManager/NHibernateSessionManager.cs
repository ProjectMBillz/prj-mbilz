namespace CBC.Framework.NHibernateManager
{
    using NHibernate;
    using NHibernate.Cfg;
    using System.Runtime.Remoting.Messaging;
    using System.Web;
    using System.Collections.Generic;
    using CBC.Framework.NHibernateManager.Configuration;
    using System;
    using System.Linq;
    using CBC.Framework.DTO;
    using CBC.Framework.DAO;
    using CBC.Framework.Utility;
    using System.Reflection;
    using NHibernate.Connection;
    using System.Configuration;
    //using log4net;

    public sealed class NHibernateSessionManager
    {
        //For logging...
        //private static readonly ILog log = 
        //    LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private const string SESSION_KEY = "::NHIBERNATE_SESSION_KEY::";
        private Dictionary<string, ISessionFactory> sessionFactory;
        private DatabaseSource dbSource = DatabaseSource.Core;
        private NHibernateSessionManager()
        {
            this.InitSessionFactory();
        }

        private void CloseSession(string key)
        {
            if (this.ContextSession != null)
            {
                this.ContextSession.Remove(key);
                this.ContextSession.Remove(GetRemoteString(key));
            }
        }

        private void CloseSessionFactory(string key)
        {
            if (this.sessionFactory != null)
            {
                ISessionFactory tempSessionFactory = null;
                //Close the local
                if (this.sessionFactory.TryGetValue(key, out tempSessionFactory))
                {
                    try
                    {
                        tempSessionFactory.Close();
                    }
                    catch { }
                }

                //Then the remote
                if (this.sessionFactory.TryGetValue(GetRemoteString(key), out tempSessionFactory))
                {
                    try
                    {
                        tempSessionFactory.Close();
                    }
                    catch { }
                }

                this.sessionFactory.Remove(key);
                this.sessionFactory.Remove(GetRemoteString(key));
            }
        }


        public void EvictQueriesFromSessionFactoryRemote<T>(string key)
        {
            if (this.sessionFactory != null)
            {
                ISessionFactory tempSessionFactory = null;

                //Then the remote
                if (this.sessionFactory.TryGetValue(GetRemoteString(key), out tempSessionFactory))
                {
                    try
                    {
                        tempSessionFactory.Evict(typeof(T));
                    }
                    catch { }
                }
            }
        }

        public void EvictQueriesFromSessionFactory(string key)
        {
            if (this.sessionFactory != null)
            {
                ISessionFactory tempSessionFactory = null;
                //Close the local
                if (this.sessionFactory.TryGetValue(key, out tempSessionFactory))
                {
                    try
                    {
                        tempSessionFactory.EvictQueries();
                    }
                    catch { }
                }

                //Then the remote
                if (this.sessionFactory.TryGetValue(GetRemoteString(key), out tempSessionFactory))
                {
                    try
                    {
                        tempSessionFactory.EvictQueries();
                    }
                    catch { }
                }

            }
        }

        public void CloseSession(string key, DatabaseSource dbSource)
        {
            string oldKey = key;
            if (dbSource == DatabaseSource.Remote)
            {
                key = GetRemoteString(key);
            }
            else if (dbSource == DatabaseSource.Core)
            {
                key = "CORE_DB";
            }

            if (this.ContextSession != null)
            {
                ISession session = null;
                if (this.ContextSession.TryGetValue(key, out session) && session.IsOpen)
                {
                    session.Close();
                    this.ContextSession.Remove(key);
                }
            }
        }


        public void CloseSession()
        {
            if (this.ContextSession != null)
            {
                //Close the REMOTE Sessions first.
                foreach (KeyValuePair<string, ISession> keyValuePair in this.ContextSession)// ISession contextSession in this.ContextSession.OrderBy(s => s.Key.EndsWith("REMOTE"))..Values)
                {
                    if (keyValuePair.Key.EndsWith("REMOTE"))
                    {
                        if ((keyValuePair.Value != null) && keyValuePair.Value.IsOpen)
                        {
                            //keyValuePair.Value.Flush();
                            keyValuePair.Value.Close();
                        }
                    }
                }
                foreach (KeyValuePair<string, ISession> keyValuePair in this.ContextSession)// ISession contextSession in this.ContextSession.OrderBy(s => s.Key.EndsWith("REMOTE"))..Values)
                {
                    if (!keyValuePair.Key.EndsWith("REMOTE"))
                    {
                        if ((keyValuePair.Value != null) && keyValuePair.Value.IsOpen)
                        {
                            //keyValuePair.Value.Flush();
                            keyValuePair.Value.Close();
                        }
                    }
                }
            }
            this.ContextSession = new Dictionary<string, ISession>();
        }

        public void RollbackSession()
        {
            if (this.ContextSession != null)
            {
                //foreach (ISession contextSession in this.ContextSession.Values)
                //{
                //    if ((contextSession != null) && contextSession.IsOpen)
                //    {
                //        contextSession.Transaction.Rollback();
                //    }
                //}

                //Rollback the Remote sessions first
                foreach (KeyValuePair<string, ISession> keyValuePair in this.ContextSession)// ISession contextSession in this.ContextSession.OrderBy(s => s.Key.EndsWith("REMOTE"))..Values)
                {
                    if (keyValuePair.Key.EndsWith("REMOTE"))
                    {
                        if ((keyValuePair.Value != null) && keyValuePair.Value.IsOpen)
                        {
                            keyValuePair.Value.Transaction.Rollback();
                        }
                    }
                }
                foreach (KeyValuePair<string, ISession> keyValuePair in this.ContextSession)// ISession contextSession in this.ContextSession.OrderBy(s => s.Key.EndsWith("REMOTE"))..Values)
                {
                    if (!keyValuePair.Key.EndsWith("REMOTE"))
                    {
                        if ((keyValuePair.Value != null) && keyValuePair.Value.IsOpen)
                        {
                            keyValuePair.Value.Transaction.Rollback();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Kills an existing Session factory and session, if exists, for both
        /// Local and Remote Connections for an institution and recreates it again.
        /// </summary>
        /// <param name="key">The Institution key</param>
        public static void ResyncConnection(string key)
        {
            //Kill the connection.
            KillConnection(key);

            //Then reconnect.
            Instance.GetSession(key, DatabaseSource.Local);
            Instance.GetSession(key, DatabaseSource.Remote);
        }

        /// <summary>
        /// Kills an existing Session factory and session, if exists,for both
        /// Local and Remote Connections for an institution.
        /// </summary>
        /// <param name="key">The Institution key</param>
        public static void KillConnection(string key)
        {
            //Treat the Session first.
            Instance.CloseSession(key);

            //Then the Session Factory
            Instance.CloseSessionFactory(key);
        }

        public ISession GetSession(string key)
        {
            return this.GetSession(key, DatabaseSource.Local);
        }

        private string GetRemoteString(string key)
        {
            return string.Concat(key, "-REMOTE");
        }

        public ISessionFactory BuildSessionFactory()
        {
            string conString = ConfigurationManager.AppSettings["Sequel"].ToString();
            var cfg = new NHibernate.Cfg.Configuration()
                        .AddProperties(new Dictionary<string, string> 
                        {
                    {NHibernate.Cfg.Environment.ConnectionDriver, typeof (NHibernate.Driver.SqlClientDriver).FullName},
                    {NHibernate.Cfg.Environment.Dialect, typeof (NHibernate.Dialect.MsSql2005Dialect).FullName},
                    {NHibernate.Cfg.Environment.ConnectionProvider, typeof (DriverConnectionProvider).FullName},
                    {NHibernate.Cfg.Environment.ConnectionString, conString},
                })
                        .AddAssembly(Assembly.GetExecutingAssembly());
            return cfg.BuildSessionFactory();
        }
        public ISession GetSession(string key, DatabaseSource dbSource)
        {
            //this.dbSource = dbSource;
            ISession contextSession = null;
            try
            {
                if (key == null)
                {
                    key = "";
                }
                string oldKey = key;
                if (dbSource == DatabaseSource.Remote)
                {
                    key = GetRemoteString(key);
                }
                else if (dbSource == DatabaseSource.Core)
                {
                    key = "CORE_DB";
                }


                lock (this)
                {
                    if (this.ContextSession.TryGetValue(key, out contextSession))
                    {
                    }
                    else
                    {
                        ISessionFactory contextSessionFactory = null;// this.sessionFactory[key];

                        if (this.sessionFactory.TryGetValue(key, out contextSessionFactory))
                        {
                            //if (dbSource == DatabaseSource.Sequel)
                            //{
                            //    this.ContextSession = null;
                            //    contextSessionFactory = null;
                            //    //string conString = @"Data Source=JOORGANAUTIA\JOORGANAUTIA;Initial Catalog=RiskManagement;Persist Security Info=True;User ID=sa;Password=joorgofGod";
                            //    contextSessionFactory = BuildSessionFactory();
                            //}
                        }
                        else
                        {
                            try
                            {
                                NHibernate.Cfg.Configuration configuration = GetConfiguration(key, oldKey, dbSource);
                                contextSessionFactory = configuration.BuildSessionFactory();
                            }
                            catch (Exception ex)
                            {
                                //Log the error with log4net
                                //if (log.IsErrorEnabled)
                                //{
                                //    log.Error("Error Building Session Factory", ex);
                                //}
                            }
                            if (contextSessionFactory != null)
                            {
                                this.sessionFactory.Add(key, contextSessionFactory); //Add it to the existing ones
                            }
                        }

                        //We have a different interceptor here bcos when we're dealing with their remote DB,
                        //we still need to save their Audit Trail data in the Local DB, so anytime we find out
                        //that we are dealing with the remote DB, we just switch Session to the Local Session.
                        IInterceptor interceptor = null;
                        if (dbSource == DatabaseSource.Remote)
                        {
                            interceptor = new AuditTrail.AuditTrailInterceptor2(GetSession(oldKey, DatabaseSource.Local));
                        }
                        else
                        {
                            interceptor = new AuditTrail.AuditTrailInterceptor2();
                        }
                        contextSession = contextSessionFactory.OpenSession(interceptor);
                        this.ContextSession.Add(key, contextSession);
                    }
                }
            }
            catch
            {
                throw;
            }
            return contextSession;
        }

        public NHibernate.Cfg.Configuration GetConfiguration(string key, DatabaseSource dbSource)
        {
            string oldKey = key;
            if (dbSource == DatabaseSource.Remote)
            {
                key = GetRemoteString(key);
            }
            else if (dbSource == DatabaseSource.Core)
            {
                key = "CORE_DB";
            }

            return GetConfiguration(key, oldKey, dbSource);
        }

        private NHibernate.Cfg.Configuration GetConfiguration(string key, string oldKey, DatabaseSource dbSource)
        {
            NHibernate.Cfg.Configuration configuration = new NHibernate.Cfg.Configuration();
            if (dbSource == DatabaseSource.Sequel)
            {
                //string coreFilePath = System.Configuration.ConfigurationManager.AppSettings["SeNhibernateFile"];
                configuration.Configure("SequelConfig.config");
                //Used for Nhibernate Prof.
                configuration.SetProperty(NHibernate.Cfg.Environment.SessionFactoryName, key);
            }
            
            //Generate configuration dyanmically if the key is not empty.
            else if (dbSource == DatabaseSource.Core)
            {
                string coreFilePath = System.Configuration.ConfigurationManager.AppSettings["CoreNhibernateFile"];
                configuration.Configure(coreFilePath);
                //Used for Nhibernate Prof.
                configuration.SetProperty(NHibernate.Cfg.Environment.SessionFactoryName, key);
            }
            else if (!String.IsNullOrEmpty(oldKey))
            {
                //Check if its Managed Services first, so there's no need for round tripping.
                //Get the connection string from Managed Services.

                //If we are in Managed Services, we dont need to call a web service.
                //The section below determines that.
                string localConnectionString = String.Empty;
                string remoteConnectionString = String.Empty;

                if (Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["IsManagedServices"]))
                {
                    IInstitution institution = IInstitutionDAO.GetByCode(oldKey);
                    if (institution == null)
                    {
                        return null;
                    }
                    localConnectionString = institution.LocalConnectionString;
                    remoteConnectionString = institution.RemoteConnectionString;
                }
                else
                {
                    //using (MfbServiceRef.MfbServiceClient client = new MfbServiceRef.MfbServiceClient())
                    {
                        var mfb = "";//client.GetByCode(oldKey);
                        if (mfb == null)
                        {
                            return null;
                        }
                        localConnectionString = ""; //mfb.LocalConnectionString;
                        remoteConnectionString = "";// mfb.RemoteConnectionString;
                    }
                }

                string connectionString;
                if (dbSource == DatabaseSource.Local)
                {
                    configuration.Configure();
                    connectionString = localConnectionString;
                }
                else
                {
                    string remoteFilePath = System.Configuration.ConfigurationManager.AppSettings["RemoteNhibernateFile"];
                    //if (log.IsErrorEnabled)
                    //{
                    //    if (!System.IO.File.Exists(remoteFilePath))
                    //    {
                    //        log.Error(String.Format("Remote Config File Not Found: {0}", remoteFilePath));
                    //    }
                    //    else
                    //    {
                    //        log.Error(String.Format("Remote Config File Found: {0}", remoteFilePath));
                    //    }
                    //}
                    configuration.Configure(remoteFilePath);
                    connectionString = remoteConnectionString;
                }

                configuration.SetProperty(NHibernate.Cfg.Environment.ConnectionString, connectionString);

                //Used for Nhibernate Prof.
                configuration.SetProperty(NHibernate.Cfg.Environment.SessionFactoryName, key);

                //if (log.IsErrorEnabled)
                //{
                //    log.Error(String.Format("Connection String: {0}", NHibernate.Cfg.Environment.ConnectionString));
                //}
            }

            return configuration;
        }


        private void InitSessionFactory()
        {
            //HibernatingRhinos.NHibernate.Profiler.Appender.NHibernateProfiler.Initialize();
            HibernatingRhinos.Profiler.Appender.NHibernate.NHibernateProfiler.Initialize();
            this.ContextSession = new Dictionary<string, ISession>();
            this.sessionFactory = new Dictionary<string, ISessionFactory>();
            this.sessionFactory.Add("", new NHibernate.Cfg.Configuration().Configure().BuildSessionFactory());
        }

        private bool IsInWebContext()
        {
            return (HttpContext.Current != null);
        }
        public static void ConfigureDB()
        {
            DBSchemaUpdate.GenerateSchema();
        }
        private Dictionary<string, ISession> ContextSession
        {
            get
            {
                if (this.IsInWebContext())
                {
                    if (HttpContext.Current.Items["::NHIBERNATE_SESSION_KEY::"] == null)
                    {
                        HttpContext.Current.Items["::NHIBERNATE_SESSION_KEY::"] = new Dictionary<string, ISession>();
                    }
                    return (Dictionary<string, ISession>)HttpContext.Current.Items["::NHIBERNATE_SESSION_KEY::"];
                }
                else
                {
                    if (CallContext.GetData("::NHIBERNATE_SESSION_KEY::") == null)
                    {
                        CallContext.SetData("::NHIBERNATE_SESSION_KEY::", new Dictionary<string, ISession>());
                    }
                    return (Dictionary<string, ISession>)CallContext.GetData("::NHIBERNATE_SESSION_KEY::");

                }
            }
            set
            {
                if (this.IsInWebContext())
                {
                    HttpContext.Current.Items["::NHIBERNATE_SESSION_KEY::"] = value;
                }
                else
                {
                    CallContext.SetData("::NHIBERNATE_SESSION_KEY::", value);
                }
            }
        }



        public static NHibernateSessionManager Instance
        {
            get
            {
                return Nested.NHibernateSessionManager;
            }
        }

        private class Nested
        {
            internal static readonly NHibernateSessionManager NHibernateSessionManager = new NHibernateSessionManager();
        }
    }
}

