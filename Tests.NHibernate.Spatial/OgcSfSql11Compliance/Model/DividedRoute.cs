using NetTopologySuite.Geometries;
using System;

namespace Tests.NHibernate.Spatial.OgcSfSql11Compliance.Model
{
    [Serializable]
    public class DividedRoute
    {
        private long fid;

        private string name;

        private int numLanes;

        private Geometry centerlines;

        public DividedRoute()
        { }

        public DividedRoute(long fid, string name, int numLanes, Geometry centerlines)
        {
            Fid = fid;
            Name = name;
            NumLanes = numLanes;
            Centerlines = centerlines;
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

        public int NumLanes
        {
            get => numLanes;
            set => numLanes = value;
        }

        public Geometry Centerlines
        {
            get => centerlines;
            set => centerlines = value;
        }
    }
}
