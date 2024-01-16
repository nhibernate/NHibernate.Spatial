using NHibernate;
using NHibernate.Cfg;
using NUnit.Framework;
using Tests.NHibernate.Spatial.RandomGeometries;

namespace Tests.NHibernate.Spatial
{
    [TestFixture]
    public class MsSql2008SpatialQueriesFixture : SpatialQueriesFixture
    {
        protected override void Configure(Configuration configuration)
        {
            TestConfiguration.Configure(configuration);
        }

        protected override string SqlLineStringFilter(string filterString)
        {
            return $@"
SELECT count_big(*)
FROM linestringtest
WHERE the_geom.Filter(geometry::STGeomFromText('{filterString}', 4326)) = 1
";
        }

        protected override string SqlPolygonFilter(string filterString)
        {
            return $@"
SELECT count_big(*)
FROM polygontest
WHERE the_geom.Filter(geometry::STGeomFromText('{filterString}', 4326)) = 1
";
        }

        protected override string SqlMultiLineStringFilter(string filterString)
        {
            return $@"
SELECT count_big(*)
FROM multilinestringtest
WHERE the_geom.Filter(geometry::STGeomFromText('{filterString}', 4326)) = 1
";
        }

        protected override string SqlOvelapsLineString(string filterString)
        {
            return $@"
SELECT count_big(*)
FROM linestringtest
WHERE the_geom IS NOT NULL
AND the_geom.STOverlaps(geometry::STPolyFromText('{filterString}', 4326)) = 1
";
        }

        protected override string SqlIntersectsLineString(string filterString)
        {
            return $@"
SELECT count_big(*)
FROM linestringtest
WHERE the_geom IS NOT NULL
AND the_geom.STIntersects(geometry::STPolyFromText('{filterString}', 4326)) = 1
";
        }

        protected override ISQLQuery SqlIsEmptyLineString(ISession session)
        {
            return session.CreateSQLQuery(@"
SELECT the_geom.STIsEmpty() as result
FROM linestringtest
WHERE oid = ?
AND the_geom IS NOT NULL
")
                .AddScalar("result", NHibernateUtil.Boolean);
        }

        protected override ISQLQuery SqlIsSimpleLineString(ISession session)
        {
            return session.CreateSQLQuery(@"
SELECT the_geom.STIsSimple() as result
FROM linestringtest
WHERE oid = ?
AND the_geom IS NOT NULL
")
                .AddScalar("result", NHibernateUtil.Boolean);
        }

        protected override ISQLQuery SqlAsBinaryLineString(ISession session)
        {
            return session.CreateSQLQuery(@"
SELECT the_geom.STAsBinary() as result
FROM linestringtest
WHERE oid = ?
AND the_geom IS NOT NULL
")
                .AddScalar("result", NHibernateUtil.BinaryBlob);
        }
    }
}
