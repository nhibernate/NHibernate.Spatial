using NetTopologySuite.Algorithm;
using NetTopologySuite.Densify;
using NetTopologySuite.Geometries;
using NetTopologySuite.Geometries.Utilities;
using NetTopologySuite.Operation.Linemerge;

namespace Open.Topology.TestRunner.Functions
{
    public class ConstructionFunctions
    {
        public static Geometry octagonalEnvelope(Geometry g)
        {
            var octEnv = new OctagonalEnvelope(g);
            return octEnv.ToGeometry(g.Factory);
        }

        public static Geometry minimumDiameter(Geometry g)
        {
            return (new MinimumDiameter(g)).Diameter;
        }

        public static Geometry minimumRectangle(Geometry g)
        {
            return (new MinimumDiameter(g)).GetMinimumRectangle();
        }

        public static Geometry minimumBoundingCircle(Geometry g)
        {
            return (new MinimumBoundingCircle(g)).GetCircle();
        }

        public static Geometry boundary(Geometry g)
        {
            return g.Boundary;
        }

        public static Geometry convexHull(Geometry g)
        {
            return g.ConvexHull();
        }

        public static Geometry centroid(Geometry g)
        {
            return g.Centroid;
        }

        public static Geometry interiorPoint(Geometry g)
        {
            return g.InteriorPoint;
        }

        public static Geometry densify(Geometry g, double distance)
        {
            return Densifier.Densify(g, distance);
        }

        public static Geometry mergeLines(Geometry g)
        {
            var merger = new LineMerger();
            merger.Add(g);
            var lines = merger.GetMergedLineStrings();
            return g.Factory.BuildGeometry(lines);
        }

        public static Geometry extractLines(Geometry g)
        {
            var lines = LinearComponentExtracter.GetLines(g);
            return g.Factory.BuildGeometry(lines);
        }
    }
}