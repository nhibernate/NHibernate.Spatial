using NHibernate.Cfg;
using NHibernate.Spatial.Criterion;
using NUnit.Framework;

namespace Tests.NHibernate.Spatial
{
    [TestFixture]
    public class MySQL57NtsTestCasesFixture : MySQLNtsTestCasesFixture
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
    }
}
