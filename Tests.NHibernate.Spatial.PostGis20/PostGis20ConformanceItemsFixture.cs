using System.Linq;
using NetTopologySuite.Geometries;
using NHibernate.Cfg;
using NUnit.Framework;
using Tests.NHibernate.Spatial.OgcSfSql11Compliance;
using Tests.NHibernate.Spatial.OgcSfSql11Compliance.Model;

namespace Tests.NHibernate.Spatial
{
	[TestFixture]
	public class PostGis20ConformanceItemsFixture : ConformanceItemsFixture
	{
		protected override void Configure(Configuration configuration)
		{
			TestConfiguration.Configure(configuration);
		}

        /// <summary>
        /// Overridden because GeometryType includes ISO prefix in PostGIS 2
        /// </summary>
        [Test]
        public override void ConformanceItemT07Hql()
        {
            string query =
                @"select NHSP.GeometryType(t.Centerlines)
				from DividedRoute t
				where t.Name = 'Route 75'
				";
            string result = session.CreateQuery(query)
                .UniqueResult<string>();

            Assert.AreEqual("ST_MULTILINESTRING", result.ToUpper());
        }

        [Test]
        public override void ConformanceItemT07Linq()
        {
            var query =
                from t in session.Query<DividedRoute>()
                where t.Name == "Route 75"
                select t.Centerlines.GeometryType;

            string result = query.Single();

            Assert.AreEqual("ST_MULTILINESTRING", result.ToUpper());
        }

        /// <summary>
        /// Overridden because GetPointN is not zero-based in PostGIS
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
        /// Overridden because GetInteriorRingN is not zero-based in PostGIS
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
        /// Overridden because GetGeometryN is not zero-based in PostGIS
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
    }
}
