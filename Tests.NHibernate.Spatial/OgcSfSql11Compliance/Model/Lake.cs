using NetTopologySuite.Geometries;
using System;

namespace Tests.NHibernate.Spatial.OgcSfSql11Compliance.Model
{
    [Serializable]
    public class Lake
    {
        public Lake()
        {
        }

        public Lake(long fid, string name, Geometry shore)
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

        private Geometry shore;

        public Geometry Shore
        {
            get { return shore; }
            set { shore = value; }
        }
    }
}