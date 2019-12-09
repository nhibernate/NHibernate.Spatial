using NetTopologySuite.Geometries;
using System;

namespace Tests.NHibernate.Spatial.OgcSfSql11Compliance.Model
{
    [Serializable]
    public class Stream
    {
        public Stream()
        {
        }

        public Stream(long fid, string name, Geometry centerline)
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

        private Geometry centerline;

        public Geometry Centerline
        {
            get { return centerline; }
            set { centerline = value; }
        }
    }
}