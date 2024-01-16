using NHibernate.Cfg;
using NUnit.Framework;

namespace Tests.NHibernate.Spatial
{
    [TestFixture]
    public class MsSql2012ProjectionsFixture : ProjectionsFixture
    {
        [Test]
        [Ignore("MsSql2012 does not support Intersection spatial aggregate function")]
        public override void IntersectionAll()
        { }

        protected override void Configure(Configuration config)
        {
            TestConfiguration.Configure(config);
        }
    }
}
