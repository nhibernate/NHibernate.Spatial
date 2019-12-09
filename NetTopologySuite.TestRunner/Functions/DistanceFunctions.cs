using NetTopologySuite.Algorithm.Distance;
using NetTopologySuite.Geometries;
using NetTopologySuite.Operation.Distance;

namespace Open.Topology.TestRunner.Functions
{
    public class DistanceFunctions
    {
        public static double distance(Geometry a, Geometry b)
        {
            return a.Distance(b);
        }

        public static bool isWithinDistance(Geometry a, Geometry b, double dist)
        {
            return a.IsWithinDistance(b, dist);
        }

        public static Geometry nearestPoints(Geometry a, Geometry b)
        {
            var pts = DistanceOp.NearestPoints(a, b);
            return a.Factory.CreateLineString(pts);
        }

        public static Geometry discreteHausdorffDistanceLine(Geometry a, Geometry b)
        {
            var dist = new DiscreteHausdorffDistance(a, b);
            dist.Distance();
            return a.Factory.CreateLineString(dist.Coordinates);
        }

        public static Geometry densifiedDiscreteHausdorffDistanceLine(Geometry a, Geometry b, double frac)
        {
            var hausDist = new DiscreteHausdorffDistance(a, b);
            hausDist.DensifyFraction = frac;
            hausDist.Distance();
            return a.Factory.CreateLineString(hausDist.Coordinates);
        }

        public static Geometry discreteOrientedHausdorffDistanceLine(Geometry a, Geometry b)
        {
            var dist = new DiscreteHausdorffDistance(a, b);
            dist.OrientedDistance();
            return a.Factory.CreateLineString(dist.Coordinates);
        }

        public static double discreteHausdorffDistance(Geometry a, Geometry b)
        {
            var dist = new DiscreteHausdorffDistance(a, b);
            return dist.Distance();
        }

        public static double discreteOrientedHausdorffDistance(Geometry a, Geometry b)
        {
            var dist = new DiscreteHausdorffDistance(a, b);
            return dist.OrientedDistance();
        }
    }
}