using NHibernate.Cfg;
using NHibernate.Spatial.Type;
using NUnit.Framework;

namespace Tests.NHibernate.Spatial
{
	[TestFixture]
    public class MsSql2008ProjectionsFixture : ProjectionsFixture
	{
		protected override void Configure(Configuration config)
		{
			TestConfiguration.Configure(config);
		}

        protected override System.Type GeometryType
        {
            get { return typeof (MsSql2008GeometryType); }
        }
	}
}
