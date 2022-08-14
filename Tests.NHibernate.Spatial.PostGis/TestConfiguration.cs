using Microsoft.Extensions.Configuration;
using NHibernate.Bytecode;
using NHibernate.Cfg;
using NHibernate.Driver;
using NHibernate.Spatial.Dialect;
using Npgsql;
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
                [Environment.Dialect] = typeof(PostGisDialect).AssemblyQualifiedName,
                [Environment.ConnectionProvider] = typeof(DebugConnectionProvider).AssemblyQualifiedName,
                [Environment.ConnectionDriver] = typeof(NpgsqlDriver).AssemblyQualifiedName,
                [Environment.ConnectionString] = _configurationRoot.GetConnectionString("PostGis")
            };
            configuration.SetProperties(properties);

            // Use NTS plugin for mapping PostGIS types; see:
            // https://www.npgsql.org/doc/release-notes/4.0.html#improved-spatial-support-postgis
            NpgsqlConnection.GlobalTypeMapper.UseNetTopologySuite();
        }
    }
}