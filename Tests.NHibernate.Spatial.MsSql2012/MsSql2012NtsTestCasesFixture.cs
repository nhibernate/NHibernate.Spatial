using System.IO;
using NHibernate.Cfg;
using NUnit.Framework;

namespace Tests.NHibernate.Spatial
{
    [TestFixture]
    public class MsSql2012NtsTestCasesFixture : MsSql2008NtsTestCasesFixture
    {
        private const string LocalDataPath = @"../../../../Tests.NHibernate.Spatial.MsSql2012/NtsTestCases/Data/vivid";

        protected override string TestRelateAADataPath => Path.Combine(LocalDataPath, @"TestRelateAA.xml");

        protected override string TestRelateACDataPath => Path.Combine(LocalDataPath, @"TestRelateAC.xml");

        protected override void Configure(Configuration configuration)
        {
            TestConfiguration.Configure(configuration);
        }
    }
}
