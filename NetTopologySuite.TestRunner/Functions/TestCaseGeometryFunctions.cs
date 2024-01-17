using NetTopologySuite.Densify;
using NetTopologySuite.Geometries;
using NetTopologySuite.Operation.Buffer;
using NetTopologySuite.Triangulate;
using Open.Topology.TestRunner.Utility;
using System;

namespace Open.Topology.TestRunner.Functions
{
    public class TestCaseGeometryFunctions
    {
        public static Geometry bufferMitredJoin(Geometry g, double distance)
        {
            var bufParams = new BufferParameters
            {
                JoinStyle = JoinStyle.Mitre
            };

            return BufferOp.Buffer(g, distance, bufParams);
        }

        public static Geometry densify(Geometry g, double distance)
        {
            return Densifier.Densify(g, distance);
        }
    }

    public class TriangleFunctions
    {
        public static Geometry circumcentre(Geometry g)
        {
            var pts = trianglePts(g);
            var cc = Triangle.Circumcentre(pts[0], pts[1], pts[2]);
            var geomFact = FunctionsUtil.getFactoryOrDefault(g);
            return geomFact.CreatePoint(cc);
        }

        public static Geometry perpendicularBisectors(Geometry g)
        {
            var pts = trianglePts(g);
            var cc = Triangle.Circumcentre(pts[0], pts[1], pts[2]);
            var geomFact = FunctionsUtil.getFactoryOrDefault(g);
            var line = new LineString[3];
            var p0 = new LineSegment(pts[1], pts[2]).ClosestPoint(cc);
            line[0] = geomFact.CreateLineString(new[] { p0, cc });
            var p1 = new LineSegment(pts[0], pts[2]).ClosestPoint(cc);
            line[1] = geomFact.CreateLineString(new[] { p1, cc });
            var p2 = new LineSegment(pts[0], pts[1]).ClosestPoint(cc);
            line[2] = geomFact.CreateLineString(new[] { p2, cc });
            return geomFact.CreateMultiLineString(line);
        }

        public static Geometry incentre(Geometry g)
        {
            var pts = trianglePts(g);
            var cc = Triangle.InCentre(pts[0], pts[1], pts[2]);
            var geomFact = FunctionsUtil.getFactoryOrDefault(g);
            return geomFact.CreatePoint(cc);
        }

        public static Geometry angleBisectors(Geometry g)
        {
            var pts = trianglePts(g);
            var cc = Triangle.InCentre(pts[0], pts[1], pts[2]);
            var geomFact = FunctionsUtil.getFactoryOrDefault(g);
            var line = new LineString[3];
            line[0] = geomFact.CreateLineString(new[] { pts[0], cc });
            line[1] = geomFact.CreateLineString(new[] { pts[1], cc });
            line[2] = geomFact.CreateLineString(new[] { pts[2], cc });
            return geomFact.CreateMultiLineString(line);
        }

        private static Coordinate[] trianglePts(Geometry g)
        {
            var pts = g.Coordinates;
            if (pts.Length < 3)
            {
                throw new ArgumentException("Input geometry must have at least 3 points");
            }
            return pts;
        }
    }

    public class TriangulationFunctions
    {
        private static readonly double TRIANGULATION_TOLERANCE = 0.0;

        public static Geometry delaunayEdges(Geometry geom)
        {
            var builder = new DelaunayTriangulationBuilder();
            builder.SetSites(geom);
            builder.Tolerance = TRIANGULATION_TOLERANCE;
            var edges = builder.GetEdges(geom.Factory);
            return edges;
        }

        public static Geometry delaunayTriangles(Geometry geom)
        {
            var builder = new DelaunayTriangulationBuilder();
            builder.SetSites(geom);
            builder.Tolerance = TRIANGULATION_TOLERANCE;
            var tris = builder.GetTriangles(geom.Factory);
            return tris;
        }

        public static Geometry voronoiDiagram(Geometry geom, Geometry g2)
        {
            var builder = new VoronoiDiagramBuilder();
            builder.SetSites(geom);
            if (g2 != null)
            {
                builder.ClipEnvelope = g2.EnvelopeInternal;
            }
            builder.Tolerance = TRIANGULATION_TOLERANCE;
            Geometry diagram = builder.GetDiagram(geom.Factory);
            return diagram;
        }

        public static Geometry voronoiDiagramWithData(Geometry geom, Geometry g2)
        {
            GeometryDataUtil.setComponentDataToIndex(geom);

            var mapper = new VertexTaggedGeometryDataMapper();
            mapper.LoadSourceGeometries(geom);

            var builder = new VoronoiDiagramBuilder();
            builder.SetSites(mapper.Coordinates);
            if (g2 != null)
            {
                builder.ClipEnvelope = g2.EnvelopeInternal;
            }
            builder.Tolerance = TRIANGULATION_TOLERANCE;
            Geometry diagram = builder.GetDiagram(geom.Factory);
            mapper.TransferData(diagram);
            return diagram;
        }

        public static Geometry conformingDelaunayEdges(Geometry sites, Geometry constraints)
        {
            var builder = new ConformingDelaunayTriangulationBuilder();
            builder.SetSites(sites);
            builder.Constraints = constraints;
            builder.Tolerance = TRIANGULATION_TOLERANCE;

            var geomFact = sites != null ? sites.Factory : constraints.Factory;
            Geometry tris = builder.GetEdges(geomFact);
            return tris;
        }

        public static Geometry conformingDelaunayTriangles(Geometry sites, Geometry constraints)
        {
            var builder = new ConformingDelaunayTriangulationBuilder();
            builder.SetSites(sites);
            builder.Constraints = constraints;
            builder.Tolerance = TRIANGULATION_TOLERANCE;

            var geomFact = sites != null ? sites.Factory : constraints.Factory;
            Geometry tris = builder.GetTriangles(geomFact);
            return tris;
        }
    }
}
