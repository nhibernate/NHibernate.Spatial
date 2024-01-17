using NHibernate.Cfg;
using NHibernate.Spatial.Criterion;
using NUnit.Framework;
using Tests.NHibernate.Spatial.NtsTestCases;

namespace Tests.NHibernate.Spatial
{
    [TestFixture]
    public class MySQL57NtsTestCasesFixture : NtsTestCasesFixture
    {
        [Test]
        public override void IsValid()
        {
            TestBooleanUnaryOperation("IsValid", SpatialProjections.IsValid, SpatialRestrictions.IsValid);
        }

        protected override void Configure(Configuration configuration)
        {
            TestConfiguration.Configure(configuration);
        }

        #region Unsupported features

        [Test]
        [Ignore("Provider does not support the Relate function")]
        public override void BooleanRelate()
        { }

        [Test]
        [Ignore("Provider does not support the CoveredBy function")]
        public override void CoveredBy()
        { }

        [Test]
        [Ignore("Provider does not support the Covers function")]
        public override void Covers()
        { }

        [Test]
        [Ignore("Provider does not support the Relate function")]
        public override void StringRelate()
        { }

        #endregion
    }
}
