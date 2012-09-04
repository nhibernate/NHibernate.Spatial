using NHibernate;
using NHibernate.Cfg;
using NUnit.Framework;
using Tests.NHibernate.Spatial.NtsTestCases;

namespace Tests.NHibernate.Spatial
{
	[TestFixture]
	public class PostGisNtsTestCasesFixture : NtsTestCasesFixture
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
		public override void Within()
		{
			PostGisTestsUtil.IgnoreIfAffectedByIssue22(this.postGisVersion);
			base.Within();
		}

		[Test]
		public override void IsValid()
		{
			PostGisTestsUtil.IgnoreIfAffectedByGEOSisvalidIssue(this.postGisVersion);
			base.IsValid();
		}
	}
}
