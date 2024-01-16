using NetTopologySuite.Geometries;
using System;

namespace Tests.NHibernate.Spatial.OgcSfSql11Compliance.Model
{
    [Serializable]
    public class Pond
    {
        private long fid;

        private string name;

        private string type;

        private Geometry shores;

        public Pond()
        { }

        public Pond(long fid, string name, string type, Geometry shores)
        {
            Fid = fid;
            Name = name;
            Type = type;
            Shores = shores;
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

        public string Type
        {
            get => type;
            set => type = value;
        }

        public Geometry Shores
        {
            get => shores;
            set => shores = value;
        }
    }
}
