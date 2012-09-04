using NetTopologySuite.IO;
using NUnit.Framework;
using NHibernate;
using NHibernate.Cfg;
using Tests.NHibernate.Spatial.RandomGeometries.Model;

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


        /// <summary>
        /// NHSP-18: Line geometry insertion fails if line contains only two idantical points
        /// </summary>
        [Test]
        [ExpectedException(typeof(System.FormatException))]
        public void NHSP_18()
        {
            var geometry = new WKTReader().Read("LINESTRING(5 5, 5 5)");
            geometry.SRID = 4326;
            var entity = new LineStringEntity { Id = int.MaxValue, Name = "test", Geometry = geometry };
            using (var session = sessions.OpenSession())
            {
                session.Transaction.Begin();
                session.Save(entity);
                session.Flush();
                session.Transaction.Rollback();
            }
        }

	}
}