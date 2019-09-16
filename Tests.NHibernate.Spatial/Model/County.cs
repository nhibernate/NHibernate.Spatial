using NetTopologySuite.Geometries;
using System;

namespace Tests.NHibernate.Spatial.Model
{
    [Serializable]
    public class County
    {
        public County()
        {
        }

        public County(string name, string state, Geometry boundaries)
        {
            this.Name = name;
            this.State = state;
            this.Boundaries = boundaries;
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

        private string state;

        public virtual string State
        {
            get { return state; }
            set { state = value; }
        }

        private Geometry boundaries;

        public virtual Geometry Boundaries
        {
            get { return boundaries; }
            set { boundaries = value; }
        }
    }
}