using System.Collections;
using NHibernate;
using NHibernate.Cfg;
using NUnit.Framework;
using Tests.NHibernate.Spatial.RandomGeometries;

namespace Tests.NHibernate.Spatial
{
    [TestFixture]
    public class PostGis20SpatialQueriesFixture : SpatialQueriesFixture
    {
        protected override void Configure(Configuration configuration)
        {
            TestConfiguration.Configure(configuration);
        }

        protected override string SqlLineStringFilter(string filterString)
        {
            return string.Format(@"
                SELECT count(*)
                FROM linestringtest
                WHERE the_geom && ST_GeomFromText('{0}', 4326)
                ", filterString);
        }

        protected override string SqlPolygonFilter(string filterString)
        {
            return string.Format(@"
                SELECT count(*)
                FROM polygontest
                WHERE the_geom && ST_GeomFromText('{0}', 4326)
                ", filterString);
        }

        protected override string SqlMultiLineStringFilter(string filterString)
        {
            return string.Format(@"
                SELECT count(*)
                FROM multilinestringtest
                WHERE the_geom && ST_GeomFromText('{0}', 4326)
                ", filterString);
        }

        protected override string SqlOvelapsLineString(string filterString)
        {
            return string.Format(@"
                SELECT count(*)
                FROM linestringtest
                WHERE the_geom IS NOT NULL
                AND ST_Overlaps(ST_PolygonFromText('{0}', 4326), the_geom)
                ", filterString);
        }

        protected override string SqlIntersectsLineString(string filterString)
        {
            return string.Format(@"
                SELECT count(*)
                FROM linestringtest
                WHERE the_geom IS NOT NULL
                AND ST_Intersects(ST_PolygonFromText('{0}', 4326), the_geom)
                ", filterString);
        }

        protected override ISQLQuery SqlIsEmptyLineString(ISession session)
        {
            return session.CreateSQLQuery(@"
                SELECT ST_IsEmpty(the_geom) as result
                FROM linestringtest
                WHERE oid = ?
                AND the_geom IS NOT NULL
                ")
                .AddScalar("result", NHibernateUtil.Boolean);
        }

        protected override ISQLQuery SqlIsSimpleLineString(ISession session)
        {
            return session.CreateSQLQuery(@"
                SELECT ST_IsSimple(the_geom) as result
                FROM linestringtest
                WHERE oid = ?
                AND the_geom IS NOT NULL
                ")
                .AddScalar("result", NHibernateUtil.Boolean);
        }

        protected override ISQLQuery SqlAsBinaryLineString(ISession session)
        {
            return session.CreateSQLQuery(@"
                SELECT ST_AsBinary(the_geom) as result
                FROM linestringtest
                WHERE oid = ?
                AND the_geom IS NOT NULL
                ")
                .AddScalar("result", NHibernateUtil.BinaryBlob);
        }

        [Test]
        public override void HqlGeometryType()
        {
            IList results = Session
                .CreateQuery(
                    "select NHSP.GeometryType(l.Geometry) from LineStringEntity as l where l.Geometry is not null")
                .List();

            foreach (object item in results)
            {
                var gt = (string)item;
                Assert.AreEqual("ST_LINESTRING", gt.ToUpper());
            }

            results = Session
                .CreateQuery("select NHSP.GeometryType(p.Geometry) from PolygonEntity as p where p.Geometry is not null")
                .List();

            foreach (object item in results)
            {
                var gt = (string)item;
                Assert.AreEqual("ST_POLYGON", gt.ToUpper());
            }
        }
    }
}