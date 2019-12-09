using NetTopologySuite.Geometries;
using NHibernate.Cfg;
using NUnit.Framework;
using System.Linq;
using Tests.NHibernate.Spatial.OgcSfSql11Compliance;
using Tests.NHibernate.Spatial.OgcSfSql11Compliance.Model;

namespace Tests.NHibernate.Spatial
{
    [TestFixture]
    public class MsSql2008ConformanceItemsFixture : ConformanceItemsFixture
    {
        protected override void Configure(Configuration configuration)
        {
            TestConfiguration.Configure(configuration);
        }

        /// <summary>
        /// Overridden because GetPointN is not zero-based in MS SQL Server
        /// </summary>
        [Test]
        public override void ConformanceItemT23Linq()
        {
            var query =
                from t in session.Query<RoadSegment>()
                where t.Fid == 102
                select ((LineString)t.Centerline)
                .GetPointN(1);

            Geometry geometry = query.Single();
            Geometry expected = Wkt.Read("POINT( 0 18 )");

            Assert.IsTrue(expected.EqualsTopologically(geometry));
        }

        /// <summary>
        /// Overridden because GetInteriorRingN is not zero-based in MS SQL Server
        /// </summary>
        [Test]
        public override void ConformanceItemT29Linq()
        {
            var query =
                from t in session.Query<Lake>()
                where t.Name == "Blue Lake"
                select ((Polygon)t.Shore).GetInteriorRingN(1);

            Geometry geometry = query.Single();
            Geometry expected = Wkt.Read("LINESTRING(59 18, 67 18, 67 13, 59 13, 59 18)");

            Assert.IsTrue(expected.EqualsTopologically(geometry));
        }

        /// <summary>
        /// Overridden because GetGeometryN is not zero-based in MS SQL Server
        /// </summary>
        [Test]
        public override void ConformanceItemT31Linq()
        {
            var query =
                from t in session.Query<DividedRoute>()
                where t.Name == "Route 75"
                select t.Centerlines.GetGeometryN(2);

            Geometry geometry = query.Single();
            Geometry expected = Wkt.Read("LINESTRING( 16 0, 16 23, 16 48 )");

            Assert.IsTrue(expected.EqualsTopologically(geometry));
        }

        /// <summary>
        /// Overridden because polygon points returned in different order in MS SQL Server
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

            Geometry geometry = Wkt.Read(result);
            Geometry expected = Wkt.Read("POLYGON((56 34, 84 42, 84 48, 62 48, 56 34))");

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

            Geometry geometry = query.Single();
            Geometry expected = Wkt.Read("POLYGON((56 34, 84 42, 84 48, 62 48, 56 34))");

            Assert.IsTrue(expected.EqualsExact(geometry, Tolerance));
        }
    }
}
