using System;
using System.IO;
using NHibernate.Cfg;
using NUnit.Framework;
using Tests.NHibernate.Spatial.NtsTestCases;
using Tests.NHibernate.Spatial.NtsTestCases.Model;

namespace Tests.NHibernate.Spatial
{
    [TestFixture]
    public class SpatiaLiteNtsTestCasesFixture : NtsTestCasesFixture
    {
        private const string LocalDataPath = "../../../../Tests.NHibernate.Spatial.SpatiaLite/NtsTestCases/Data";

        protected override string TestSimpleDataPath => Path.Combine(LocalDataPath, "general", "TestSimple.xml");

        protected override string TestValidDataPath => Path.Combine(LocalDataPath, "general", "TestValid.xml");

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

        [Test]
        [Ignore("Not supported by SpatiaLite")]
        public override void EqualsExact()
        { }

        [Test]
        [Ignore("Not supported by SpatiaLite")]
        public override void StringRelate()
        { }

        protected override void Configure(Configuration configuration)
        {
            TestConfiguration.Configure(configuration);
        }
    }
}
