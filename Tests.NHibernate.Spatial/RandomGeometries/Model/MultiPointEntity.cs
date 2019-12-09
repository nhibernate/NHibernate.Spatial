using NetTopologySuite.Geometries;
using System;

namespace Tests.NHibernate.Spatial.RandomGeometries.Model
{
    [Serializable]
    public class MultiPointEntity
    {
        public MultiPointEntity()
        {
        }

        public MultiPointEntity(string name, Geometry geometry)
        {
            this.Name = name;
            this.Geometry = geometry;
        }

        public MultiPointEntity(long id, string name, Geometry geometry)
        {
            this.Id = id;
            this.Name = name;
            this.Geometry = geometry;
        }

        private long id;

        public virtual long Id
        {
            get { return id; }
            set { id = value; }
        }

        private string name;

        public virtual string Name
        {
            get { return name; }
            set { name = value; }
        }

        private Geometry geometry;

        public virtual Geometry Geometry
        {
            get { return geometry; }
            set { geometry = value; }
        }
    }
}