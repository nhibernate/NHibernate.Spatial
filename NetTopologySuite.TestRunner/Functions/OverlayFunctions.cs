using NetTopologySuite.Geometries;

namespace Open.Topology.TestRunner.Functions
{
    public class OverlayFunctions
    {
        public static Geometry intersection(Geometry a, Geometry b)
        {
            return a.Intersection(b);
        }

        public static Geometry union(Geometry a, Geometry b)
        {
            return a.Union(b);
        }

        public static Geometry symDifference(Geometry a, Geometry b)
        {
            return a.SymmetricDifference(b);
        }

        public static Geometry difference(Geometry a, Geometry b)
        {
            return a.Difference(b);
        }

        public static Geometry differenceBA(Geometry a, Geometry b)
        {
            return b.Difference(a);
        }

        public static Geometry unaryUnion(Geometry a)
        {
            return a.Union();
        }
    }
}