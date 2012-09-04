using System;
using NUnit.Framework;
using NHibernate;
using NHibernate.Cfg;
using Tests.NHibernate.Spatial.NtsTestCases;
using Tests.NHibernate.Spatial.OgcSfSql11Compliance;

namespace Tests.NHibernate.Spatial.RandomGeometries
{
	[TestFixture]
	public class MsSqlSpatialSpatialQueriesFixture : SpatialQueriesFixture
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
WHERE oid in (
	SELECT oid 
	FROM ST.FilterQuery('linestringtest', 'the_geom', ST.GeomFromText('{0}', 4326))
)
", filterString);
		}

		protected override string SqlPolygonFilter(string filterString)
		{
			return string.Format(@"
SELECT count_big(*) 
FROM polygontest 
WHERE oid in (
	SELECT oid 
	FROM ST.FilterQuery('polygontest', 'the_geom', ST.GeomFromText('{0}', 4326))
)
", filterString);
		}

		protected override string SqlMultiLineStringFilter(string filterString)
		{
			return string.Format(@"
SELECT count_big(*) 
FROM multilinestringtest 
WHERE oid in (
	SELECT oid 
	FROM ST.FilterQuery('multilinestringtest', 'the_geom', ST.GeomFromText('{0}', 4326))
)
", filterString);
		}

		protected override string SqlOvelapsLineString(string filterString)
		{
			return string.Format(@"
SELECT count_big(*)
FROM linestringtest
WHERE the_geom IS NOT NULL
AND ST.Overlaps(ST.PolygonFromText('{0}', 4326), the_geom) = 1
", filterString);
		}

		protected override string SqlIntersectsLineString(string filterString)
		{
			return string.Format(@"
SELECT count_big(*)
FROM linestringtest
WHERE the_geom IS NOT NULL
AND ST.Intersects(ST.PolygonFromText('{0}', 4326), the_geom) = 1
", filterString);
		}

		protected override ISQLQuery SqlIsEmptyLineString(ISession session)
		{
			return session.CreateSQLQuery(@"
SELECT ST.IsEmpty(the_geom) as result
FROM linestringtest
WHERE oid = ?
AND the_geom IS NOT NULL
")
		.AddScalar("result", NHibernateUtil.Boolean);
		}

		protected override ISQLQuery SqlIsSimpleLineString(ISession session)
		{
			return session.CreateSQLQuery(@"
SELECT ST.IsSimple(the_geom) as result
FROM linestringtest
WHERE oid = ?
AND the_geom IS NOT NULL
")
		.AddScalar("result", NHibernateUtil.Boolean);
		}

		protected override ISQLQuery SqlAsBinaryLineString(ISession session)
		{
			return session.CreateSQLQuery(@"
SELECT ST.AsBinary(the_geom) as result
FROM linestringtest
WHERE oid = ?
AND the_geom IS NOT NULL
")
		.AddScalar("result", NHibernateUtil.BinaryBlob);
		}

	}
}