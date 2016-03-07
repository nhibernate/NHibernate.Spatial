using GeoAPI.Geometries;
using NetTopologySuite.Geometries;
using NetTopologySuite.Noding.Snapround;
using NetTopologySuite.Precision;
using NetTopologySuite.Utilities;
using System.Collections;
using System.Collections.Generic;

namespace Open.Topology.TestRunner.Functions
{
    public class NodingFunctions
    {
        public static IGeometry NodeWithPointwisePrecision(IGeometry geom, double scaleFactor)
        {
            var pm = new PrecisionModel(scaleFactor);

            var roundedGeom = GeometryPrecisionReducer.Reduce(geom, pm);

            var geomList = new List<IGeometry>();
            geomList.Add(roundedGeom);

            var noder = new GeometryNoder(pm);
            var lines = noder.Node(geomList);

            return Utility.FunctionsUtil.getFactoryOrDefault(geom).BuildGeometry(CollectionUtil.Cast<IGeometry>((ICollection)lines));
        }
    }
}