using NHibernate.Cfg;
using NUnit.Framework;
using System.IO;
using Tests.NHibernate.Spatial.NtsTestCases;

namespace Tests.NHibernate.Spatial
{
    [TestFixture]
    public class MySQL57NtsTestCasesFixture : NtsTestCasesFixture
    {
        private const string LocalDataPath = "../../../../Tests.NHibernate.Spatial.MySQL57/NtsTestCases/Data/vivid";

        protected override string TestFunctionAAPrecDataPath => Path.Combine(LocalDataPath, "TestFunctionAAPrec.xml");

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
