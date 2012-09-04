using System;
using GeoAPI.Geometries;

namespace Tests.NHibernate.Spatial.OgcSfSql11Compliance.Model
{
	[Serializable]
	public class DividedRoute
	{
		public DividedRoute()
		{
		}

		public DividedRoute(long fid, string name, int numLanes, IGeometry centerlines)
		{
			this.Fid = fid;
			this.Name = name;
			this.NumLanes = numLanes;
			this.Centerlines = centerlines;
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

		private int numLanes;
		public int NumLanes
		{
			get { return numLanes; }
			set { numLanes = value; }
		}

		private IGeometry centerlines;
		public IGeometry Centerlines
		{
			get { return centerlines; }
			set { centerlines = value; }
		}

	}
}
