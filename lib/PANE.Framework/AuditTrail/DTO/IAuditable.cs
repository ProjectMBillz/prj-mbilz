using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CBC.Framework.Functions.DTO;

namespace CBC.Framework.AuditTrail.DTO
{
    public interface IAuditable
    {
        IUser AuditableUser { get; set; }
    }
}
