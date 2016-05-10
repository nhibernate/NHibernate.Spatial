using NHibernate.Cfg;

namespace NHibernate.Spatial.Mapping
{
	public static class SpatialAuxilaryDatabaseObjectEx
	{
		/// <summary>
		/// Helper for the spatial Auxilary Object
		/// </summary>
		/// <param name="cfg"></param>
		/// <returns></returns>
		public static Configuration SetSpatialAuxilaryObject(this Configuration cfg)
		{
			var obj = new SpatialAuxiliaryDatabaseObject(cfg);
			cfg.AddAuxiliaryDatabaseObject(obj);
			return cfg;
		}
	}
}