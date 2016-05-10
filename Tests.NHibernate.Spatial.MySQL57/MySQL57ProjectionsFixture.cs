using NHibernate.Cfg;
using NHibernate.Spatial.Type;
using NUnit.Framework;

namespace Tests.NHibernate.Spatial
{
    [TestFixture]
    public class MySQL57ProjectionsFixture : MySQLProjectionsFixture
    {
        protected override void Configure(Configuration configuration)
        {
            TestConfiguration.Configure(configuration);
        }

        protected override System.Type GeometryType
        {
            get { return typeof(MySQL57GeometryType); }
        }
    }
}