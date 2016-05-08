using System.Linq;
using GeoAPI.Geometries;
using NHibernate.Cfg;
using NHibernate.Linq;
using NUnit.Framework;
using Tests.NHibernate.Spatial.OgcSfSql11Compliance;
using Tests.NHibernate.Spatial.OgcSfSql11Compliance.Model;

namespace Tests.NHibernate.Spatial
{
    [TestFixture]
    public class MySQL57ConformanceItemsFixture : MySQLConformanceItemsFixture
    {
        protected override void Configure(Configuration configuration)
        {
            TestConfiguration.Configure(configuration);
        }

        /// <summary>
        /// Overriden because GetPointN is not zero-based in MySQL
        /// </summary>
        [Test]
        public override void ConformanceItemT23Linq()
        {
            var query =
                from t in session.Query<RoadSegment>()
                where t.Fid == 102
                select ((ILineString)t.Centerline)
                .GetPointN(1);

            IGeometry geometry = query.Single();
            IGeometry expected = Wkt.Read("POINT( 0 18 )");

            Assert.IsTrue(expected.Equals(geometry));
        }

        /// <summary>
        /// Overriden because GetInteriorRingN is not zero-based in MySQL
        /// </summary>
        [Test]
        public override void ConformanceItemT29Linq()
        {
            var query =
                from t in session.Query<Lake>()
                where t.Name == "Blue Lake"
                select ((IPolygon)t.Shore).GetInteriorRingN(1);

            IGeometry geometry = query.Single();
            IGeometry expected = Wkt.Read("LINESTRING(59 18, 67 18, 67 13, 59 13, 59 18)");

            Assert.IsTrue(expected.Equals(geometry));
        }

        /// <summary>
        /// Overriden because GetGeometryN is not zero-based in MySQL
        /// </summary>
        [Test]
        public override void ConformanceItemT31Linq()
        {
            var query =
                from t in session.Query<DividedRoute>()
                where t.Name == "Route 75"
                select t.Centerlines.GetGeometryN(2);

            IGeometry geometry = query.Single();
            IGeometry expected = Wkt.Read("LINESTRING( 16 0, 16 23, 16 48 )");

            Assert.IsTrue(expected.Equals(geometry));
        }

        #region Features that are currently missing in MySQL 5.7

        [Test]
        public override void ConformanceItemT13Hql()
        {
            Assert.Ignore("Provider does not support Boundary function");
        }

        [Test]
        public override void ConformanceItemT13Linq()
        {
            Assert.Ignore("Provider does not support Boundary function");
        }

        [Test]
        public override void ConformanceItemT19Hql()
        {
            Assert.Ignore("Provider does not support Boundary function");
        }

        [Test]
        public override void ConformanceItemT19Linq()
        {
            Assert.Ignore("Provider does not support Boundary function");
        }

        [Test]
        public override void ConformanceItemT20Hql()
        {
            Assert.Ignore("Provider does not support IsRing function");
        }

        [Test]
        public override void ConformanceItemT20Linq()
        {
            Assert.Ignore("Provider does not support IsRing function");
        }

        [Test]
        public override void ConformanceItemT25Hql()
        {
            Assert.Ignore("Provider does not support IsRing function");
        }

        [Test]
        public override void ConformanceItemT25Linq()
        {
            Assert.Ignore("Provider does not support IsRing function");
        }

        [Test]
        public override void ConformanceItemT35Hql()
        {
            Assert.Ignore("Provider does not support PointOnSurface function");
        }

        [Test]
        public override void ConformanceItemT35Linq()
        {
            Assert.Ignore("Provider does not support PointOnSurface function");
        }
        [Test]
        public override void ConformanceItemT45Hql()
        {
            Assert.Ignore("Provider does not support Boundary function");
        }

        [Test]
        public override void ConformanceItemT45Linq()
        {
            Assert.Ignore("Provider does not support Boundary function");
        }


        #endregion
    }
}