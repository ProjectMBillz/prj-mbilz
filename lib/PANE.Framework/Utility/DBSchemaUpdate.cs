using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Tool.hbm2ddl;
using NHibernate.Cfg;

namespace CBC.Framework.Utility
{
    public class DBSchemaUpdate
    {
        public DBSchemaUpdate()
        { 
        }
        
        public static void GenerateSchema()
        {
            var cfg = new Configuration();
            
            SchemaUpdate su = new SchemaUpdate(cfg);
            try
            {
                cfg.SetProperty("hbm2dll", "update");
                cfg.Configure();
                su.Execute(true, true);
            }
            catch (Exception e)
            { }
        }
        public static void AddGenerateSchema(List<Type> assemblyNames)
        {
            var cfg = new Configuration();
            try
            {
                cfg.Configure();
                foreach (Type t in assemblyNames)
                {
                    cfg.AddAssembly(t.Assembly);
                }
                new SchemaUpdate(cfg).Execute(true, true);
            }
            catch (Exception e)
            { }
        }
    }
}
