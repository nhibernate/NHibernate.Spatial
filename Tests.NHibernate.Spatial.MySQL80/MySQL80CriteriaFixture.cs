using NHibernate.Cfg;
using NUnit.Framework;

namespace Tests.NHibernate.Spatial
{
    [TestFixture]
    public class MySQL80CriteriaFixture : MySQL57CriteriaFixture
    {
        protected override void Configure(Configuration configuration)
        {
            TestConfiguration.Configure(configuration);
        }
    }
}
