using NetTopologySuite.Geometries;
using NetTopologySuite.Noding.Snapround;
using NetTopologySuite.Precision;
using System.Collections.Generic;
using System.Linq;

namespace Open.Topology.TestRunner.Functions
{
    public class NodingFunctions
    {
        public static Geometry NodeWithPointwisePrecision(Geometry geom, double scaleFactor)
        {
            var pm = new PrecisionModel(scaleFactor);

            var roundedGeom = GeometryPrecisionReducer.Reduce(geom, pm);

            var geomList = new List<Geometry>();
            geomList.Add(roundedGeom);

            var noder = new GeometryNoder(pm);
            var lines = noder.Node(geomList);

            return Utility.FunctionsUtil.getFactoryOrDefault(geom).BuildGeometry(lines.Cast<Geometry>().ToList());
        }
    }
}