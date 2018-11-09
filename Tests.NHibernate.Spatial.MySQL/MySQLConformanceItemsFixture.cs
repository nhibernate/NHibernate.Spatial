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
    public class MySQLConformanceItemsFixture : ConformanceItemsFixture
    {
        protected override void Configure(Configuration configuration)
        {
            TestConfiguration.Configure(configuration);
        }

        /// <summary>
        /// Overridden because GetPointN is not zero-based in MySQL
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

            Assert.IsTrue(expected.EqualsTopologically(geometry));
        }

        /// <summary>
        /// Overridden because GetInteriorRingN is not zero-based in MySQL
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

            Assert.IsTrue(expected.EqualsTopologically(geometry));
        }

        /// <summary>
        /// Overridden because GetGeometryN is not zero-based in MySQL
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

            Assert.IsTrue(expected.EqualsTopologically(geometry));
        }

        /// <summary>
        /// Overridden because polygon vertices are returned in a different order in MySQL
        /// </summary>
        [Test]
        public override void ConformanceItemT48Hql()
        {
            string query =
                @"select NHSP.AsText(NHSP.Difference(np.Boundary, f.Boundary))
				from NamedPlace np, Forest f
				where np.Name = 'Ashton' and f.Name = 'Green Forest'
				";
            string result = session.CreateQuery(query)
                .UniqueResult<string>();

            IGeometry geometry = Wkt.Read(result);
            IGeometry expected = Wkt.Read("POLYGON((84 42, 84 48, 62 48, 56 34, 84 42))");

            Assert.IsTrue(expected.EqualsExact(geometry, Tolerance));
        }

        [Test]
        public override void ConformanceItemT48Linq()
        {
            var query =
                from np in session.Query<NamedPlace>()
                from f in session.Query<Forest>()
                where np.Name == "Ashton" && f.Name == "Green Forest"
                select np.Boundary.Difference(f.Boundary);

            IGeometry geometry = query.Single();
            IGeometry expected = Wkt.Read("POLYGON((84 42, 84 48, 62 48, 56 34, 84 42))");

            Assert.IsTrue(expected.EqualsExact(geometry, Tolerance));
        }

        #region Features that are currently missing in MySQL

        [Test]
        [Ignore("Provider does not support Boundary function")]
        public override void ConformanceItemT13Hql()
        { }

        [Test]
        [Ignore("Provider does not support Boundary function")]
        public override void ConformanceItemT13Linq()
        { }

        [Test]
        [Ignore("Provider does not support Boundary function")]
        public override void ConformanceItemT19Hql()
        { }

        [Test]
        [Ignore("Provider does not support Boundary function")]
        public override void ConformanceItemT19Linq()
        { }

        [Test]
        [Ignore("Provider does not support IsRing function")]
        public override void ConformanceItemT20Hql()
        { }

        [Test]
        [Ignore("Provider does not support IsRing function")]
        public override void ConformanceItemT20Linq()
        { }

        [Test]
        [Ignore("Provider does not support IsRing function")]
        public override void ConformanceItemT25Hql()
        { }

        [Test]
        [Ignore("Provider does not support IsRing function")]
        public override void ConformanceItemT25Linq()
        { }

        [Test]
        [Ignore("Provider does not support PointOnSurface function")]
        public override void ConformanceItemT35Hql()
        { }

        [Test]
        [Ignore("Provider does not support PointOnSurface function")]
        public override void ConformanceItemT35Linq()
        { }

        [Test]
        [Ignore("Provider does not support Boundary function")]
        public override void ConformanceItemT45Hql()
        { }

        [Test]
        [Ignore("Provider does not support Boundary function")]
        public override void ConformanceItemT45Linq()
        { }

        #endregion
    }
}
