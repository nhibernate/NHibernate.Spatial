using System;
using GeoAPI.Geometries;

namespace Tests.NHibernate.Spatial.OgcSfSql11Compliance.Model
{
	[Serializable]
	public class RoadSegment
	{
		public RoadSegment()
		{
		}

		public RoadSegment(long fid, string name, string aliases, int numLanes, IGeometry centerline)
		{
			this.Fid = fid;
			this.Name = name;
			this.Aliases = aliases;
			this.NumLanes = numLanes;
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

		private string aliases;
		public string Aliases
		{
			get { return aliases; }
			set { aliases = value; }
		}

		private int numLanes;
		public int NumLanes
		{
			get { return numLanes; }
			set { numLanes = value; }
		}

		private IGeometry centerline;
		public IGeometry Centerline
		{
			get { return centerline; }
			set { centerline = value; }
		}
	}
}
