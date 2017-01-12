using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CBC.Framework.AuditTrail.Attributes
{
    [global::System.AttributeUsage(AttributeTargets.Property | AttributeTargets.Enum, AllowMultiple = false)]
    public sealed class TrailableDependencyPropertyAttribute : Attribute
    {
        private string _propertyName;
        private string _ifTrueText;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyName">Must be a Boolean property in this class.</param>\
        /// <param name="ifTrueText">The text to specify if the <paramref name="PropertyName"/> is true.</param>
        public TrailableDependencyPropertyAttribute(string propertyName, string ifTrueText)
        {
            this._propertyName = propertyName;
            this._ifTrueText = ifTrueText;
        }

        public string IfTrueText { get { return this._ifTrueText; } }

        public string PropertyName { get { return this._propertyName; } }
    }
}
