using NHibernate.Cfg;
using NHibernate.Spatial.Type;
using NUnit.Framework;

namespace Tests.NHibernate.Spatial
{
    [TestFixture]
    public class MsSql2008ProjectionsFixture : ProjectionsFixture
    {
        protected override void Configure(Configuration config)
        {
            TestConfiguration.Configure(config);
        }

        protected override System.Type GeometryType
        {
            get { return typeof(MsSqlGeometryType); }
        }

        [Test]
        [Ignore("MsSql2008 does not support spatial aggregate functions")]
        public override void CollectAll()
        { }

        [Test]
        [Ignore("MsSql2008 does not support spatial aggregate functions")]
        public override void CountAndUnion()
        { }

        [Test]
        [Ignore("MsSql2008 does not support spatial aggregate functions")]
        public override void CountAndUnionByState()
        { }

        [Test]
        [Ignore("MsSql2008 does not support spatial aggregate functions")]
        public override void CountAndUnionByStateLambda()
        { }

        [Test]
        [Ignore("MsSql2008 does not support spatial aggregate functions")]
        public override void EnvelopeAll()
        { }

        [Test]
        [Ignore("MsSql2008 does not support spatial aggregate functions")]
        public override void IntersectionAll()
        { }
    }
}
