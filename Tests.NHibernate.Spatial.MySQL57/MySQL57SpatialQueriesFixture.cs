using NHibernate;
using NHibernate.Cfg;
using NUnit.Framework;
using Tests.NHibernate.Spatial.RandomGeometries;

namespace Tests.NHibernate.Spatial
{
    [TestFixture]
    public class MySQL57SpatialQueriesFixture : MySQLSpatialQueriesFixture
    {
        protected override void Configure(Configuration configuration)
        {
            TestConfiguration.Configure(configuration);
        }
    }
}