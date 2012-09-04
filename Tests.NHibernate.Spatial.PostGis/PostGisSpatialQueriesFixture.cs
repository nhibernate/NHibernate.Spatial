using NUnit.Framework;
using NHibernate;
using NHibernate.Cfg;

namespace Tests.NHibernate.Spatial.RandomGeometries
{
	[TestFixture]
	public class PostGisSpatialQueriesFixture : SpatialQueriesFixture
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
WHERE the_geom && GeomFromText('{0}', 4326)
", filterString);
		}

		protected override string SqlPolygonFilter(string filterString)
		{
			return string.Format(@"
SELECT count(*) 
FROM polygontest 
WHERE the_geom && GeomFromText('{0}', 4326)
", filterString);
		}

		protected override string SqlMultiLineStringFilter(string filterString)
		{
			return string.Format(@"
SELECT count(*) 
FROM multilinestringtest 
WHERE the_geom && GeomFromText('{0}', 4326)
", filterString);
		}

		protected override string SqlOvelapsLineString(string filterString)
		{
			return string.Format(@"
SELECT count(*)
FROM linestringtest
WHERE the_geom IS NOT NULL
AND ST_Overlaps(PolygonFromText('{0}', 4326), the_geom)
", filterString);
		}

		protected override string SqlIntersectsLineString(string filterString)
		{
			return string.Format(@"
SELECT count(*)
FROM linestringtest
WHERE the_geom IS NOT NULL
AND ST_Intersects(PolygonFromText('{0}', 4326), the_geom)
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

	}
}
