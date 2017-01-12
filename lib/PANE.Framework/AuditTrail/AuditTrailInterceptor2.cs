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
using System.Reflection;
using CBC.Framework.AuditTrail.DTO;

namespace CBC.Framework.AuditTrail
{
    public class AuditTrailInterceptor2 : IInterceptor
    {
        private string SYSTEM_CHANGE_TEXT = "[[SYSTEM CHANGE]]";
        public AuditTrailInterceptor2(ISession session)
        {
            _session = session;
        }

        public AuditTrailInterceptor2()
        {
            _session = null;
        }

        private ISession _session;
        private ISession _session2;
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
                if (!(entity as DataObject).UseAuditTrail)
                {
                    return ;
                }
                AuditTrail.DTO.AuditTrail trail = CreateTrail(AuditTrail.Enums.AuditAction.DELETE, entity, trailable);
                
                //Serialize just the data to be deleted.
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
                if (!(entity as DataObject).UseAuditTrail)
                {
                    return false;
                }
                AuditTrail.DTO.AuditTrail trail = CreateTrail(AuditTrail.Enums.AuditAction.UPDATE, entity, trailable);
                //Create a deep copy of the current entity.
                byte[] newObjectBytes = CBC.Framework.Utility.BinarySerializer.SerializeObject(entity);
                object newObject = CBC.Framework.Utility.BinarySerializer.DeSerializeObject(newObjectBytes);

                //Get the old copy.
                _session2.Refresh(entity);

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
                if (!(entity as DataObject).UseAuditTrail)
                {
                    return false;
                }
                AuditTrail.DTO.AuditTrail trail = CreateTrail(AuditTrail.Enums.AuditAction.CREATE, entity, trailable);

                //Serialize just the data to be saved.
                trail.Data = Utility.BinarySerializer.SerializeData(null, entity);
                _inserts.Add(trail);

                return true;
            }
            return false;
        }


        private DTO.AuditTrail CreateTrail(Enums.AuditAction action, object entity, object[] trailable)
        {
            AuditTrail.DTO.AuditTrail trail = new CBC.Framework.AuditTrail.DTO.AuditTrail();
            trail.Action = action; //Action currently performed.
            trail.ActionDate = DateTime.Now;                                   //Current date.

            trail.Object = entity as DataObject;
            if (action != CBC.Framework.AuditTrail.Enums.AuditAction.CREATE)
            {
                trail.ObjectID = trail.Object.ID;
            }

            if (!trail.Object.IsASystemChange)
            {

                try
                {
                    if (HttpContext.Current != null)
                    {
                        trail.ClientIPAddress = CBC.Framework.Utility.IPResolver.GetIP4Address(true); //IP Address of the host computer.
                        trail.ClientName = System.Net.Dns.GetHostEntry(trail.ClientIPAddress).HostName; //The name of host the computer.
                    }
                }
                catch
                {
                    trail.ClientName = "[Could not resolve Client Name]";
                }


                IAuditable auditable = entity as IAuditable;
                if (auditable != null && auditable.AuditableUser != null)
                {
                    trail.UserID = auditable.AuditableUser.ID;
                    trail.UserName = auditable.AuditableUser.UserName.Split(':')[0];
                }
            }


            trail.ApplicationName = System.Configuration.ConfigurationManager.AppSettings["ApplicationName"];

            try
            {
                IUser iUser = entity as IUser;
                if (iUser != null && !trail.Object.IsASystemChange)
                {
                    trail.SubjectIdentifier = iUser.UserName;
                    trail.DataType = String.Format("User({0})", Convert.ToString(iUser.Role.Name).Trim());
                    trail.ChangeObjectIDWhenSaving = true;
                }
                else
                {
                    //The Data Type of the entity to be persisted, e.g. User, Branch, etc.
                    trail.DataType = ((TrailableAttribute)entity.GetType().GetCustomAttributes(typeof(TrailableAttribute), false)[0]).LoggedName;
                    if (String.IsNullOrEmpty(trail.DataType)) trail.DataType = entity.GetType().Name;
                    trail.DataType = CBC.Framework.Utility.EnumBinder.SplitAtCapitalLetters(trail.DataType);

                    PropertyInfo pInfo = entity.GetType().GetProperty((trailable[0] as TrailableAttribute).MainIdentifier);
                    if (pInfo != null)
                    {
                        object identifier = pInfo.GetValue(entity, null);
                        if (identifier != null)
                        {
                            if (pInfo.PropertyType.IsEnum)
                            {
                                trail.SubjectIdentifier = CBC.Framework.Utility.EnumBinder.SplitAtCapitalLetters(identifier.ToString());
                                if ((trailable[0] as TrailableAttribute).UseMainIdentifierAsObjectID)
                                {
                                    trail.ChangeObjectIDWhenSaving = false;
                                    trail.ObjectID = Convert.ToInt32(identifier);
                                }
                            }
                            else
                            {
                                trail.ChangeObjectIDWhenSaving = true;
                                trail.SubjectIdentifier = identifier.ToString();
                            }
                        }
                    }
                }
            }
            catch { }

            if (trail.Object.IsASystemChange)
            {
                trail.ClientIPAddress = SYSTEM_CHANGE_TEXT;
                trail.ClientName = SYSTEM_CHANGE_TEXT;
                trail.UserName = SYSTEM_CHANGE_TEXT;
            }

            return trail;
        }

        public void PostFlush(System.Collections.ICollection entities)
        {
            ISession session = _session;

            try
            {
                string relatedEventsGuid = Guid.NewGuid().ToString();
                foreach (AuditTrail.DTO.AuditTrail auditTrailItem in _inserts)
                {
                    if (auditTrailItem.ChangeObjectIDWhenSaving)
                    {
                        auditTrailItem.ObjectID = auditTrailItem.Object.ID;
                    }
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
            _session2 = session;
            if (_session == null)
            {
                _session = _session2;
            }
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
