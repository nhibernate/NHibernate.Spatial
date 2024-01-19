using System;
using NHibernate.Cfg;
using NUnit.Framework;
using Tests.NHibernate.Spatial.NtsTestCases;
using Tests.NHibernate.Spatial.NtsTestCases.Model;

namespace Tests.NHibernate.Spatial
{
    [TestFixture]
    public class MySQL80NtsTestCasesFixture : NtsTestCasesFixture
    {
        protected override Type[] Mappings
        {
            get
            {
                return new[]
                {
                    typeof(NtsTestCase)
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
        [Ignore("Provider does not support the Relate function")]
        public override void StringRelate()
        { }

        #endregion
    }
}
