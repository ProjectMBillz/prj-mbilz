using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CBC.Framework.ISO8583.Utility
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class XMLNameAttribute : Attribute
    {
        /// <summary>
        /// Constructor for specifying the type this attribute should be applied to.
        /// </summary>
        /// <param name="name">A friendly name of this class.</param>
        public XMLNameAttribute(string name)
        {
            this.Name = name;
        }

        public string Name { get; set; }


    }
}
