using NUnit.Framework;
using NHibernate;
using NHibernate.Cfg;

namespace Tests.NHibernate.Spatial.RandomGeometries
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
			return string.Format(@"
SELECT count_big(*) 
FROM linestringtest 
WHERE the_geom.Filter(geometry::STGeomFromText('{0}', 4326)) = 1
", filterString);
		}

		protected override string SqlPolygonFilter(string filterString)
		{
			return string.Format(@"
SELECT count_big(*) 
FROM polygontest 
WHERE the_geom.Filter(geometry::STGeomFromText('{0}', 4326)) = 1
", filterString);
		}

		protected override string SqlMultiLineStringFilter(string filterString)
		{
			return string.Format(@"
SELECT count_big(*) 
FROM multilinestringtest 
WHERE the_geom.Filter(geometry::STGeomFromText('{0}', 4326)) = 1
", filterString);
		}

		protected override string SqlOvelapsLineString(string filterString)
		{
			return string.Format(@"
SELECT count_big(*)
FROM linestringtest
WHERE the_geom IS NOT NULL
AND the_geom.STOverlaps(geometry::STPolyFromText('{0}', 4326)) = 1
", filterString);
		}

		protected override string SqlIntersectsLineString(string filterString)
		{
			return string.Format(@"
SELECT count_big(*)
FROM linestringtest
WHERE the_geom IS NOT NULL
AND the_geom.STIntersects(geometry::STPolyFromText('{0}', 4326)) = 1
", filterString);
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