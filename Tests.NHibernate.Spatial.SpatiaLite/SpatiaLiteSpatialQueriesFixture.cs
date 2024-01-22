using NHibernate;
using NHibernate.Cfg;
using NUnit.Framework;
using Tests.NHibernate.Spatial.RandomGeometries;

namespace Tests.NHibernate.Spatial
{
    [TestFixture]
    public class SpatiaLiteSpatialQueriesFixture : SpatialQueriesFixture
    {
        protected override void Configure(Configuration configuration)
        {
            TestConfiguration.Configure(configuration);
        }

        protected override string SqlLineStringFilter(string filterString)
        {
            return $@"
                SELECT count(*)
                FROM linestringtest
                WHERE MbrIntersects(the_geom, ST_GeomFromText('{filterString}', 4326))
                ";
        }

        protected override string SqlPolygonFilter(string filterString)
        {
            return $@"
                SELECT count(*)
                FROM polygontest
                WHERE MbrIntersects(the_geom, ST_GeomFromText('{filterString}', 4326))
                ";
        }

        protected override string SqlMultiLineStringFilter(string filterString)
        {
            return $@"
                SELECT count(*)
                FROM multilinestringtest
                WHERE MbrIntersects(the_geom, ST_GeomFromText('{filterString}', 4326))
                ";
        }

        protected override string SqlOvelapsLineString(string filterString)
        {
            return $@"
                SELECT count(*)
                FROM linestringtest
                WHERE the_geom IS NOT NULL
                AND ST_Overlaps(ST_PolygonFromText('{filterString}', 4326), the_geom)
                ";
        }

        protected override string SqlIntersectsLineString(string filterString)
        {
            return $@"
                SELECT count(*)
                FROM linestringtest
                WHERE the_geom IS NOT NULL
                AND ST_Intersects(ST_PolygonFromText('{filterString}', 4326), the_geom)
                ";
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
    }
}
