using NetTopologySuite.Geometries;
using System.Collections.Generic;

namespace Open.Topology.TestRunner.Functions
{
    public class ConversionFunctions
    {
        public static Geometry toGeometryCollection(Geometry g)
        {
            if (!(g is GeometryCollection))
            {
                return g.Factory.CreateGeometryCollection(new[] { g });
            }

            var atomicGeoms = new List<Geometry>();
            var it = new GeometryCollectionEnumerator(g as GeometryCollection);
            while (it.MoveNext())
            {
                var g2 = it.Current;
                if (!(g2 is GeometryCollection))
                    atomicGeoms.Add(g2);
            }

            return g.Factory.CreateGeometryCollection(GeometryFactory.ToGeometryArray(atomicGeoms));
        }
    }
}