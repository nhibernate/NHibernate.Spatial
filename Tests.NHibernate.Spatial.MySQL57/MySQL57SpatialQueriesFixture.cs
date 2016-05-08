using NHibernate;
using NHibernate.Cfg;
using NUnit.Framework;
using Tests.NHibernate.Spatial.RandomGeometries;

namespace Tests.NHibernate.Spatial
{
    [TestFixture]
    public class MySQL57SpatialQueriesFixture : MySQLSpatialQueriesFixture
    {
        protected override void Configure(Configuration configuration)
        {
            TestConfiguration.Configure(configuration);
        }

        #region Unsupported features

        [Test]
        public override void HqlBoundary()
        {
            Assert.Ignore("Provider does not support the Boundary function");
        }

        [Test]
        public override void HqlRelateLineString()
        {
            Assert.Ignore("Provider does not support the Relate function");
        }

        #endregion
    }
}