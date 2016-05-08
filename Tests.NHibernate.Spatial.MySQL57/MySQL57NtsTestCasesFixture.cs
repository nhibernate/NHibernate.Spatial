using NHibernate.Cfg;
using NUnit.Framework;
using Tests.NHibernate.Spatial.NtsTestCases;

namespace Tests.NHibernate.Spatial
{
    [TestFixture]
    public class MySQL57NtsTestCasesFixture : MySQLNtsTestCasesFixture
    {
        protected override void Configure(Configuration configuration)
        {
            TestConfiguration.Configure(configuration);
        }

        #region Unsupported features

        [Test]
        public override void BooleanRelate()
        {
            Assert.Ignore("Provider does not support the Relate function");
        }

        [Test]
        public override void CoveredBy()
        {
            Assert.Ignore("Provider does not support the Relate function");
        }
        [Test]
        public override void Covers()
        {
            Assert.Ignore("Provider does not support the Relate function");
        }
        [Test]
        public override void IsValid()
        {
            Assert.Ignore("Provider does not support the IsValid function");
        }
        [Test]
        public override void StringRelate()
        {
            Assert.Ignore("Provider does not support the Relate function");
        }

        #endregion
    }
}