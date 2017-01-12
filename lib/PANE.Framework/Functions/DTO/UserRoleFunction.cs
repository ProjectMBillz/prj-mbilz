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
    [Serializable]
    [DataContract]
    //[Loggable]
    public class UserRoleFunction: DataObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserRoleFunction"/> class.
        /// </summary>
        public UserRoleFunction()
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="UserRoleFunction"/> class.
        /// </summary>
        /// <param name="id">The id.</param>
        public UserRoleFunction(long id)
        {
            ID = id;
        }

        /// <summary>
        /// Gets or sets the function.
        /// </summary>
        /// <value>The function.</value>
        [DataMember]
        public virtual long TheFunctionID { get; set; }

        /// <summary>
        /// Gets or sets the user role.
        /// </summary>
        /// <value>The user role.</value>
        [DataMember]
        public virtual UserRole TheUserRole { get; set; }

        //[DataMember]
        //public virtual string Endpoint { get; set; }

        [DataMember]
        public virtual long TheUserRoleID { get; set; }
 
        /// <summary>
        /// Gets or sets the sub user roles.
        /// </summary>
        /// <value>The sub user roles.</value>
        //[XmlIgnore]
        public virtual IList<UserRoleFunctionSubRole> SubUserRoles { get; set; }

        public virtual string EndPoint { get; set; }
    }
}
