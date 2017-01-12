using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using CBC.Framework.DTO;
using System.Runtime.Serialization;
using CBC.Framework.AuditTrail.Attributes;
using System.Xml.Serialization;

namespace CBC.Framework.Functions.DTO
{
    [Trailable]
    [DataContract]
    [Serializable]
    // [Loggable]
    public class UserRole : DataObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserRole"/> class.
        /// </summary>
        public UserRole()
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="UserRole"/> class.
        /// </summary>
        /// <param name="id">The id.</param>
        
        public UserRole(long id)
        {
            ID = id;
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [DataMember]
        public virtual string Name { get; set; }


        [DataMember]
        public virtual string Code { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        [DataMember]
        public virtual string Description { get; set; }

        /// <summary>
        /// Gets or sets the scope.
        /// </summary>
        /// <value>The scope.</value>
        //[DataMember]
        [TrailableIgnore]
        [DataMember]
        public virtual UserRoleScope Scope { get; set; }

        [TrailableIgnore]
        
        public virtual int ScopeID
        {
            get { return Convert.ToInt32(Scope); }
            set
            {
                throw new Exception();
            }
        }
        public virtual bool HasApprovalRight { get; set; }
        //[DataMember]
        public virtual Status Status { get; set; }

        [DataMember]
        [TrailableIgnore]
        public virtual UserCategory UserCategory { get; set; }

        /// <summary>
        /// Gets or sets the user role functions.
        /// </summary>
        /// <value>The user role functions.</value>
        //[XmlIgnore]
        //[TrailableIgnore]
        //[TrailableInnerClass(true, "Name")]
        //[TrailableName("Functions")]
        //public virtual IList<UserRoleFunction> TheUserRoleFunctions { get; set; }

        [TrailableName("Scope")]
        public virtual string ScopeName { get { return this.Scope.ToString(); } }
        [TrailableIgnore]
        public virtual bool StatusBool { get { return this.Status == Status.Active ? true : false; } }

    }
}
