using NetTopologySuite.Geometries;
using NetTopologySuite.Geometries.Utilities;
using NetTopologySuite.Operation.Buffer;
using NetTopologySuite.Operation.Buffer.Validate;
using System;
using System.Collections.Generic;

namespace Open.Topology.TestRunner.Functions
{
    public class BufferFunctions
    {
        public static Geometry Buffer(Geometry g, double distance)
        {
            return g.Buffer(distance);
        }

        public static Geometry BufferWithParams(Geometry g, Double? distance,
                int? quadrantSegments, int? capStyle, int? joinStyle, Double? mitreLimit)
        {
            double dist = 0;
            if (distance != null) dist = distance.Value;

            var bufParams = new BufferParameters();
            if (quadrantSegments != null) bufParams.QuadrantSegments = quadrantSegments.Value;
            if (capStyle != null) bufParams.EndCapStyle = (EndCapStyle)capStyle.Value;
            if (joinStyle != null) bufParams.JoinStyle = (JoinStyle)joinStyle.Value;
            if (mitreLimit != null) bufParams.MitreLimit = mitreLimit.Value;

            return BufferOp.Buffer(g, dist, bufParams);
        }

        public static Geometry BufferOffsetCurve(Geometry g, double distance)
        {
            return BuildCurveSet(g, distance, new BufferParameters());
        }

        public static Geometry BufferOffsetCurveWithParams(Geometry g, Double? distance,
                int? quadrantSegments, int? capStyle, int? joinStyle, Double? mitreLimit)
        {
            double dist = 0;
            if (distance != null) dist = distance.Value;

            var bufParams = new BufferParameters();
            if (quadrantSegments != null) bufParams.QuadrantSegments = quadrantSegments.Value;
            if (capStyle != null) bufParams.EndCapStyle = (EndCapStyle)capStyle.Value;
            if (joinStyle != null) bufParams.JoinStyle = (JoinStyle)joinStyle.Value;
            if (mitreLimit != null) bufParams.MitreLimit = mitreLimit.Value;

            return BuildCurveSet(g, dist, bufParams);
        }

        private static Geometry BuildCurveSet(Geometry g, double dist, BufferParameters bufParams)
        {
            // --- now construct curve
            var ocb = new OffsetCurveBuilder(g.Factory.PrecisionModel, bufParams);
            var ocsb = new OffsetCurveSetBuilder(g, dist, ocb);
            var curves = ocsb.GetCurves();

            var lines = new List<Geometry>();
            foreach (var ss in curves)
            {
                var pts = ss.Coordinates;
                lines.Add(g.Factory.CreateLineString(pts));
            }
            Geometry curve = g.Factory.BuildGeometry(lines);
            return curve;
        }

        public static Geometry BufferLineSimplifier(Geometry g, double distance)
        {
            return BuildBufferLineSimplifiedSet(g, distance);
        }

        private static Geometry BuildBufferLineSimplifiedSet(Geometry g, double distance)
        {
            var simpLines = new List<Geometry>();

            var lines = LinearComponentExtracter.GetLines(g);

            foreach (var line in lines)
            {
                var pts = line.Coordinates;
                simpLines.Add(g.Factory.CreateLineString(BufferInputLineSimplifier.Simplify(pts, distance)));
            }
            var simpGeom = g.Factory.BuildGeometry(simpLines);
            return simpGeom;
        }

        public static Geometry BufferValidated(Geometry g, double distance)
        {
            var buf = g.Buffer(distance);
            var errMsg = BufferResultValidator.IsValidMessage(g, distance, buf);
            if (errMsg != null)
                throw new InvalidOperationException(errMsg);
            return buf;
        }
    }
}