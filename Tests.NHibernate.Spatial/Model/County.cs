using System;
using GeoAPI.Geometries;

namespace Tests.NHibernate.Spatial.Model
{
	[Serializable]
	public class County
	{
		public County()
		{
		}

		public County(string name, string state, IGeometry boundaries)
		{
			this.Name = name;
			this.State = state;
			this.Boundaries = boundaries;
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


		private string state;
		public virtual string State
		{
			get { return state; }
			set { state = value; }
		}

		private IGeometry boundaries;
		public virtual IGeometry Boundaries
		{
			get { return boundaries; }
			set { boundaries = value; }
		}
	}
}
