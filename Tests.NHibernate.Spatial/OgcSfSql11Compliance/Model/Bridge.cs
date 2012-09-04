using System;
using GeoAPI.Geometries;

namespace Tests.NHibernate.Spatial.OgcSfSql11Compliance.Model
{
	[Serializable]
	public class Bridge
	{
		public Bridge()
		{
		}

		public Bridge(long fid, string name, IGeometry position)
		{
			this.Fid = fid;
			this.Name = name;
			this.Position = position;
		}

		private long fid;
		public long Fid
		{
			get { return fid; }
			set { fid = value; }
		}

		private string name;
		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		private IGeometry position;
		public IGeometry Position
		{
			get { return position; }
			set { position = value; }
		}
	}
}
