using NetTopologySuite.Geometries;
using System;

namespace Tests.NHibernate.Spatial.OgcSfSql11Compliance.Model
{
    [Serializable]
    public class Building
    {
        private long fid;

        private string address;

        private Geometry position;

        private Geometry footprint;

        public Building()
        { }

        public Building(long fid, string address, Geometry position, Geometry footprint)
        {
            Fid = fid;
            Address = address;
            Position = position;
            Footprint = footprint;
        }

        public long Fid
        {
            get => fid;
            set => fid = value;
        }

        public string Address
        {
            get => address;
            set => address = value;
        }

        public Geometry Position
        {
            get => position;
            set => position = value;
        }

        public Geometry Footprint
        {
            get => footprint;
            set => footprint = value;
        }
    }
}
