using NHibernate.Cfg;
using NUnit.Framework;

namespace Tests.NHibernate.Spatial
{
    [TestFixture]
    public class MySQL57CriteriaFixture : MySQLCriteriaFixture
    {
        protected override void Configure(Configuration configuration)
        {
            TestConfiguration.Configure(configuration);
        }

        [Test]
        [Ignore("Empty geometry collections not supported by MySql.Data.Types.MySqlGeometry")]
        public override void CountNull()
        { }

        [Test]
        [Ignore("Empty geometry collections not supported by MySql.Data.Types.MySqlGeometry")]
        public override void CountSpatialEmpty()
        { }

        [Test]
        [Ignore("Empty geometry collections not supported by MySql.Data.Types.MySqlGeometry")]
        public override void IsDirty()
        { }
    }
}
