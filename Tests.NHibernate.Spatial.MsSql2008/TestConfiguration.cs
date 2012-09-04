using System.Collections.Generic;
using NHibernate.ByteCode.Castle;
using NHibernate.Cfg;
using NHibernate.Driver;
using NHibernate.Spatial.Dialect;
using Environment = NHibernate.Cfg.Environment;
using Settings = Tests.NHibernate.Spatial.Properties.Settings;

namespace Tests.NHibernate.Spatial
{
	internal static class TestConfiguration
	{
		public static void Configure(Configuration configuration)
		{
			IDictionary<string, string> properties = new Dictionary<string, string>();
			properties[Environment.ProxyFactoryFactoryClass] = typeof(ProxyFactoryFactory).AssemblyQualifiedName;
			properties[Environment.Dialect] = typeof(MsSql2008GeometryDialect).AssemblyQualifiedName;
			properties[Environment.ConnectionProvider] = typeof(DebugConnectionProvider).AssemblyQualifiedName;
			properties[Environment.ConnectionDriver] = typeof(SqlClientDriver).AssemblyQualifiedName;
			properties[Environment.ConnectionString] = Settings.Default.ConnectionString;
			//properties[Environment.Hbm2ddlAuto] = "create-drop";
			configuration.SetProperties(properties);
		}

	}
}
