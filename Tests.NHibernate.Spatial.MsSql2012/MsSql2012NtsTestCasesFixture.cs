using System;
using System.IO;
using NHibernate.Cfg;
using NHibernate.Criterion;
using NHibernate.Spatial.Criterion;
using NUnit.Framework;
using Tests.NHibernate.Spatial.NtsTestCases;
using Tests.NHibernate.Spatial.NtsTestCases.Model;

namespace Tests.NHibernate.Spatial
{
    [TestFixture]
    public class MsSql2012NtsTestCasesFixture : NtsTestCasesFixture
    {
        private const string LocalDataPath = "../../../../Tests.NHibernate.Spatial.MsSql2012/NtsTestCases/Data";

        protected override string TestRelateAADataPath => Path.Combine(LocalDataPath, "general", "TestRelateAA.xml");

        protected override string TestRelateACDataPath => Path.Combine(LocalDataPath, "general", "TestRelateAC.xml");

        protected override string TestSimpleDataPath => Path.Combine(LocalDataPath, "general", "TestSimple.xml");

        protected override string TestRelateACValidateDataPath => Path.Combine(LocalDataPath, "validate", "TestRelateAC.xml");

        protected override string TestRelateLAValidateDataPath => Path.Combine(LocalDataPath, "validate", "TestRelateLA.xml");

        [Test]
        [Ignore("Not supported by SQL Server")]
        public override void EqualsExact()
        { }

        [Test]
        [Ignore("Not supported by SQL Server")]
        public override void StringRelate()
        { }

        [Test]
        public void WhenRelateWithoutPatternThenThrows()
        {
            Assert.Throws<ArgumentNullException>(() => _session.CreateCriteria(typeof(NtsTestCase))
                .Add(Restrictions.Eq("Operation", "Relate"))
                .SetProjection(Projections.ProjectionList()
                    .Add(Projections.Property("Description"))
                    .Add(Projections.Property("Parameter"))
                    .Add(SpatialProjections.Relate("GeometryA", "GeometryB"))
                )
                .List());
        }

        protected override void Configure(Configuration configuration)
        {
            TestConfiguration.Configure(configuration);
        }
    }
}
