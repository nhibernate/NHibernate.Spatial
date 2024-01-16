using NetTopologySuite.Geometries;

namespace Open.Topology.TestRunner.Operations
{
    public class NormalizedGeometryMatcher : IGeometryMatcher
    {
        /*
        public NormalizedGeometryMatcher()
        {
        }
         */

        public double Tolerance { get; set; }

        public bool Match(Geometry a, Geometry b)
        {
            var aClone = a.Copy();
            var bClone = b.Copy();
            aClone.Normalize();
            bClone.Normalize();
            return aClone.EqualsExact(bClone, Tolerance);
        }
    }
}
