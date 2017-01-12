using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CBC.Framework.AuditTrail.Attributes
{
    /// <summary>
    /// Used for classes that need to have Audit Trail functionality.
    /// </summary>
    [global::System.AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class TrailableInnerClassAttribute : Attribute
    {
        private bool _needOnlyNameProperty = false;
        private string _classNameIdentifier = String.Empty;

        public TrailableInnerClassAttribute(bool needOnlyNameProperty, string classNameIdentifier)
        {
            this._needOnlyNameProperty = needOnlyNameProperty;
            _classNameIdentifier = classNameIdentifier;
            this.Seperator = "-";
            this.AppendPropertiesToClassName = true;
            this.UseSeperator = true;
        }

        public TrailableInnerClassAttribute(bool needOnlyNameProperty)
            : this(needOnlyNameProperty, "Name")
        { }

        public TrailableInnerClassAttribute()
            : this(true)
        { }
       
       //Should be used only on class properties that need you to retrieve only its name property.
        public bool NeedOnlyNameProperty
        {
            get
            {
                return this._needOnlyNameProperty;
            }
        }

      
        //Is only needed if the NeedOnlyNameProperty is set to true.
        public string ClassNameIdentifier
        {
            get
            {
                return this._classNameIdentifier;
            }
        }

        /// <summary>
        /// Given a class, if we are to display the inner properties, this property achieves the essence
        /// of displaying the sub classes properties with the name of the parent class. 
        /// </summary>
        public bool AppendPropertiesToClassName { get; set; }


        /// <summary>
        /// Can apply only if this.NeedOnlyNameProperty == false
        /// </summary>
        public bool UseSeperator { get; set; }

        /// <summary>
        /// Only applies if this.UseSeperator == true.
        /// </summary>
        public string Seperator { get; set; }
        
    }
}
