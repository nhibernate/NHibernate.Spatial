using NetTopologySuite.Geometries;
using System;

namespace Tests.NHibernate.Spatial.OgcSfSql11Compliance.Model
{
    [Serializable]
    public class Lake
    {
        private long fid;

        private string name;

        private Geometry shore;

        public Lake()
        { }

        public Lake(long fid, string name, Geometry shore)
        {
            Fid = fid;
            Name = name;
            Shore = shore;
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

        public Geometry Shore
        {
            get => shore;
            set => shore = value;
        }
    }
}
