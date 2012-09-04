using NHibernate.Cfg;
using NUnit.Framework;

namespace Tests.NHibernate.Spatial
{
	[TestFixture]
	[Ignore("Not Implemented yet.")]
    public class MsSql2008ProjectionsFixture : ProjectionsFixture
	{
		protected override void Configure(Configuration configuration)
		{
			TestConfiguration.Configure(configuration);
		}
	}
}
