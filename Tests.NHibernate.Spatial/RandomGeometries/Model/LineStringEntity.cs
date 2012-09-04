using System;
using GeoAPI.Geometries;

namespace Tests.NHibernate.Spatial.RandomGeometries.Model
{
	[Serializable]
	public class LineStringEntity
	{
		public LineStringEntity()
		{
		}

		public LineStringEntity(string name, IGeometry geometry)
		{
			this.Name = name;
			this.Geometry = geometry;
		}

		public LineStringEntity(long id, string name, IGeometry geometry)
		{
			this.Id = id;
			this.Name = name;
			this.Geometry = geometry;
		}

		private long id;
		public virtual long Id
		{
			get { return id; }
			set { id = value; }
		}

		private string name;
		public virtual string Name
		{
			get { return name; }
			set { name = value; }
		}

		private IGeometry geometry;
		public virtual IGeometry Geometry
		{
			get { return geometry; }
			set { geometry = value; }
		}
	}
}
