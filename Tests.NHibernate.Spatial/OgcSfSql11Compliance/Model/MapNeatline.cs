using NetTopologySuite.Geometries;
using System;

namespace Tests.NHibernate.Spatial.OgcSfSql11Compliance.Model
{
    [Serializable]
    public class MapNeatline
    {
        public MapNeatline()
        {
        }

        public MapNeatline(long fid, Geometry neatline)
        {
            this.Fid = fid;
            this.Neatline = neatline;
        }

        private long fid;

        public long Fid
        {
            get { return fid; }
            set { fid = value; }
        }

        private Geometry neatline;

        public Geometry Neatline
        {
            get { return neatline; }
            set { neatline = value; }
        }
    }
}