using NHibernate.Cfg;
using NHibernate.Driver;
using NHibernate.Spatial.Dialect;
using System.Collections.Generic;
using Environment = NHibernate.Cfg.Environment;
using NHibernateFactory = NHibernate.Bytecode.DefaultProxyFactoryFactory;
using Settings = Tests.NHibernate.Spatial.Properties.Settings;

namespace Tests.NHibernate.Spatial
{
    internal static class TestConfiguration
    {
        public static void Configure(Configuration configuration)
        {
            IDictionary<string, string> properties = new Dictionary<string, string>();
            properties[Environment.ProxyFactoryFactoryClass] = typeof(NHibernateFactory).AssemblyQualifiedName;
            properties[Environment.Dialect] = typeof(MySQLSpatialDialect).AssemblyQualifiedName;
            properties[Environment.ConnectionProvider] = typeof(DebugConnectionProvider).AssemblyQualifiedName;
            properties[Environment.ConnectionDriver] = typeof(MySqlDataDriver).AssemblyQualifiedName;
            properties[Environment.ConnectionString] = Settings.Default.ConnectionString;
            configuration.SetProperties(properties);
        }
    }
}