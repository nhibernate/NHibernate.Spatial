﻿using NHibernate.Cfg;
using NUnit.Framework;
using Tests.NHibernate.Spatial.NtsTestCases;

namespace Tests.NHibernate.Spatial
{
    [TestFixture]
    public class MsSqlSpatialNtsTestCasesFixture : NtsTestCasesFixture
    {
        protected override void Configure(Configuration configuration)
        {
            TestConfiguration.Configure(configuration);
        }

        [Test]
        [Ignore("Not supported by MsSqlSpatial")]
        public override void StringRelate()
        {
            base.StringRelate();
        }
    }
}
