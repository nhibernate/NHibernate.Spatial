using System;
using NHibernate.Cfg;
using NHibernate.Spatial.Dialect;
using NUnit.Framework;
using Tests.NHibernate.Spatial.RandomGeometries;
using Tests.NHibernate.Spatial.RandomGeometries.Model;

namespace Tests.NHibernate.Spatial
{
    [TestFixture]
    public class MySQL80SpatialQueriesFixture : MySQL57SpatialQueriesFixture
    {
        protected override Type[] Mappings
        {
            get
            {
                return new[]
                {
                    typeof(LineStringEntity),
                    typeof(MultiLineStringEntity),
                    typeof(MultiPointEntity),
                    typeof(MultiPolygonEntity),
                    typeof(PointEntity),
                    typeof(PolygonEntity),
                };
            }
        }

        [Test]
        public override void HqlSRID()
        {
            var results = Session
                .CreateQuery("select NHSP.SRID(l.Geometry) from LineStringEntity as l where l.Geometry is not null")
                .List();

            foreach (object item in results)
            {
                int srid = (int) item;
                Assert.AreEqual(0, srid);
            }
        }

        protected override string SqlLineStringFilter(string filterString)
        {
            return $@"
SELECT count(*)
FROM linestringtest
WHERE MBRIntersects(the_geom, {SpatialDialect.IsoPrefix}GeomFromText('{filterString}', 0))
";
        }

        protected override string SqlPolygonFilter(string filterString)
        {
            return $@"
SELECT count(*)
FROM polygontest
WHERE MBRIntersects(the_geom, {SpatialDialect.IsoPrefix}GeomFromText('{filterString}', 0))
";
        }

        protected override string SqlMultiLineStringFilter(string filterString)
        {
            return $@"
SELECT count(*)
FROM multilinestringtest
WHERE MBRIntersects(the_geom, {SpatialDialect.IsoPrefix}GeomFromText('{filterString}', 0))
";
        }

        protected override string SqlOvelapsLineString(string filterString)
        {
            return $@"
SELECT count(*)
FROM linestringtest
WHERE the_geom IS NOT NULL
AND {SpatialDialect.IsoPrefix}Overlaps({SpatialDialect.IsoPrefix}PolygonFromText('{filterString}', 0), the_geom)
";
        }

        protected override string SqlIntersectsLineString(string filterString)
        {
            return $@"
SELECT count(*)
FROM linestringtest
WHERE the_geom IS NOT NULL
AND {SpatialDialect.IsoPrefix}Intersects({SpatialDialect.IsoPrefix}PolygonFromText('{filterString}', 0), the_geom)
";
        }

        protected override void Configure(Configuration configuration)
        {
            TestConfiguration.Configure(configuration);
        }

        protected override void OnTestFixtureSetUp()
        {
            const int srid = 0;
            DataGenerator.Generate(sessions, srid);

            Filter = Wkt.Read(FilterString);
            Filter.SRID = srid;
        }
    }
}
