using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CBC.Framework.AuditTrail.Attributes
{
    /// <summary>
    /// Used for classes that need to have Audit Trail functionality.
    /// </summary>
    [global::System.AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum, AllowMultiple = false)]
    public sealed class TrailableAttribute : Attribute
    {
        private string _loggedName = string.Empty;
        private string _identifier = String.Empty;
        public TrailableAttribute()
            : this("")
        {
        }

        public TrailableAttribute(string name)
            : this(name, "Name")
        {
        }

        public TrailableAttribute(string name, string mainIdentifier)
        {
            _loggedName = name;
            _identifier = mainIdentifier;
            MaxDepth = 4;
        }

        public bool UseMainIdentifierAsObjectID { get; set; }

        public string LoggedName
        {
            get
            {
                return this._loggedName;
            }
        }

        public string MainIdentifier
        {
            get
            {
                return this._identifier;
            }
        }

        public int MaxDepth { get; set; }

    }
}
