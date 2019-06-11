using Microsoft.Extensions.Configuration;
using NHibernate.Bytecode;
using NHibernate.Cfg;
using NHibernate.Driver;
using NHibernate.Spatial.Dialect;
using System.Collections.Generic;

namespace Tests.NHibernate.Spatial
{
    internal static class TestConfiguration
    {
        private static readonly IConfigurationRoot _configurationRoot;

        static TestConfiguration()
        {
            _configurationRoot = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
        }

        public static void Configure(Configuration configuration)
        {
            IDictionary<string, string> properties = new Dictionary<string, string>
            {
                [Environment.ProxyFactoryFactoryClass] = typeof(StaticProxyFactoryFactory).AssemblyQualifiedName,
                [Environment.Dialect] = typeof(MsSql2008GeometryDialect).AssemblyQualifiedName,
                [Environment.ConnectionProvider] = typeof(DebugConnectionProvider).AssemblyQualifiedName,
                [Environment.ConnectionDriver] = typeof(SqlClientDriver).AssemblyQualifiedName,
                [Environment.ConnectionString] = _configurationRoot.GetConnectionString("MsSql2008")
            };
            configuration.SetProperties(properties);
        }
    }
}
