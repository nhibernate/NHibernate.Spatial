using System;
using GeoAPI.Geometries;

namespace Tests.NHibernate.Spatial.OgcSfSql11Compliance.Model
{
	[Serializable]
	public class Stream
	{
		public Stream()
		{
		}

		public Stream(long fid, string name, IGeometry centerline)
		{
			this.Fid = fid;
			this.Name = name;
			this.Centerline = centerline;
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

		private IGeometry centerline;
		public IGeometry Centerline
		{
			get { return centerline; }
			set { centerline = value; }
		}
	}
}
