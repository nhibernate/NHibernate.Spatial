using NetTopologySuite.Geometries;
using System;

namespace Tests.NHibernate.Spatial.Model
{
    [Serializable]
    public class County
    {
        private long id;

        private string name;

        private string state;

        private Geometry boundaries;

        public County()
        { }

        public County(string name, string state, Geometry boundaries)
        {
            Name = name;
            State = state;
            Boundaries = boundaries;
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

        public virtual string State
        {
            get => state;
            set => state = value;
        }

        public virtual Geometry Boundaries
        {
            get => boundaries;
            set => boundaries = value;
        }
    }
}
