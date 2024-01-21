using NetTopologySuite.Geometries;
using System;

namespace Tests.NHibernate.Spatial.NtsTestCases.Model
{
    [Serializable]
    public class NtsTestCase
    {
        private long id;

        private string description;

        private Geometry geometryA = GeometryCollection.Empty;

        private Geometry geometryB = GeometryCollection.Empty;

        private string operation;

        private string parameter;

        private Geometry geometryResult = GeometryCollection.Empty;

        private bool booleanResult;

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

        public virtual Geometry GeometryA
        {
            get => geometryA;
            set => geometryA = value;
        }

        public virtual Geometry GeometryB
        {
            get => geometryB;
            set => geometryB = value;
        }

        public virtual string Operation
        {
            get => operation;
            set => operation = value;
        }

        public virtual string Parameter
        {
            get => parameter;
            set => parameter = value;
        }

        public virtual Geometry GeometryResult
        {
            get => geometryResult;
            set => geometryResult = value;
        }

        public virtual bool BooleanResult
        {
            get => booleanResult;
            set => booleanResult = value;
        }
    }
}
