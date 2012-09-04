using NHibernate.Cfg;
using NUnit.Framework;

namespace Tests.NHibernate.Spatial
{
	[TestFixture]
    public class MsSql2008CriteriaFixture : CriteriaFixture
	{
		protected override void Configure(Configuration configuration)
		{
			TestConfiguration.Configure(configuration);
		}
	}
}
