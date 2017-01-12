using System;
using System.Collections.Generic;
using System.Text;
using CBC.Framework.DTO;
using CBC.Framework.Functions.DTO;

namespace CBC.Framework.Functions.DTO
{
    public interface IUser: IDataObject
    {
        string UserID { get; set; }

        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        /// <value>The name of the user.</value>
        System.String UserName { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>The password.</value>
        System.String Password { get; set; }

        

        /// <summary>
        /// Gets or sets the role.
        /// </summary>
        /// <value>The role.</value>
        UserRole Role { get; set; }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        /// <value>The email.</value>
        string Email { get; set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>The status.</value>
        UserStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the last login date.
        /// </summary>
        /// <value>The last login date.</value>
        System.DateTime LastLoginDate { get; set; }

        /// <summary>
        /// Gets or sets the creation date.
        /// </summary>
        /// <value>The creation date.</value>
        System.DateTime CreationDate { get; set; }

        /// <summary>
        /// Gets or sets the number of times a User tried to Log in unsucessfully.
        /// </summary>
        /// /// <value>The Failed Password Attempt count.</value>
        int FailedPasswordAttemptCount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        bool IsLockedOut { get; set; }


        /// <summary>
        /// Used to skip Audit Trail when un-necessary.
        /// </summary>
        bool DoesntNeedAudit { get; set; }

        /// <summary>
        /// Indicates whether its a super user.
        /// </summary>
        bool IsSuperUser { get; set; }
    }
}
