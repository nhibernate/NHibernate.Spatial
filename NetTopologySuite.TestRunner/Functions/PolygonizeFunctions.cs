using NetTopologySuite.Geometries;
using NetTopologySuite.Geometries.Utilities;
using NetTopologySuite.Operation.Polygonize;
using System.Collections.Generic;
using System.Linq;

namespace Open.Topology.TestRunner.Functions
{
    public class PolygonizeFunctions
    {
        public static Geometry polygonize(Geometry g)
        {
            var lines = LineStringExtracter.GetLines(g);
            var polygonizer = new Polygonizer();
            polygonizer.Add(lines);
            var polys = polygonizer.GetPolygons();
            var polyArray = GeometryFactory.ToPolygonArray(polys);
            return g.Factory.CreateGeometryCollection(polyArray);
        }

        public static Geometry polygonizeDangles(Geometry g)
        {
            var lines = LineStringExtracter.GetLines(g);
            Polygonizer polygonizer = new Polygonizer();
            polygonizer.Add(lines);
            var geomList = polygonizer.GetDangles()
                .Select(x => (Geometry) x)
                .ToList();

            return g.Factory.BuildGeometry(geomList);
        }

        public static Geometry polygonizeCutEdges(Geometry g)
        {
            var lines = LineStringExtracter.GetLines(g);
            Polygonizer polygonizer = new Polygonizer();
            polygonizer.Add(lines);
            var geomList = polygonizer.GetCutEdges()
                .Select(x => (Geometry) x)
                .ToList();
            return g.Factory.BuildGeometry(geomList);
        }

        public static Geometry polygonizeInvalidRingLines(Geometry g)
        {
            var lines = LineStringExtracter.GetLines(g);
            Polygonizer polygonizer = new Polygonizer();
            polygonizer.Add(lines);
            var geom = polygonizer.GetInvalidRingLines();
            return g.Factory.BuildGeometry(geom);
        }

        public static Geometry polygonizeAllErrors(Geometry g)
        {
            var lines = LineStringExtracter.GetLines(g);
            Polygonizer polygonizer = new Polygonizer();
            polygonizer.Add(lines);
            var geomList = new List<Geometry>();
            geomList.AddRange(polygonizer.GetDangles());
            geomList.AddRange(polygonizer.GetCutEdges());
            geomList.AddRange(polygonizer.GetInvalidRingLines());

            return g.Factory.BuildGeometry(geomList);
        }
    }
}