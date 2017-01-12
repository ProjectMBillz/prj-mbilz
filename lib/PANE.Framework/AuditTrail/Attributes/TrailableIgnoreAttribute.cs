using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CBC.Framework.AuditTrail.Attributes
{
    /// <summary>
    /// When used, it specifies that the property should be be ignored.
    /// </summary>
    [global::System.AttributeUsage(AttributeTargets.Property, AllowMultiple=false)]
    public sealed class TrailableIgnoreAttribute : Attribute
    {
        private bool _isSpecial;
        public TrailableIgnoreAttribute(bool isSpecial)
        {
            this._isSpecial = isSpecial;
        }

        public TrailableIgnoreAttribute()
            : this(false)
        {
        }

        public bool IsSpecial { get { return this._isSpecial; } }
    }
}
