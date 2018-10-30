using NHibernate.Cfg;
using NHibernate.Driver;
using NHibernate.Spatial.Dialect;
using System.Collections.Generic;
using Environment = NHibernate.Cfg.Environment;
using NHibernateFactory = NHibernate.Bytecode.DefaultProxyFactoryFactory;

namespace Tests.NHibernate.Spatial
{
    internal static class TestConfiguration
    {
        public static void Configure(Configuration configuration)
        {
            IDictionary<string, string> properties = new Dictionary<string, string>();
            properties[Environment.ProxyFactoryFactoryClass] = typeof(NHibernateFactory).AssemblyQualifiedName;
            properties[Environment.Dialect] = typeof(MsSql2012GeometryDialect).AssemblyQualifiedName;
            properties[Environment.ConnectionProvider] = typeof(DebugConnectionProvider).AssemblyQualifiedName;
            properties[Environment.ConnectionDriver] = typeof(SqlClientDriver).AssemblyQualifiedName;
            properties[Environment.ConnectionString] = System.Configuration.ConfigurationManager.ConnectionStrings["SqlServer2012"].ConnectionString;
            configuration.SetProperties(properties);
        }
    }
}
