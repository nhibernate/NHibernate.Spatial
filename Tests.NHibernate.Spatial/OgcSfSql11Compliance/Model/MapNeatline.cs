using System;
using GeoAPI.Geometries;

namespace Tests.NHibernate.Spatial.OgcSfSql11Compliance.Model
{
	[Serializable]
	public class MapNeatline
	{
		public MapNeatline()
		{
		}

		public MapNeatline(long fid, IGeometry neatline)
		{
			this.Fid = fid;
			this.Neatline = neatline;
		}

		private long fid;
		public long Fid
		{
			get { return fid; }
			set { fid = value; }
		}

		private IGeometry neatline;
		public IGeometry Neatline
		{
			get { return neatline; }
			set { neatline = value; }
		}
	}
}
