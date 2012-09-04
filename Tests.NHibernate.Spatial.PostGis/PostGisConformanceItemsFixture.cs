using NHibernate;
using NHibernate.Cfg;
using NUnit.Framework;
using Tests.NHibernate.Spatial.OgcSfSql11Compliance;

namespace Tests.NHibernate.Spatial
{
	[TestFixture]
	public class PostGisConformanceItemsFixture : ConformanceItemsFixture
	{
		protected override void Configure(Configuration configuration)
		{
			TestConfiguration.Configure(configuration);
		}

		private string postGisVersion;

		protected override void OnTestFixtureSetUp()
		{
			this.postGisVersion = PostGisTestsUtil.GetPostGisVersion(this.sessions);
			base.OnTestFixtureSetUp();
		}

		[Test]
		public override void ConformanceItemT40Hql()
		{
			PostGisTestsUtil.IgnoreIfAffectedByIssue22(this.postGisVersion);
			base.ConformanceItemT40Hql();
		}

		[Test]
		public override void ConformanceItemT40Linq()
		{
			PostGisTestsUtil.IgnoreIfAffectedByIssue22(this.postGisVersion);
			base.ConformanceItemT40Linq();
		}

		[Test]
		public override void ConformanceItemT51Hql()
		{
			PostGisTestsUtil.IgnoreIfAffectedByIssue22(this.postGisVersion);
			base.ConformanceItemT51Hql();
		}

		[Test]
		public override void ConformanceItemT51Linq()
		{
			PostGisTestsUtil.IgnoreIfAffectedByIssue22(this.postGisVersion);
			base.ConformanceItemT51Linq();
		}
	}
}
