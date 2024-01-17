﻿using NHibernate.Cfg;
using NHibernate.Criterion;
using NHibernate.Spatial.Criterion;
using NUnit.Framework;
using System;
using System.IO;
using Tests.NHibernate.Spatial.NtsTestCases;
using Tests.NHibernate.Spatial.NtsTestCases.Model;

namespace Tests.NHibernate.Spatial
{
    [TestFixture]
    public class MsSql2008NtsTestCasesFixture : NtsTestCasesFixture
    {
        private const string LocalDataPath = @"../../../../Tests.NHibernate.Spatial.MsSql2008/NtsTestCases/Data/vivid";

        protected override string TestSimpleDataPath => Path.Combine(LocalDataPath, @"TestSimple.xml");

        [Test]
        [Ignore("Not supported by MS SQL")]
        public override void StringRelate()
        {
            base.StringRelate();
        }

        [Test]
        public void WhenRelateWithoutPatternThenThrows()
        {
            Assert.Throws<ArgumentNullException>(() => _session.CreateCriteria(typeof(NtsTestCase))
                .Add(Restrictions.Eq("Operation", "Relate"))
                .SetProjection(Projections.ProjectionList()
                    .Add(Projections.Property("Description"))
                    .Add(Projections.Property("RelatePattern"))
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
