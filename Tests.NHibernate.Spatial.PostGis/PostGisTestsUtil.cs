using NHibernate;
using NUnit.Framework;

namespace Tests.NHibernate.Spatial
{
	public class PostGisTestsUtil
	{

		public static string GetPostGisVersion(ISessionFactory sessionFactory)
		{
			using (ISession session = sessionFactory.OpenSession())
			{
				return (string)session
					.CreateSQLQuery("SELECT postgis_lib_version()")
					.UniqueResult();
			}
		}

		/// <summary>
		/// Ignores test if affected by PostGIS issue 22
		/// http://code.google.com/p/postgis/issues/detail?id=22
		/// </summary>
		public static void IgnoreIfAffectedByIssue22(string version)
		{
			if (version == "1.3.3")
			{
				Assert.Ignore("Affected by known bug in PostGIS {0}", version);
			}
		}

		/// <summary>
		/// Ignores test if affected by PostGIS issue:
		/// "ERROR: XX000: GEOS isvalid() threw an error!"
		/// </summary>
		/// <param name="version"></param>
		public static void IgnoreIfAffectedByGEOSisvalidIssue(string version)
		{
			if (version == "1.2.1")
			{
				Assert.Ignore("Affected by known bug in PostGIS {0}", version);
			}
		}
	}
}