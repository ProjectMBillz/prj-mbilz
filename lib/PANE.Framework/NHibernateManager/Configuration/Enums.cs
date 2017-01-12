using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CBC.Framework.NHibernateManager.Configuration
{
    public enum DatabaseSource
    {
        /// <summary>
        /// Used for Databases on CBC's end.
        /// </summary>
        Local = 0,
        /// <summary>
        /// Used for databases on the institutions' end.
        /// </summary>
        Remote,

        /// <summary>
        /// Another kind of databases on CBCs' end. A special type though.
        /// </summary>
        Core,

        Sequel
    }
}
