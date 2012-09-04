using NHibernate.Cfg;
using NUnit.Framework;
using Tests.NHibernate.Spatial.OgcSfSql11Compliance;

namespace Tests.NHibernate.Spatial
{
	[TestFixture]
	public class MsSql2008ConformanceItemsFixture : ConformanceItemsFixture
	{
		protected override void Configure(Configuration configuration)
		{
			TestConfiguration.Configure(configuration);
		}
	}
}
