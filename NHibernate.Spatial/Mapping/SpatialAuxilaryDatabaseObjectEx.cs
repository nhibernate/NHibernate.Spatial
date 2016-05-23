using NHibernate.Cfg;
using NHibernate.Mapping;
using NHibernate.Spatial.Type;
using NHibernate.Type;
using System;
using System.Linq;

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
			return SetSpatialAuxilaryObject(cfg, sao => { });
		}

		public static Configuration SetSpatialAuxilaryObject(this Configuration cfg, Action<SpatialAuxiliaryDatabaseObject> map)
		{
			var obj = new SpatialAuxiliaryDatabaseObject(cfg);
			map(obj);
			cfg.AddAuxiliaryDatabaseObject(obj);
			return cfg;
		}

		public static Action<SpatialAuxiliaryDatabaseObject> GetSridMapperFor<TEntity>(this Configuration cfg, string propertyName, int srid)
		{
			PersistentClass targetClass = cfg.GetClassMapping(typeof(TEntity));
			var prop = targetClass.GetProperty(propertyName);
			var column = (Column)prop.ColumnIterator.First();
			if (((CustomType)column.Value.Type).UserType is IGeometryUserType)
			{
				return x => x.SetSRID(targetClass.Table, column, srid);
			}
			return x => { };
		}
	}
}