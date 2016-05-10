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
    public class PostGisConformanceItemsFixture : ConformanceItemsFixture
    {
        protected override void Configure(Configuration configuration)
        {
            TestConfiguration.Configure(configuration);
        }

        private string postGisVersion;

        protected override void OnTestFixtureSetUp()
        {
            this.postGisVersion = PostGisTestsUtil.GetPostGisVersion(this.sessions);
            base.OnTestFixtureSetUp();
        }


        /// <summary>
        /// Overriden because GetPointN is not zero-based in PostGis 2.0
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
        /// Overriden because GetInteriorRingN is not zero-based in PostGis 2.0
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
        /// Overriden because GetGeometryN is not zero-based in PostGis 2.0
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


        [Test]
        public override void ConformanceItemT40Hql()
        {
            PostGisTestsUtil.IgnoreIfAffectedByIssue22(this.postGisVersion);
            base.ConformanceItemT40Hql();
        }

        [Test]
        public override void ConformanceItemT40Linq()
        {
            PostGisTestsUtil.IgnoreIfAffectedByIssue22(this.postGisVersion);
            base.ConformanceItemT40Linq();
        }

        [Test]
        public override void ConformanceItemT51Hql()
        {
            PostGisTestsUtil.IgnoreIfAffectedByIssue22(this.postGisVersion);
            base.ConformanceItemT51Hql();
        }

        [Test]
        public override void ConformanceItemT51Linq()
        {
            PostGisTestsUtil.IgnoreIfAffectedByIssue22(this.postGisVersion);
            base.ConformanceItemT51Linq();
        }
    }
}