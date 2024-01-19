using NHibernate.Cfg;
using NUnit.Framework;

namespace Tests.NHibernate.Spatial
{
    [TestFixture]
    public class MySQL80ConformanceItemsFixture : MySQL57ConformanceItemsFixture
    {
        protected override string SpatialReferenceSystemWKT => @"PROJCS[""WGS 72 / UTM zone 14N"",GEOGCS[""WGS 72"",DATUM[""World Geodetic System 1972"",SPHEROID[""WGS 72"",6378135,298.26,AUTHORITY[""EPSG"",""7043""]],TOWGS84[0,0,4.5,0,0,0.554,0.219],AUTHORITY[""EPSG"",""6322""]],PRIMEM[""Greenwich"",0,AUTHORITY[""EPSG"",""8901""]],UNIT[""degree"",0.017453292519943278,AUTHORITY[""EPSG"",""9122""]],AXIS[""Lat"",NORTH],AXIS[""Lon"",EAST],AUTHORITY[""EPSG"",""4322""]],PROJECTION[""Transverse Mercator"",AUTHORITY[""EPSG"",""9807""]],PARAMETER[""Latitude of natural origin"",0,AUTHORITY[""EPSG"",""8801""]],PARAMETER[""Longitude of natural origin"",-99,AUTHORITY[""EPSG"",""8802""]],PARAMETER[""Scale factor at natural origin"",0.9996,AUTHORITY[""EPSG"",""8805""]],PARAMETER[""False easting"",500000,AUTHORITY[""EPSG"",""8806""]],PARAMETER[""False northing"",0,AUTHORITY[""EPSG"",""8807""]],UNIT[""metre"",1,AUTHORITY[""EPSG"",""9001""]],AXIS[""E"",EAST],AXIS[""N"",NORTH],AUTHORITY[""EPSG"",""32214""]]";

        [Test]
        [Ignore("The ST_GEOMETRY_COLUMNS view in MySQL 8 does not include geometry dimension")]
        public override void ConformanceItemT03Hql()
        { }

        [Test]
        [Ignore("The ST_GEOMETRY_COLUMNS view in MySQL 8 does not include geometry dimension")]
        public override void ConformanceItemT03Linq()
        { }

        protected override void Configure(Configuration configuration)
        {
            TestConfiguration.Configure(configuration);
        }

        protected override void OnBeforeCreateSchema()
        {
            using (var session = sessions.OpenSession())
            {
                var query = session.CreateSQLQuery("CREATE SPATIAL REFERENCE SYSTEM :srs_id NAME 'Test' ORGANIZATION :org_name IDENTIFIED BY :org_srs_id DEFINITION :definition")
                    .SetParameter("srs_id", 101)
                    .SetParameter("org_name", "POSC")
                    .SetParameter("org_srs_id", 32214)
                    .SetParameter("definition", SpatialReferenceSystemWKT);

                query.ExecuteUpdate();
            }
        }

        protected override void OnAfterDropSchema()
        {
            using (var session = sessions.OpenSession())
            {
                var query = session.CreateSQLQuery("DROP SPATIAL REFERENCE SYSTEM :srs_id")
                    .SetParameter("srs_id", 101);

                query.ExecuteUpdate();
            }
        }
    }
}
