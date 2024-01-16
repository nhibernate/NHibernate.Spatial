using NetTopologySuite.Geometries;
using System;

namespace Tests.NHibernate.Spatial.OgcSfSql11Compliance.Model
{
    [Serializable]
    public class Forest
    {
        private long fid;

        private string name;

        private Geometry boundary;

        public Forest()
        { }

        public Forest(long fid, string name, Geometry boundary)
        {
            Fid = fid;
            Name = name;
            Boundary = boundary;
        }

        public long Fid
        {
            get => fid;
            set => fid = value;
        }

        public string Name
        {
            get => name;
            set => name = value;
        }

        public Geometry Boundary
        {
            get => boundary;
            set => boundary = value;
        }
    }
}
