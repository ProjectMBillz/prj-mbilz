using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using CBC.Framework.AuditTrail.Attributes;
using CBC.Framework.AuditTrail.DTO;
using CBC.Framework.Functions.DTO;

namespace CBC.Framework.DTO
{
    [Serializable]
    [DataContract]
    [Trailable]
    public abstract class DataObject : IDataObject, IAuditable
    {
        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        /// <value>The ID.</value>
        [DataMember]
        [TrailableIgnore]
        public virtual Int64 ID { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is deleted.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is deleted; otherwise, <c>false</c>.
        /// </value>
        [DataMember]
        [TrailableIgnore]
        public virtual System.Boolean IsDeleted { get; set; }

        [DataMember]
        [TrailableIgnore]
        public virtual string MFBCode { get; set; }

        [DataMember]
        [TrailableIgnore]
        public virtual System.Boolean LogObject { get; set;}

        [DataMember]
        [TrailableIgnore]
        public virtual bool UseAuditTrail { get; set; }

        /// <summary>
        /// This is used when we dont want an object to be logged to Audit Trail e.g. when we edit something 
        /// in the background. If true, Audit Trail is skipped.
        /// </summary>
        [TrailableIgnore]
        public virtual bool SkipAuditTrail{get;set;}


        /// <summary>
        /// Indicates that this Audit trail info being logged was auto generated and not meant to be associated with any user.
        /// </summary>
        [TrailableIgnore]
        public virtual bool IsASystemChange { get; set; }

        #region IAuditable Members

        IUser IAuditable.AuditableUser { get; set; }

        #endregion

    }
}
