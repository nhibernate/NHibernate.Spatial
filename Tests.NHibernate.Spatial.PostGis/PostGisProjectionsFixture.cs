using NHibernate.Cfg;
using NHibernate.Driver;
using NHibernate.Criterion;
using NHibernate.Spatial.Dialect;
using NUnit.Framework;

namespace Tests.NHibernate.Spatial
{
	[TestFixture]
	public class PostGisProjectionsFixture : ProjectionsFixture
	{
		protected override void Configure(Configuration configuration)
		{
			TestConfiguration.Configure(configuration);
		}
	}
}
