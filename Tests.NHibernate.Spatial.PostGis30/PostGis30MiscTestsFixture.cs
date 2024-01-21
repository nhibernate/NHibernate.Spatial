using NHibernate.Cfg;
using NUnit.Framework;

namespace Tests.NHibernate.Spatial
{
    [TestFixture]
    public class PostGis30MiscTestsFixture : PostGis20MiscTestsFixture
    {
        protected override void Configure(Configuration config)
        {
            TestConfiguration.Configure(config);
        }
    }
}
