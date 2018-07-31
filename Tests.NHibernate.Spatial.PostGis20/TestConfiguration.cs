using Microsoft.Extensions.Configuration;
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
        private static readonly IConfigurationRoot _configurationRoot;

        static TestConfiguration()
        {
            _configurationRoot = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
        }

        public static void Configure(Configuration configuration)
        {
            IDictionary<string, string> properties = new Dictionary<string, string>();
            properties[Environment.ProxyFactoryFactoryClass] = typeof(NHibernateFactory).AssemblyQualifiedName;
            properties[Environment.Dialect] = typeof(PostGis20Dialect).AssemblyQualifiedName;
            properties[Environment.ConnectionProvider] = typeof(DebugConnectionProvider).AssemblyQualifiedName;
            properties[Environment.ConnectionDriver] = typeof(NpgsqlDriver).AssemblyQualifiedName;
            properties[Environment.ConnectionString] = _configurationRoot.GetConnectionString("PostGis20");
            configuration.SetProperties(properties);
        }
    }
}