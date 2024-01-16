using NetTopologySuite.Geometries;
using NetTopologySuite.Operation.Buffer;
using NetTopologySuite.Utilities;
using System;

namespace Open.Topology.TestRunner.Functions
{
    public class NTSFunctions
    {
        //public static String version(Geometry g)
        //{
        //    return NTSVersion.CURRENT_VERSION.toString();
        //}

        private static readonly double HEIGHT = 70;
        private static readonly double WIDTH = 150; //125;
        private static readonly double J_WIDTH = 30;
        private static readonly double J_RADIUS = J_WIDTH - 5;

        private static readonly double S_RADIUS = HEIGHT/4;

        private static readonly double T_WIDTH = WIDTH - 2*S_RADIUS - J_WIDTH;

        public static Geometry logoLines(Geometry g)
        {
            return create_J(g)
                .Union(create_T(g))
                .Union(create_S(g));
        }

        public static Geometry logoBuffer(Geometry g, double distance)
        {
            var lines = logoLines(g);
            var bufParams = new BufferParameters
            {
                EndCapStyle = EndCapStyle.Square
            };
            return BufferOp.Buffer(lines, distance, bufParams);
        }

        private static Geometry create_J(Geometry g)
        {
            var gf = Utility.FunctionsUtil.getFactoryOrDefault(g);

            var jTop = new[]
            {
                new Coordinate(0, HEIGHT),
                new Coordinate(J_WIDTH, HEIGHT),
                new Coordinate(J_WIDTH, J_RADIUS)
            };
            var jBottom = new[]
            {
                new Coordinate(J_WIDTH - J_RADIUS, 0),
                new Coordinate(0, 0)
            };

            var gsf = new GeometricShapeFactory(gf)
            {
                Base = new Coordinate(J_WIDTH - 2*J_RADIUS, 0),
                Size = 2*J_RADIUS,
                NumPoints = 10
            };
            var jArc = gsf.CreateArc(1.5*Math.PI, 0.5*Math.PI);

            var coordList = new CoordinateList
            {
                { jTop, false },
                { jArc.Reverse().Coordinates, false, 1, jArc.NumPoints - 1 },
                { jBottom, false }
            };

            return gf.CreateLineString(coordList.ToCoordinateArray());
        }

        private static Geometry create_T(Geometry g)
        {
            var gf = Utility.FunctionsUtil.getFactoryOrDefault(g);

            var tTop = new[]
            {
                new Coordinate(J_WIDTH, HEIGHT),
                new Coordinate(WIDTH - S_RADIUS - 5, HEIGHT)
            };
            var tBottom = new[]
            {
                new Coordinate(J_WIDTH + 0.5*T_WIDTH, HEIGHT),
                new Coordinate(J_WIDTH + 0.5*T_WIDTH, 0)
            };
            var lines = new[]
            {
                gf.CreateLineString(tTop),
                gf.CreateLineString(tBottom)
            };
            return gf.CreateMultiLineString(lines);
        }

        private static Geometry create_S(Geometry g)
        {
            var gf = Utility.FunctionsUtil.getFactoryOrDefault(g);

            double centreX = WIDTH - S_RADIUS;

            var top = new[]
            {
                new Coordinate(WIDTH, HEIGHT),
                new Coordinate(centreX, HEIGHT)
            };
            var bottom = new[]
            {
                new Coordinate(centreX, 0),
                new Coordinate(WIDTH - 2*S_RADIUS, 0)
            };

            var gsf = new GeometricShapeFactory(gf)
            {
                Centre = new Coordinate(centreX, HEIGHT - S_RADIUS),
                Size = 2*S_RADIUS,
                NumPoints = 10
            };
            var arcTop = gsf.CreateArc(0.5*Math.PI, Math.PI);

            var gsf2 = new GeometricShapeFactory(gf)
            {
                Centre = new Coordinate(centreX, S_RADIUS),
                Size = 2*S_RADIUS,
                NumPoints = 10
            };
            var arcBottom = (LineString) gsf2.CreateArc(1.5*Math.PI, Math.PI).Reverse();

            var coordList = new CoordinateList
            {
                { top, false },
                { arcTop.Coordinates, false, 1, arcTop.NumPoints - 1 },
                new Coordinate(centreX, HEIGHT/2),
                { arcBottom.Coordinates, false, 1, arcBottom.NumPoints - 1 },
                { bottom, false }
            };

            return gf.CreateLineString(coordList.ToCoordinateArray());
        }
    }
}
