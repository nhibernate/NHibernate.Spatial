using NetTopologySuite.Geometries;
using System;

namespace Tests.NHibernate.Spatial.OgcSfSql11Compliance.Model
{
    [Serializable]
    public class MapNeatline
    {
        private long fid;

        private Geometry neatline;

        public MapNeatline()
        { }

        public MapNeatline(long fid, Geometry neatline)
        {
            Fid = fid;
            Neatline = neatline;
        }

        public long Fid
        {
            get => fid;
            set => fid = value;
        }

        public Geometry Neatline
        {
            get => neatline;
            set => neatline = value;
        }
    }
}
