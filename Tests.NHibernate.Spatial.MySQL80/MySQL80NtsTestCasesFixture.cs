using System;
using System.IO;
using NHibernate.Cfg;
using NUnit.Framework;
using Tests.NHibernate.Spatial.NtsTestCases;
using Tests.NHibernate.Spatial.NtsTestCases.Model;

namespace Tests.NHibernate.Spatial
{
    [TestFixture]
    public class MySQL80NtsTestCasesFixture : NtsTestCasesFixture
    {
        private const string LocalDataPath = "../../../../Tests.NHibernate.Spatial.MySQL80/NtsTestCases/Data";

        protected override string TestRelateACValidateDataPath => Path.Combine(LocalDataPath, "validate", "TestRelateAC.xml");

        protected override string TestRelateLAValidateDataPath => Path.Combine(LocalDataPath, "validate", "TestRelateLA.xml");

        protected override Type[] Mappings
        {
            get
            {
                return new[]
                {
#pragma warning disable 0436
                    typeof(NtsTestCase)
#pragma warning restore 0436
                };
            }
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
        [Ignore("Provider does not support the EqualsExact function")]
        public override void EqualsExact()
        { }

        [Test]
        [Ignore("Provider does not support the Relate function")]
        public override void StringRelate()
        { }

        #endregion
    }
}
