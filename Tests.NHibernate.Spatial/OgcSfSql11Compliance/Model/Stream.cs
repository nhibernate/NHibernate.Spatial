using NetTopologySuite.Geometries;
using System;

namespace Tests.NHibernate.Spatial.OgcSfSql11Compliance.Model
{
    [Serializable]
    public class Stream
    {
        private long fid;

        private string name;

        private Geometry centerline;

        public Stream()
        { }

        public Stream(long fid, string name, Geometry centerline)
        {
            Fid = fid;
            Name = name;
            Centerline = centerline;
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

        public Geometry Centerline
        {
            get => centerline;
            set => centerline = value;
        }
    }
}
