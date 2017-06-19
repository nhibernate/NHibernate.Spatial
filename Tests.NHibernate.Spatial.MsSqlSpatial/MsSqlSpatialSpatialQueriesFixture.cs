using NHibernate.Cfg;
using NUnit.Framework;

namespace Tests.NHibernate.Spatial.RandomGeometries
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
