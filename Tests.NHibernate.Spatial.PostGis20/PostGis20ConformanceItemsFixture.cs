using System.Linq;
using GeoAPI.Geometries;
using NHibernate.Cfg;
using NHibernate.Linq;
using NUnit.Framework;
using Tests.NHibernate.Spatial.OgcSfSql11Compliance.Model;

namespace Tests.NHibernate.Spatial
{
	[TestFixture]
	public class PostGis20ConformanceItemsFixture : PostGisConformanceItemsFixture
	{
		protected override void Configure(Configuration configuration)
		{
			TestConfiguration.Configure(configuration);
		}

        /// <summary>
		/// Conformance Item T48
		/// Difference(g1 Geometry, g2 Geometry) : Geometry
		/// For this test we will determine the difference between Ashton and
		/// Green Forest.
		///
		/// ANSWER: 'POLYGON( ( 56 34, 62 48, 84 48, 84 42, 56 34) )'
		/// NOTE: The order of the vertices here is arbitrary.
		///
		/// Original SQL:
		/// <code>
		///		SELECT Difference(named_places.boundary, forests.boundary)
		///		FROM named_places, forests
		///		WHERE named_places.name = 'Ashton' AND forests.name = 'Green Forest';
		/// </code>
		/// </summary>
		[Test]
        public override void ConformanceItemT48Hql()
        {
            string query =
                @"select NHSP.AsText(NHSP.Difference(np.Boundary, f.Boundary))
				from NamedPlace np, Forest f
				where np.Name = 'Ashton' and f.Name = 'Green Forest'
				";
            string result = session.CreateQuery(query)
                .UniqueResult<string>();

            IGeometry geometry = Wkt.Read(result);
            IGeometry expected = Wkt.Read("POLYGON ((62 48, 84 48, 84 42, 56 34, 62 48))");

            Assert.IsTrue(expected.EqualsExact(geometry, Tolerance));
        }

        [Test]
        public override void ConformanceItemT48Linq()
        {
            var query =
                from np in session.Query<NamedPlace>()
                from f in session.Query<Forest>()
                where np.Name == "Ashton" && f.Name == "Green Forest"
                select np.Boundary.Difference(f.Boundary);

            IGeometry geometry = query.Single();
            IGeometry expected = Wkt.Read("POLYGON ((62 48, 84 48, 84 42, 56 34, 62 48))");

            Assert.IsTrue(expected.EqualsExact(geometry, Tolerance));
        }

        /// <summary>
		/// Conformance Item T7
		/// GeometryType(g Geometry) : String
		/// For this test we will determine the type of Route 75.
		///
		/// ANSWER: 9 (which corresponds to 'MULTILINESTRING')
		///
		/// Original SQL:
		/// <code>
		///		SELECT GeometryType(centerlines)
		///		FROM lakes
		///		WHERE name = 'Route 75';
		/// </code>
		/// </summary>
		/// <remarks>
		/// Correction:
		/// * Table name should be DIVIDED_ROUTES instead of LAKES
		/// </remarks>
        [Test]
		public override void ConformanceItemT07Hql()
		{
			string query =
				@"select NHSP.GeometryType(t.Centerlines)
				from DividedRoute t
				where t.Name = 'Route 75'
				";
			string result = session.CreateQuery(query)
				.UniqueResult<string>();

			Assert.AreEqual("ST_MULTILINESTRING", result.ToUpper());
		}

		[Test]
		public override void ConformanceItemT07Linq()
		{
			var query =
				from t in session.Query<DividedRoute>()
				where t.Name == "Route 75"
				select t.Centerlines.GeometryType;

			string result = query.Single();

			Assert.AreEqual("ST_MULTILINESTRING", result.ToUpper());
		}
	}
}