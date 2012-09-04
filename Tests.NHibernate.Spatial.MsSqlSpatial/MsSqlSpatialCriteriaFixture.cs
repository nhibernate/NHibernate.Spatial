using NHibernate.Cfg;
using NUnit.Framework;

namespace Tests.NHibernate.Spatial
{
	[TestFixture]
	public class MsSqlSpatialCriteriaFixture : CriteriaFixture
	{
		protected override void Configure(Configuration configuration)
		{
			TestConfiguration.Configure(configuration);
		}
	}
}
