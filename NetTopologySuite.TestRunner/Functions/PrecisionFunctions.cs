using NetTopologySuite.Geometries;
using NetTopologySuite.Precision;

namespace Open.Topology.TestRunner.Functions
{
    public class PrecisionFunctions
    {
        public static Geometry ReducePrecisionPointwise(Geometry geom, double scaleFactor)
        {
            var pm = new PrecisionModel(scaleFactor);

            var reducedGeom = GeometryPrecisionReducer.Reduce(geom, pm);

            return reducedGeom;
        }
    }
}