using System.Collections;
using NHibernate.Cfg;
using NUnit.Framework;

namespace Tests.NHibernate.Spatial
{
	[TestFixture]
	public class PostGis20SpatialQueriesFixture : PostGisSpatialQueriesFixture
	{
		protected override void Configure(Configuration configuration)
		{
			TestConfiguration.Configure(configuration);
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