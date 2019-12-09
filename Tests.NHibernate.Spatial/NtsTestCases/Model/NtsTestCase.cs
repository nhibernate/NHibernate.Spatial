using NetTopologySuite.Geometries;
using System;

namespace Tests.NHibernate.Spatial.NtsTestCases.Model
{
    [Serializable]
    public class NtsTestCase
    {
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

        private Geometry geometryA = GeometryCollection.Empty;

        public virtual Geometry GeometryA
        {
            get { return geometryA; }
            set { geometryA = value; }
        }

        private Geometry geometryB = GeometryCollection.Empty;

        public virtual Geometry GeometryB
        {
            get { return geometryB; }
            set { geometryB = value; }
        }

        private string operation;

        public virtual string Operation
        {
            get { return operation; }
            set { operation = value; }
        }

        private string relatePattern;

        public virtual string RelatePattern
        {
            get { return relatePattern; }
            set { relatePattern = value; }
        }

        private Geometry geometryResult = GeometryCollection.Empty;

        public virtual Geometry GeometryResult
        {
            get { return geometryResult; }
            set { geometryResult = value; }
        }

        private bool booleanResult;

        public virtual bool BooleanResult
        {
            get { return booleanResult; }
            set { booleanResult = value; }
        }
    }
}