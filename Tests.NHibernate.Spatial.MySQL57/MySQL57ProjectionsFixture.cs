using NHibernate.Cfg;
using NHibernate.Spatial.Type;
using NUnit.Framework;

namespace Tests.NHibernate.Spatial
{
    [TestFixture]
    public class MySQL57ProjectionsFixture : MySQLProjectionsFixture
    {
        protected override System.Type GeometryType => typeof(MySQL57GeometryType);

        protected override void Configure(Configuration configuration)
        {
            TestConfiguration.Configure(configuration);
        }
    }
}
