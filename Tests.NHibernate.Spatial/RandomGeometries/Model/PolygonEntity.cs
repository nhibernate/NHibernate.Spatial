using NetTopologySuite.Geometries;
using System;

namespace Tests.NHibernate.Spatial.RandomGeometries.Model
{
    [Serializable]
    public class PolygonEntity
    {
        private long id;

        private string name;

        private Geometry geometry;

        public PolygonEntity()
        { }

        public PolygonEntity(string name, Geometry geometry)
        {
            Name = name;
            Geometry = geometry;
        }

        public PolygonEntity(long id, string name, Geometry geometry)
        {
            Id = id;
            Name = name;
            Geometry = geometry;
        }

        public virtual long Id
        {
            get => id;
            set => id = value;
        }

        public virtual string Name
        {
            get => name;
            set => name = value;
        }

        public virtual Geometry Geometry
        {
            get => geometry;
            set => geometry = value;
        }
    }
}
