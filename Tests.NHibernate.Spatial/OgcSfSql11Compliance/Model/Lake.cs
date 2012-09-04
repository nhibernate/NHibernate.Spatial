using System;
using GeoAPI.Geometries;

namespace Tests.NHibernate.Spatial.OgcSfSql11Compliance.Model
{
	[Serializable]
	public class Lake
	{
		public Lake()
		{
		}

		public Lake(long fid, string name, IGeometry shore)
		{
			this.Fid = fid;
			this.Name = name;
			this.Shore = shore;
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

		private IGeometry shore;
		public IGeometry Shore
		{
			get { return shore; }
			set { shore = value; }
		}
	}
}
