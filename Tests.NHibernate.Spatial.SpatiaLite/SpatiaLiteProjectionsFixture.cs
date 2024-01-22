using NHibernate.Cfg;
using NUnit.Framework;

namespace Tests.NHibernate.Spatial
{
    [TestFixture]
    public class SpatiaLiteProjectionsFixture : ProjectionsFixture
    {
        [Test]
        [Ignore("SpatiaLite does not support Intersection spatial aggregate function")]
        public override void IntersectionAll()
        { }

        protected override void Configure(Configuration config)
        {
            TestConfiguration.Configure(config);
        }
    }
}
