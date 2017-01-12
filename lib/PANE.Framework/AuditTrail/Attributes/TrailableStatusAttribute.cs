using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CBC.Framework.AuditTrail.Attributes
{
    /// <summary>
    /// When used, the specify that the property of the class is the default Status.
    /// </summary>
    [global::System.AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class TrailableStatusAttribute : Attribute
    {
    }
}
