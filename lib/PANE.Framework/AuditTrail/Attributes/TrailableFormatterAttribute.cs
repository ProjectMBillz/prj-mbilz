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
    public sealed class TrailableFormatterAttribute : Attribute
    {
        private string _format = string.Empty;
        private int _noToDivideBy;
        private bool _doDivision;

        public TrailableFormatterAttribute(string format)
            :this(format, 1)
        {
            _doDivision = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <param name="noToDivideBy">Applicable to ints or decimals that were saved in kobo and need to be converted to say naira.</param>
        public TrailableFormatterAttribute(string format, int noToDivideBy)
        {
            _noToDivideBy = noToDivideBy;
            this._format = format;
            _doDivision = true;
        }

        public string Format
        {
            get
            {
                return this._format;
            }
        }

        public int NoToDivideBy
        {
            get
            {
                return this._noToDivideBy;
            }
        }

        public bool DoDivision
        {
            get { return _doDivision; ;}
        }
    }
}
