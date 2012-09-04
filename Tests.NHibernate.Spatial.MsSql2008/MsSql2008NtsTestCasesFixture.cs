using System;
using NHibernate.Cfg;
using NHibernate.Criterion;
using NHibernate.Spatial.Criterion;
using NUnit.Framework;
using Tests.NHibernate.Spatial.NtsTestCases;
using Tests.NHibernate.Spatial.NtsTestCases.Model;

namespace Tests.NHibernate.Spatial
{
	[TestFixture]
    public class MsSql2008NtsTestCasesFixture : NtsTestCasesFixture
	{
		protected override void Configure(Configuration configuration)
		{
			TestConfiguration.Configure(configuration);
		}

		[Test]
		[Ignore("Not supported by MsSQL 2008")]
		public override void StringRelate()
		{
			base.StringRelate();
		}

		[Test]
		public void WhenRelateWithoutPatternThenThrows()
		{
			Assert.Throws<ArgumentNullException>(()=> 			session.CreateCriteria(typeof(NtsTestCase))
				.Add(Restrictions.Eq("Operation", "Relate"))
				.SetProjection(Projections.ProjectionList()
					.Add(Projections.Property("Description"))
					.Add(Projections.Property("RelatePattern"))
					.Add(SpatialProjections.Relate("GeometryA", "GeometryB"))
					)
				.List());
		}
	}
}
