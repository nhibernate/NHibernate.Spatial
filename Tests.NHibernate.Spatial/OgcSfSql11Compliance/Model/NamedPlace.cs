using NetTopologySuite.Geometries;
using System;

namespace Tests.NHibernate.Spatial.OgcSfSql11Compliance.Model
{
    [Serializable]
    public class NamedPlace
    {
        public NamedPlace()
        {
        }

        public NamedPlace(long fid, string name, Geometry boundary)
        {
            this.Fid = fid;
            this.Name = name;
            this.Boundary = boundary;
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

        private Geometry boundary;

        public Geometry Boundary
        {
            get { return boundary; }
            set { boundary = value; }
        }
    }
}