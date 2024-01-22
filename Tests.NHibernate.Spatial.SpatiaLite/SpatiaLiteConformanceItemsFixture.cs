using System.Linq;
using NetTopologySuite.Geometries;
using NHibernate.Cfg;
using NHibernate.Spatial.Metadata;
using NUnit.Framework;
using Tests.NHibernate.Spatial.OgcSfSql11Compliance;
using Tests.NHibernate.Spatial.OgcSfSql11Compliance.Model;

namespace Tests.NHibernate.Spatial
{
    [TestFixture]
    public class SpatiaLiteConformanceItemsFixture : ConformanceItemsFixture
    {
        [Test]
        public override void ConformanceItemT05Hql()
        {
            if (!Metadata.SupportsSpatialMetadata(session, MetadataClass.SpatialReferenceSystem))
            {
                Assert.Ignore("Provider does not support spatial metadata");
            }
            var srs = session.CreateQuery(
                    "from SpatiaLiteSpatialReferenceSystem where SRID=101")
                .UniqueResult<SpatialReferenceSystem>();

            Assert.IsNotNull(srs);
            Assert.AreEqual(SpatialReferenceSystemWKT, srs.WellKnownText);

            // Alternative syntax for identifiers:
            srs = session.CreateQuery(
                    "from SpatiaLiteSpatialReferenceSystem s where s=101")
                .UniqueResult<SpatiaLiteSpatialReferenceSystem>();

            Assert.IsNotNull(srs);
            Assert.AreEqual(SpatialReferenceSystemWKT, srs.WellKnownText);
        }

        /// <summary>
        /// Overridden because GetPointN is not zero-based in SpatiaLite
        /// </summary>
        [Test]
        public override void ConformanceItemT23Linq()
        {
            var query =
                from t in session.Query<RoadSegment>()
                where t.Fid == 102
                select ((LineString) t.Centerline)
                    .GetPointN(1);

            Geometry geometry = query.Single();
            var expected = Wkt.Read("POINT( 0 18 )");

            Assert.IsTrue(expected.EqualsTopologically(geometry));
        }

        /// <summary>
        /// Overridden because GetPointN is not zero-based in SpatiaLite
        /// </summary>
        [Test]
        public override void ConformanceItemT29Linq()
        {
            var query =
                from t in session.Query<Lake>()
                where t.Name == "Blue Lake"
                select ((Polygon) t.Shore).GetInteriorRingN(1);

            Geometry geometry = query.Single();
            var expected = Wkt.Read("LINESTRING(59 18, 67 18, 67 13, 59 13, 59 18)");

            Assert.IsTrue(expected.EqualsTopologically(geometry));
        }

        /// <summary>
        /// Overridden because GetPointN is not zero-based in SpatiaLite
        /// </summary>
        [Test]
        public override void ConformanceItemT31Linq()
        {
            var query =
                from t in session.Query<DividedRoute>()
                where t.Name == "Route 75"
                select t.Centerlines.GetGeometryN(2);

            var geometry = query.Single();
            var expected = Wkt.Read("LINESTRING( 16 0, 16 23, 16 48 )");

            Assert.IsTrue(expected.EqualsTopologically(geometry));
        }

        protected override void Configure(Configuration configuration)
        {
            TestConfiguration.Configure(configuration);
        }

        protected override void OnBeforeCreateSchema()
        {
            using (var session = sessions.OpenSession())
            {
                session.Save(new SpatiaLiteSpatialReferenceSystem(101, "POSC", 32214, "NHSP", "PROJ.4", SpatialReferenceSystemWKT));
                session.Flush();
            }
        }

        protected override void OnAfterDropSchema()
        {
            using (var session = sessions.OpenSession())
            {
                session.Delete("from SpatiaLiteSpatialReferenceSystem where SRID=101");
                session.Flush();
            }
        }
    }
}
