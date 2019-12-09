using NetTopologySuite.Geometries;
using System;

namespace Tests.NHibernate.Spatial.OgcSfSql11Compliance.Model
{
    [Serializable]
    public class Building
    {
        public Building()
        {
        }

        public Building(long fid, string address, Geometry position, Geometry footprint)
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

        private Geometry position;

        public Geometry Position
        {
            get { return position; }
            set { position = value; }
        }

        private Geometry footprint;

        public Geometry Footprint
        {
            get { return footprint; }
            set { footprint = value; }
        }
    }
}