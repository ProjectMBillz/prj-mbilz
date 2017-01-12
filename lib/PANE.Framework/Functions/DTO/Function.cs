using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using CBC.Framework.DTO;
using System.Runtime.Serialization;
using CBC.Framework.Approval.DTO;

namespace CBC.Framework.Functions.DTO
{
    [Serializable]
    [DataContract]
    public class Function : DataObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Function"/> class.
        /// </summary>
        public Function()
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="Function"/> class.
        /// </summary>
        /// <param name="id">The id.</param>
        public Function(long id)
        {
            ID = id;
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [DataMember]
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        [DataMember]
        public virtual string Description { get; set; }


        /// <summary>
        /// Gets or sets the parent function.
        /// </summary>
        /// <value>The parent function.</value>
        [DataMember]
        public virtual Function ParentFunction { get; set; }


        /// <summary>
        /// Gets or sets the name of the role.
        /// </summary>
        /// <value>The name of the role.</value>
        [DataMember]
        public virtual string RoleName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has sub roles.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has sub roles; otherwise, <c>false</c>.
        /// </value>
        [DataMember]
        public virtual Boolean HasSubRoles { get; set; }


        //[DataMember]
        //public virtual IList<ApprovalConfiguration> ApprovalConfigurations { get; set; }

        [DataMember]
        public virtual bool IsApprovable { get; set; }

        [DataMember]
        public virtual IList<ApprovalConfiguration> TheApprovalConfigurations { get; set; }

        [DataMember]
        public virtual ApprovalConfiguration ApprovalConfigurationUpdate { get; set; }

        [DataMember]
        public virtual UserRole SubUserRoleUpdate { get; set; }

        [DataMember]
        public virtual long ApplicationID { get; set; }

        [DataMember]
        public virtual UserCategory UserCategory { get; set; }

        [DataMember]
        public virtual bool IsDefault { get; set; }

    }
}
