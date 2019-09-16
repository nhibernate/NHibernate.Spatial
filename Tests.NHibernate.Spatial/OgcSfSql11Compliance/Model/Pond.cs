using NetTopologySuite.Geometries;
using System;

namespace Tests.NHibernate.Spatial.OgcSfSql11Compliance.Model
{
    [Serializable]
    public class Pond
    {
        public Pond()
        {
        }

        public Pond(long fid, string name, string type, Geometry shores)
        {
            this.Fid = fid;
            this.Name = name;
            this.Type = type;
            this.Shores = shores;
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

        private string type;

        public string Type
        {
            get { return type; }
            set { type = value; }
        }

        private Geometry shores;

        public Geometry Shores
        {
            get { return shores; }
            set { shores = value; }
        }
    }
}