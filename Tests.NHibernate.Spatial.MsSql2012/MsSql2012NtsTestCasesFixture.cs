using NHibernate.Cfg;
using NUnit.Framework;

namespace Tests.NHibernate.Spatial
{
    [TestFixture]
    public class MsSql2012NtsTestCasesFixture : MsSql2008NtsTestCasesFixture
    {
        protected override void Configure(Configuration configuration)
        {
            TestConfiguration.Configure(configuration);
        }
    }
}
