using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace CBC.Framework.NHibernateManager.Configuration
{
    public class SessionFactoryElement : ConfigurationElement
    {
        public SessionFactoryElement() { }

        public SessionFactoryElement(string name, string configPath)
        {
            Name = name;
            FactoryConfigPathLocal = configPath;
        }

        [ConfigurationProperty("name", IsRequired = true,
             IsKey = true, DefaultValue = "Not Supplied")]
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }

        [ConfigurationProperty("factoryConfigPathLocal", IsRequired = true,
                 DefaultValue = "Not Supplied")]
        public string FactoryConfigPathLocal
        {
            get { return (string)this["factoryConfigPathLocal"]; }
            set { this["factoryConfigPathLocal"] = value; }
        }

        [ConfigurationProperty("factoryConfigPathRemote", IsRequired = true, DefaultValue = "Not Supplied")]
        public string FactoryConfigPathRemote
        {
            get { return (string)this["factoryConfigPathRemote"]; }
            set { this["factoryConfigPathRemote"] = value; }
        }


        //[ConfigurationProperty("isTransactional",
        //              IsRequired = false, DefaultValue = false)]
        //public bool IsTransactional
        //{
        //    get { return (bool)this["isTransactional"]; }
        //    set { this["isTransactional"] = value; }
        //}
    }
}