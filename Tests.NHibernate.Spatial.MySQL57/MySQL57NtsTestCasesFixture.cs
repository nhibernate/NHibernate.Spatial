using NHibernate.Cfg;
using NHibernate.Spatial.Criterion;
using NUnit.Framework;

namespace Tests.NHibernate.Spatial
{
    [TestFixture]
    public class MySQL57NtsTestCasesFixture : MySQLNtsTestCasesFixture
    {
        protected override void Configure(Configuration configuration)
        {
            TestConfiguration.Configure(configuration);
        }
        
        [Test]
        public override void IsValid()
        {
            TestBooleanUnaryOperation("IsValid", SpatialProjections.IsValid, SpatialRestrictions.IsValid);
        }
    }
}
