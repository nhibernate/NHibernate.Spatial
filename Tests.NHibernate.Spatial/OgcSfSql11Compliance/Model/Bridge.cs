using NetTopologySuite.Geometries;
using System;

namespace Tests.NHibernate.Spatial.OgcSfSql11Compliance.Model
{
    [Serializable]
    public class Bridge
    {
        private long fid;

        private string name;

        private Geometry position;

        public Bridge()
        { }

        public Bridge(long fid, string name, Geometry position)
        {
            Fid = fid;
            Name = name;
            Position = position;
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

        public Geometry Position
        {
            get => position;
            set => position = value;
        }
    }
}
