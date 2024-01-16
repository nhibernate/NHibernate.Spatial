using NetTopologySuite.Geometries;
using System.Collections.Generic;

namespace Open.Topology.TestRunner.Functions
{
    public class ConversionFunctions
    {
        public static Geometry toGeometryCollection(Geometry g)
        {
            if (!(g is GeometryCollection collection))
            {
                return g.Factory.CreateGeometryCollection(new[] { g });
            }

            var atomicGeoms = new List<Geometry>();
            var it = new GeometryCollectionEnumerator(collection);
            while (it.MoveNext())
            {
                var g2 = it.Current;
                if (!(g2 is GeometryCollection))
                {
                    atomicGeoms.Add(g2);
                }
            }

            return collection.Factory.CreateGeometryCollection(GeometryFactory.ToGeometryArray(atomicGeoms));
        }
    }
}
