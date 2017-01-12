using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CBC.Framework.AuditTrail.Attributes
{
    /// <summary>
    /// Overrides the Name of the property.
    /// </summary>
    [global::System.AttributeUsage(AttributeTargets.Property | AttributeTargets.Enum, AllowMultiple=false)]
    public sealed class TrailableNameAttribute : Attribute
    {
        private string _loggedName = string.Empty;
        private string _emptyStringText = String.Empty;
        private bool _useEmptyStringText = false;

        public TrailableNameAttribute(string name)
        {
            _loggedName = name;
            this._useEmptyStringText = false;
        }

        public TrailableNameAttribute(string name, string emptyStringText)
            :this(name)
        {
            this._useEmptyStringText = true;
            this._emptyStringText = emptyStringText;
        }

        public bool UseEmptyStringText
        {
            get
            {
                return this._useEmptyStringText;
            }
        }

        public string LoggedName
        {
            get
            {
                return this._loggedName;
            }
        }

        public string EmptyStringText
        {
            get
            {
                return this._emptyStringText;
            }
        }
    }
}
