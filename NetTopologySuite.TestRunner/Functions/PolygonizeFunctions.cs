using GeoAPI.Geometries;
using NetTopologySuite.Geometries;
using NetTopologySuite.Geometries.Utilities;
using NetTopologySuite.Operation.Polygonize;
using System.Collections.Generic;
using System.Linq;

namespace Open.Topology.TestRunner.Functions
{
    public class PolygonizeFunctions
    {
        public static IGeometry polygonize(IGeometry g)
        {
            var lines = LineStringExtracter.GetLines(g);
            var polygonizer = new Polygonizer();
            polygonizer.Add(lines);
            var polys = polygonizer.GetPolygons();
            var polyArray = GeometryFactory.ToPolygonArray(polys);
            return g.Factory.CreateGeometryCollection(polyArray);
        }

        public static IGeometry polygonizeDangles(IGeometry g)
        {
            var lines = LineStringExtracter.GetLines(g);
            Polygonizer polygonizer = new Polygonizer();
            polygonizer.Add(lines);
            var geomList = polygonizer.GetDangles()
                .Select(x => (IGeometry) x)
                .ToList();

            return g.Factory.BuildGeometry(geomList);
        }

        public static IGeometry polygonizeCutEdges(IGeometry g)
        {
            var lines = LineStringExtracter.GetLines(g);
            Polygonizer polygonizer = new Polygonizer();
            polygonizer.Add(lines);
            var geomList = polygonizer.GetCutEdges()
                .Select(x => (IGeometry) x)
                .ToList();
            return g.Factory.BuildGeometry(geomList);
        }

        public static IGeometry polygonizeInvalidRingLines(IGeometry g)
        {
            var lines = LineStringExtracter.GetLines(g);
            Polygonizer polygonizer = new Polygonizer();
            polygonizer.Add(lines);
            var geom = polygonizer.GetInvalidRingLines();
            return g.Factory.BuildGeometry(geom);
        }

        public static IGeometry polygonizeAllErrors(IGeometry g)
        {
            var lines = LineStringExtracter.GetLines(g);
            Polygonizer polygonizer = new Polygonizer();
            polygonizer.Add(lines);
            var geomList = new List<IGeometry>();
            geomList.AddRange(polygonizer.GetDangles());
            geomList.AddRange(polygonizer.GetCutEdges());
            geomList.AddRange(polygonizer.GetInvalidRingLines());

            return g.Factory.BuildGeometry(geomList);
        }
    }
}