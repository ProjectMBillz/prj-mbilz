using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using System.Web.Security;
using System.Web;
using CBC.Framework.Functions.DTO;
using CBC.Framework.AuditTrail.Attributes;
using CBC.Framework.DTO;
using CBC.Framework.AuditTrail.Exceptions;

namespace CBC.Framework.AuditTrail
{
    public class AuditTrailInterceptorOld : IInterceptor
    {
        private ISession _session;
        private List<AuditTrail.DTO.AuditTrail> _inserts = new List<CBC.Framework.AuditTrail.DTO.AuditTrail>();
        private List<AuditTrail.DTO.AuditTrail> _updates = new List<CBC.Framework.AuditTrail.DTO.AuditTrail>();
        private List<AuditTrail.DTO.AuditTrail> _deletes = new List<CBC.Framework.AuditTrail.DTO.AuditTrail>();

        #region IInterceptor Members

        public void AfterTransactionBegin(ITransaction tx)
        {
           // throw new NotImplementedException();
        }

        public void AfterTransactionCompletion(ITransaction tx)
        {
            //throw new NotImplementedException();
        }

        public void BeforeTransactionCompletion(ITransaction tx)
        {
            //throw new NotImplementedException();
        }

        public int[] FindDirty(object entity, object id, object[] currentState, object[] previousState, string[] propertyNames, NHibernate.Type.IType[] types)
        {
           // throw new NotImplementedException();
            return null;
        }

        public object Instantiate(Type type, object id)
        {
            //throw new NotImplementedException();
            return null;
        }

        public object IsUnsaved(object entity)
        {
            //throw new NotImplementedException();
            return null;
        }

        public void OnDelete(object entity, object id, object[] state, string[] propertyNames, NHibernate.Type.IType[] types)
        {
            //Class must have TrailableAttribute attribute to be enabled for Audit Trail.
            object[] trailable = entity.GetType().GetCustomAttributes(typeof(TrailableAttribute), false);
            if (trailable.Length > 0)
            {
                AuditTrail.DTO.AuditTrail trail = new CBC.Framework.AuditTrail.DTO.AuditTrail();
                trail.Action = CBC.Framework.AuditTrail.Enums.AuditAction.DELETE; //Action currently performed.
                trail.ActionDate = DateTime.Now;                                   //Current date.

                try
                {
                    trail.ClientIPAddress = HttpContext.Current.Request.UserHostAddress; //IP Address of the host computer.
                    trail.ClientName = System.Net.Dns.GetHostEntry(trail.ClientIPAddress).HostName; //The name of host the computer.
                }
                catch
                {
                    trail.ClientName = "[Could not resolve Client Name]";
                }

                try
                {
                    FunctionsMembershipUser memUser = Membership.GetUser() as FunctionsMembershipUser;
                    if (memUser != null)
                    {
                        trail.UserID = memUser.UserDetails.ID;
                        trail.UserName = memUser.UserName.Split(':')[0];
                    }
                }
                catch { }
                trail.ObjectID = (entity as DataObject).ID;
                trail.ApplicationName = System.Configuration.ConfigurationManager.AppSettings["ApplicationName"];
                
                //The Data Type of the entity to be persisted, e.g. User, Branch, etc.
                trail.DataType = ((TrailableAttribute)entity.GetType().GetCustomAttributes(typeof(TrailableAttribute), false)[0]).LoggedName;
                if (String.IsNullOrEmpty(trail.DataType)) trail.DataType = entity.GetType().Name;

                try
                {
                    IUser iUser = entity as IUser;
                    if (iUser != null)
                    {
                        trail.SubjectIdentifier = iUser.UserName;
                        trail.DataType = iUser.Role.Name;
                    }
                    else
                    {
                        object identifier = entity.GetType().GetProperty((trailable[0] as TrailableAttribute).MainIdentifier).GetValue(entity, null);
                        if (identifier != null)
                        {
                            trail.SubjectIdentifier = identifier.ToString();
                        }
                    }
                }
                catch { }

                //Serialize just the data to be saved.
                trail.Data = Utility.BinarySerializer.SerializeData(entity, null);

                _deletes.Add(trail);

            }
        }

        public bool OnFlushDirty(object entity, object id, object[] currentState, object[] previousState, string[] propertyNames, NHibernate.Type.IType[] types)
        {
            //Class must have Loggable attribute to be enabled for Audit Trail.
            object[] trailable = entity.GetType().GetCustomAttributes(typeof(TrailableAttribute), false);
            if (trailable.Length > 0)
            {
                AuditTrail.DTO.AuditTrail trail = new CBC.Framework.AuditTrail.DTO.AuditTrail();
                trail.Action = CBC.Framework.AuditTrail.Enums.AuditAction.UPDATE; //Action currently performed.
                trail.ActionDate = DateTime.Now;                                   //Current date.

                try
                {
                    trail.ClientIPAddress = HttpContext.Current.Request.UserHostAddress; //IP Address of the host computer.
                    trail.ClientName = System.Net.Dns.GetHostEntry(trail.ClientIPAddress).HostName; //The name of host the computer.
                }
                catch
                {
                    trail.ClientName = "[Could not resolve Client Name]";
                }

                try
                {
                    FunctionsMembershipUser memUser = Membership.GetUser() as FunctionsMembershipUser;
                    if (memUser != null)
                    {
                        trail.UserID = memUser.UserDetails.ID;
                        trail.UserName = memUser.UserName.Split(':')[0];
                    }
                }
                catch { }
                trail.ObjectID = (entity as DataObject).ID;
                trail.ApplicationName = System.Configuration.ConfigurationManager.AppSettings["ApplicationName"];

                //The Data Type of the entity to be persisted, e.g. User, Branch, etc.
                trail.DataType = ((TrailableAttribute)entity.GetType().GetCustomAttributes(typeof(TrailableAttribute), false)[0]).LoggedName;
                if (String.IsNullOrEmpty(trail.DataType)) trail.DataType = entity.GetType().Name;

                try
                {
                    IUser iUser = entity as IUser;
                    if (iUser != null)
                    {
                        trail.SubjectIdentifier = iUser.UserName;
                        trail.DataType = iUser.Role.Name;
                    }
                    else
                    {
                        object identifier = entity.GetType().GetProperty((trailable[0] as TrailableAttribute).MainIdentifier).GetValue(entity, null);
                        if (identifier != null)
                        {
                            trail.SubjectIdentifier = identifier.ToString();
                        }
                    }
                }
                catch { }

                //Create a deep copy of the current entity.
                byte[] newObjectBytes = CBC.Framework.Utility.BinarySerializer.SerializeObject(entity);
                object newObject = CBC.Framework.Utility.BinarySerializer.DeSerializeObject(newObjectBytes);

                //Get the old copy.
                _session.Refresh(entity);

                //Get the comparison b/w the old and new data and serialize as bytes.
                trail.Data = Utility.BinarySerializer.SerializeData(entity, newObject);

                _updates.Add(trail);

                return true;
            }
            return false;
        }

        public bool OnLoad(object entity, object id, object[] state, string[] propertyNames, NHibernate.Type.IType[] types)
        {
            //throw new NotImplementedException();
            return false;
        }

        
        public bool OnSave(object entity, object id, object[] state, string[] propertyNames, NHibernate.Type.IType[] types)
        {
            //Class must have TrailableAttribute attribute to be enabled for Audit Trail.
            object[] trailable = entity.GetType().GetCustomAttributes(typeof(TrailableAttribute), false);
            if (trailable.Length > 0)
            {
                AuditTrail.DTO.AuditTrail trail = new CBC.Framework.AuditTrail.DTO.AuditTrail();
                trail.Action = CBC.Framework.AuditTrail.Enums.AuditAction.CREATE; //Action currently performed.
                trail.ActionDate = DateTime.Now;                                   //Current date.

                try
                {
                    trail.ClientIPAddress= HttpContext.Current.Request.UserHostAddress; //IP Address of the host computer.
                    trail.ClientName = System.Net.Dns.GetHostEntry(trail.ClientIPAddress).HostName; //The name of host the computer.
                }
                catch
                {
                    trail.ClientName = "[Could not resolve Client Name]";
                }

                try
                {
                    FunctionsMembershipUser memUser = Membership.GetUser() as FunctionsMembershipUser;
                    if (memUser != null)
                    {
                        trail.UserID = memUser.UserDetails.ID;
                        trail.UserName = memUser.UserName.Split(':')[0];
                    }
                }
                catch { }
                //trail.ObjectID = (entity as DataObject).ID;
                trail.Object = (entity as DataObject);
                trail.ApplicationName = System.Configuration.ConfigurationManager.AppSettings["ApplicationName"];

                //The Data Type of the entity to be persisted, e.g. User, Branch, etc.
                trail.DataType = ((TrailableAttribute)entity.GetType().GetCustomAttributes(typeof(TrailableAttribute), false)[0]).LoggedName;
                if (String.IsNullOrEmpty(trail.DataType)) trail.DataType = entity.GetType().Name;

                try
                {
                    IUser iUser = entity as IUser;
                    if (iUser != null)
                    {
                        trail.SubjectIdentifier = iUser.UserName;
                        trail.DataType = iUser.Role.Name;
                    }
                    else
                    {
                        object identifier = entity.GetType().GetProperty((trailable[0] as TrailableAttribute).MainIdentifier).GetValue(entity, null);
                        if (identifier != null)
                        {
                            trail.SubjectIdentifier = identifier.ToString();
                        }
                    }
                }
                catch { }

                //Serialize just the data to be saved.
                trail.Data = Utility.BinarySerializer.SerializeData(null, entity);

                _inserts.Add(trail);

                return true;
            }
            return false;
        }

        public void PostFlush(System.Collections.ICollection entities)
        {
            ISession session = _session;

            try
            {
                string relatedEventsGuid = Guid.NewGuid().ToString();
                foreach (AuditTrail.DTO.AuditTrail auditTrailItem in _inserts)
                {
                    auditTrailItem.ObjectID = auditTrailItem.Object.ID; 
                    auditTrailItem.GroupName = relatedEventsGuid;
                    session.Save(auditTrailItem);
                }
                foreach (AuditTrail.DTO.AuditTrail auditTrailItem in _updates)
                {
                    auditTrailItem.GroupName = relatedEventsGuid;
                    session.Save(auditTrailItem);
                }
                foreach (AuditTrail.DTO.AuditTrail auditTrailItem in _deletes)
                {
                    auditTrailItem.GroupName = relatedEventsGuid;
                    session.Save(auditTrailItem);
                }
            }
            catch (HibernateException e)
            {
                throw new CallbackException(e);
            }
            finally
            {
                _inserts.Clear();
                _updates.Clear();
                _deletes.Clear();
                //session.Flush();
                //session.Close();
            }
        }

        public void PreFlush(System.Collections.ICollection entities)
        {
            //throw new NotImplementedException();
        }

        public void SetSession(ISession session)
        {
            //throw new NotImplementedException();
            _session = session;
        }

        #endregion

        #region IInterceptor Members


        public object GetEntity(string entityName, object id)
        {
            return null;
            //throw new NotImplementedException();
        }

        public string GetEntityName(object entity)
        {
            return null;
            //throw new NotImplementedException();
        }

        public object Instantiate(string entityName, EntityMode entityMode, object id)
        {
            return null;
            //throw new NotImplementedException();
            
        }

        public bool? IsTransient(object entity)
        {
            return null;
            //throw new NotImplementedException();
        }

        public void OnCollectionRecreate(object collection, object key)
        {
            //return null;
            //throw new NotImplementedException();
        }

        public void OnCollectionRemove(object collection, object key)
        {
            //return null;
            //throw new NotImplementedException();
        }

        public void OnCollectionUpdate(object collection, object key)
        {
            //return null;
            //throw new NotImplementedException();
        }

        public NHibernate.SqlCommand.SqlString OnPrepareStatement(NHibernate.SqlCommand.SqlString sql)
        {
            return sql;
        }

        #endregion
    }
}
