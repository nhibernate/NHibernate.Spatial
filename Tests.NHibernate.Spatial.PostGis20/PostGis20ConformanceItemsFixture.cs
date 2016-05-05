using System.Linq;
using NHibernate.Cfg;
using NHibernate.Linq;
using NUnit.Framework;
using Tests.NHibernate.Spatial.OgcSfSql11Compliance;
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