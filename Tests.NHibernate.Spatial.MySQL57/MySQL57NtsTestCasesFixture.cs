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
    }
}