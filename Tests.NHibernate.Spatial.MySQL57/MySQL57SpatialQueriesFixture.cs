using NHibernate;
using NHibernate.Cfg;
using NHibernate.Spatial.Dialect;
using NUnit.Framework;
using Tests.NHibernate.Spatial.RandomGeometries;

namespace Tests.NHibernate.Spatial
{
    [TestFixture]
    public class MySQL57SpatialQueriesFixture : SpatialQueriesFixture
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
WHERE MBRIntersects(the_geom, GeomFromText('{filterString}', 4326))
";
        }

        protected override string SqlPolygonFilter(string filterString)
        {
            return $@"
SELECT count(*)
FROM polygontest
WHERE MBRIntersects(the_geom, GeomFromText('{filterString}', 4326))
";
        }

        protected override string SqlMultiLineStringFilter(string filterString)
        {
            return $@"
SELECT count(*)
FROM multilinestringtest
WHERE MBRIntersects(the_geom, GeomFromText('{filterString}', 4326))
";
        }

        protected override string SqlOvelapsLineString(string filterString)
        {
            return $@"
SELECT count(*)
FROM linestringtest
WHERE the_geom IS NOT NULL
AND {SpatialDialect.IsoPrefix}Overlaps(PolygonFromText('{filterString}', 4326), the_geom)
";
        }

        protected override string SqlIntersectsLineString(string filterString)
        {
            return $@"
SELECT count(*)
FROM linestringtest
WHERE the_geom IS NOT NULL
AND {SpatialDialect.IsoPrefix}Intersects(PolygonFromText('{filterString}', 4326), the_geom)
";
        }

        protected override ISQLQuery SqlIsEmptyLineString(ISession session)
        {
            return session.CreateSQLQuery($@"
SELECT {SpatialDialect.IsoPrefix}IsEmpty(the_geom) as result
FROM linestringtest
WHERE oid = ?
AND the_geom IS NOT NULL
")
                .AddScalar("result", NHibernateUtil.Boolean);
        }

        protected override ISQLQuery SqlIsSimpleLineString(ISession session)
        {
            return session.CreateSQLQuery($@"
SELECT {SpatialDialect.IsoPrefix}IsSimple(the_geom) as result
FROM linestringtest
WHERE oid = ?
AND the_geom IS NOT NULL
")
                .AddScalar("result", NHibernateUtil.Boolean);
        }

        protected override ISQLQuery SqlAsBinaryLineString(ISession session)
        {
            return session.CreateSQLQuery($@"
SELECT {SpatialDialect.IsoPrefix}AsBinary(the_geom) as result
FROM linestringtest
WHERE oid = ?
AND the_geom IS NOT NULL
")
                .AddScalar("result", NHibernateUtil.BinaryBlob);
        }

        #region Unsupported features

        [Test]
        [Ignore("Provider does not support the Boundary function")]
        public override void HqlBoundary()
        { }

        [Test]
        [Ignore("Provider does not support the Relate function")]
        public override void HqlRelateLineString()
        { }

        #endregion
    }
}
