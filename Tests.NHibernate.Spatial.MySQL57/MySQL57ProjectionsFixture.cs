using NHibernate.Cfg;
using NUnit.Framework;

namespace Tests.NHibernate.Spatial
{
    [TestFixture]
    public class MySQL57ProjectionsFixture : ProjectionsFixture
    {
        [Test]
        [Ignore("MySQL does not support Union spatial aggregate function")]
        public override void CountAndUnion()
        { }

        [Test]
        [Ignore("MySQL does not support Union spatial aggregate function")]
        public override void CountAndUnionByState()
        { }

        [Test]
        [Ignore("MySQL does not support Union spatial aggregate function")]
        public override void CountAndUnionByStateLambda()
        { }

        [Test]
        [Ignore("MySQL does not support Intersection spatial aggregate function")]
        public override void IntersectionAll()
        { }

        protected override void Configure(Configuration configuration)
        {
            TestConfiguration.Configure(configuration);
        }
    }
}
