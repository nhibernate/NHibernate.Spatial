using NetTopologySuite.Geometries;
using System;

namespace Tests.NHibernate.Spatial.Model
{
    [Serializable]
    public class Simple
    {
        public Simple()
        {
        }

        public Simple(string description, Geometry geometry)
        {
            this.Description = description;
            this.Geometry = geometry;
        }

        private long id;

        public virtual long Id
        {
            get { return id; }
            set { id = value; }
        }

        private string description;

        public virtual string Description
        {
            get { return description; }
            set { description = value; }
        }

        private Geometry geometry;

        public virtual Geometry Geometry
        {
            get { return geometry; }
            set { geometry = value; }
        }
    }
}