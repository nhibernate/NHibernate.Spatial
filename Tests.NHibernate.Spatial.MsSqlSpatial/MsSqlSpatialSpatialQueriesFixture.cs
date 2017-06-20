using NHibernate.Cfg;
using NUnit.Framework;
using Tests.NHibernate.Spatial.RandomGeometries;

namespace Tests.NHibernate.Spatial
{
    [TestFixture]
    public class MsSqlSpatialSpatialQueriesFixture : MsSql2008SpatialQueriesFixture
    {
        protected override void Configure(Configuration configuration)
        {
            TestConfiguration.Configure(configuration);
        }
    }
}
