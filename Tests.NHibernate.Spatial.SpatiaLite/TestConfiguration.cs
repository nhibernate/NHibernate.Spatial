using Microsoft.Extensions.Configuration;
using NHibernate.Bytecode;
using NHibernate.Cfg;
using NHibernate.Spatial.Dialect;
using NHibernate.Spatial.Driver;
using System.Collections.Generic;
using System.IO;

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

            try
            {
                File.Delete("nhsp_test.sqlite");
            }
            catch
            { }
        }

        public static void Configure(Configuration configuration)
        {
            IDictionary<string, string> properties = new Dictionary<string, string>
            {
                [Environment.ProxyFactoryFactoryClass] = typeof(StaticProxyFactoryFactory).AssemblyQualifiedName,
                [Environment.Dialect] = typeof(SpatiaLiteDialect).AssemblyQualifiedName,
                [Environment.ConnectionProvider] = typeof(DebugConnectionProvider).AssemblyQualifiedName,
                [Environment.ConnectionDriver] = typeof(SpatiaLiteDriver).AssemblyQualifiedName,
                [Environment.ConnectionString] = _configurationRoot.GetConnectionString("SpatiaLite")
            };
            configuration.SetProperties(properties);
        }
    }
}
