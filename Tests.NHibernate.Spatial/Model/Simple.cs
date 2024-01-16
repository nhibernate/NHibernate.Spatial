using NetTopologySuite.Geometries;
using System;

namespace Tests.NHibernate.Spatial.Model
{
    [Serializable]
    public class Simple
    {
        private long id;

        private string description;

        private Geometry geometry;

        public Simple()
        { }

        public Simple(string description, Geometry geometry)
        {
            Description = description;
            Geometry = geometry;
        }

        public virtual long Id
        {
            get => id;
            set => id = value;
        }

        public virtual string Description
        {
            get => description;
            set => description = value;
        }

        public virtual Geometry Geometry
        {
            get => geometry;
            set => geometry = value;
        }
    }
}
