using NetTopologySuite.Geometries;
using System;

namespace Tests.NHibernate.Spatial.OgcSfSql11Compliance.Model
{
    [Serializable]
    public class RoadSegment
    {
        private long fid;

        private string name;

        private string aliases;

        private int numLanes;

        private Geometry centerline;

        public RoadSegment()
        { }

        public RoadSegment(long fid, string name, string aliases, int numLanes, Geometry centerline)
        {
            Fid = fid;
            Name = name;
            Aliases = aliases;
            NumLanes = numLanes;
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

        public string Aliases
        {
            get => aliases;
            set => aliases = value;
        }

        public int NumLanes
        {
            get => numLanes;
            set => numLanes = value;
        }

        public Geometry Centerline
        {
            get => centerline;
            set => centerline = value;
        }
    }
}
