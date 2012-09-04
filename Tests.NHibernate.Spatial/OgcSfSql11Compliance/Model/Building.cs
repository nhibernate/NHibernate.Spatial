using System;
using GeoAPI.Geometries;

namespace Tests.NHibernate.Spatial.OgcSfSql11Compliance.Model
{
	[Serializable]
	public class Building
	{
		public Building()
		{
		}

		public Building(long fid, string address, IGeometry position, IGeometry footprint)
		{
			this.Fid = fid;
			this.Address = address;
			this.Position = position;
			this.Footprint = footprint;
		}

		private long fid;
		public long Fid
		{
			get { return fid; }
			set { fid = value; }
		}

		private string address;
		public string Address
		{
			get { return address; }
			set { address = value; }
		}

		private IGeometry position;
		public IGeometry Position
		{
			get { return position; }
			set { position = value; }
		}

		private IGeometry footprint;
		public IGeometry Footprint
		{
			get { return footprint; }
			set { footprint = value; }
		}
	}
}
