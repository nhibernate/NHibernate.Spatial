using System;
using GeoAPI.Geometries;

namespace Tests.NHibernate.Spatial.Model
{
	[Serializable]
	public class Simple
	{
		public Simple()
		{
		}

		public Simple(string description, IGeometry geometry)
		{
			this.Description = description;
			this.Geometry = geometry;
		}

		private long id;
		public virtual long Id
		{
			get { return id; }
			set { id = value; }
		}

		private string description;
		public virtual string Description
		{
			get { return description; }
			set { description = value; }
		}

		private IGeometry geometry;
		public virtual IGeometry Geometry
		{
			get { return geometry; }
			set { geometry = value; }
		}
	}
}
