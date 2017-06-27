using NHibernate.Cfg;
using NHibernate.Spatial.Type;
using NUnit.Framework;

namespace Tests.NHibernate.Spatial
{
    [TestFixture]
    public class MsSql2012ProjectionsFixture : ProjectionsFixture
    {
        protected override void Configure(Configuration config)
        {
            TestConfiguration.Configure(config);
        }
    }
}
