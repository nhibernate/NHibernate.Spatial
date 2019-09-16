using NetTopologySuite.Geometries;
using NetTopologySuite.Operation.Overlay.Snap;

namespace Open.Topology.TestRunner.Functions
{
    /**
     * Implementations for various geometry functions.
     *
     * @author Martin Davis
     *
     */

    public class GeometryFunctions
    {
        public static double length(Geometry g)
        {
            return g.Length;
        }

        public static double area(Geometry g)
        {
            return g.Area;
        }

        public static bool isSimple(Geometry g)
        {
            return g.IsSimple;
        }

        public static bool isValid(Geometry g)
        {
            return g.IsValid;
        }

        public static bool isRectangle(Geometry g)
        {
            return g.IsRectangle;
        }

        public static Geometry envelope(Geometry g)
        {
            return g.Envelope;
        }

        public static Geometry reverse(Geometry g)
        {
            return g.Reverse();
        }

        public static Geometry normalize(Geometry g)
        {
            Geometry gNorm = g.Copy();
            gNorm.Normalize();
            return gNorm;
        }

        public static Geometry snap(Geometry g, Geometry g2, double distance)
        {
            Geometry[] snapped = GeometrySnapper.Snap(g, g2, distance);
            return snapped[0];
        }

        public static Geometry getGeometryN(Geometry g, int i)
        {
            return g.GetGeometryN(i);
        }

        public static Geometry getCoordinates(Geometry g)
        {
            var pts = g.Coordinates;
            return g.Factory.CreateMultiPointFromCoords(pts);
        }
    }
}