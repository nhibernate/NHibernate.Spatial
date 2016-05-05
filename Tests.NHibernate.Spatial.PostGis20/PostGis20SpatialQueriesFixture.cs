using NHibernate;
using NHibernate.Cfg;
using NUnit.Framework;

namespace Tests.NHibernate.Spatial.RandomGeometries
{
	[TestFixture]
	public class PostGis20SpatialQueriesFixture : PostGisSpatialQueriesFixture
	{
		protected override void Configure(Configuration configuration)
		{
			TestConfiguration.Configure(configuration);
		}
	}
}