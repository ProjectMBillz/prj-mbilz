using System;
using CBC.Framework.AuditTrail.Enums;
using CBC.Framework.DTO;
using CBC.Framework.Functions.DTO;

namespace CBC.Framework.AuditTrail.DTO
{

    [Serializable]
    public class AuditTrail : DataObject
    {
        public AuditTrail()
        { }

        public AuditTrail(long id)
        {
            this.ID = id;
        }

        /// <summary>
        /// The type of Action that occured. It could either be create, update or delete.
        /// </summary>
        public virtual AuditAction Action { get; set; }

        /// <summary>
        /// The Date this Action happened.
        /// </summary>
        public virtual DateTime ActionDate { get; set; }

        /// <summary>
        /// A combination of the data that was added/modified at both stages. 
        /// More like a before and after.
        /// </summary>
        public virtual byte[] Data { get; set; }

        /// <summary>
        /// The Data Type of the data. E.g User, Branch, State, etc.
        /// </summary>
        public virtual String DataType { get; set; }

        /// <summary>
        /// The Unique Identifier of the Object.
        /// </summary>
        public virtual long ObjectID { get; set; }

        /// <summary>
        /// The User Name of the person that performed this action. 
        /// </summary>
        public virtual string UserName { get; set; }

        /// <summary>
        /// The ID of the person that performed this action.
        /// </summary>
        public virtual long UserID { get; set; }

        /// <summary>
        /// The IP Address of the machine this action was performed on.
        /// </summary>
        public virtual string ClientIPAddress { get; set; }

        /// <summary>
        /// The Name of the machine this action was performed on.
        /// </summary>
        public virtual string ClientName { get; set; }

        /// <summary>
        /// This indicated actions that occured at the same time. 
        /// <example>Assuming a banking transaction, if u debit and credit 2 different accounts,
        /// even though the Audit Trail for Account 1 may show things related to only it, this property indicates
        /// that the action was actually done in conjunction with crediting Account 2.
        /// </example>
        /// </summary>
        public virtual string GroupName { get; set; }

        public virtual string SubjectIdentifier { get; set; }


        /// <summary>
        /// Utility property. Used when saving an object in AuditTrailInterceptor.
        /// </summary>
        public virtual DataObject Object { get; set; }

        /// <summary>
        /// Utility property. Used when saving an object in AuditTrailInterceptor.
        /// </summary>
        public virtual bool ChangeObjectIDWhenSaving { get; set; }

        /// <summary>
        /// The name of the current application.
        /// </summary>
        public virtual string ApplicationName { get; set; }

        /// <summary>
        /// Utility property. Do not modify.
        /// </summary>
        public virtual string DataTypeString { get { return CBC.Framework.Utility.EnumBinder.SplitAtCapitalLetters(this.DataType); } }

    }
}
