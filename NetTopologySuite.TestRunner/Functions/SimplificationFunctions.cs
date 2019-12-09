using NetTopologySuite.Geometries;
using NetTopologySuite.Simplify;

namespace Open.Topology.TestRunner.Functions
{
    public class SimplificationFunctions
    {
        public static Geometry simplifyDP(Geometry g, double distance)
        { return DouglasPeuckerSimplifier.Simplify(g, distance); }

        public static Geometry simplifyTP(Geometry g, double distance)
        { return TopologyPreservingSimplifier.Simplify(g, distance); }
    }
}