using NetTopologySuite.Geometries;
using NHibernate.Linq.Functions;

namespace NHibernate.Spatial.Linq.Functions
{
    public class SpatialLinqToHqlGeneratorsRegistry : DefaultLinqToHqlGeneratorsRegistry
    {
        public SpatialLinqToHqlGeneratorsRegistry()
        {
            this.Merge(new RelationsGenerator());

            this.Merge(new AnalysisGenerator());
            this.Merge(new AnalysisDistanceGenerator());
            this.Merge(new AnalysisSymDifferenceGenerator());

            this.Merge(new ValidationsGenerator());
            this.Merge(new ValidationsOfCurveGenerator());

            this.Merge(new BoundaryGenerator());
            this.Merge(new CentroidGenerator());
            this.Merge(new EndPointGenerator());
            this.Merge(new EnvelopeGenerator());
            this.Merge(new ExteriorRingGenerator());
            this.Merge(new GeometryNGenerator());
            this.Merge(new InteriorRingNGenerator());
            this.Merge(new PointNGenerator());
            this.Merge(new PointOnSurfaceGenerator());
            this.Merge(new SimplifyGenerator());
            this.Merge(new StartPointGenerator());
            this.Merge(new TransformGenerator());

            this.Merge(new GeomCollFromTextGenerator());
            this.Merge(new GeomCollFromWKBGenerator());
            this.Merge(new GeomFromTextGenerator());
            this.Merge(new GeomFromWKBGenerator());
            this.Merge(new LineFromTextGenerator());
            this.Merge(new LineFromWKBGenerator());
            this.Merge(new PointFromTextGenerator());
            this.Merge(new PointFromWKBGenerator());
            this.Merge(new PolyFromTextGenerator());
            this.Merge(new PolyFromWKBGenerator());
            this.Merge(new MLineFromTextGenerator());
            this.Merge(new MLineFromWKBGenerator());
            this.Merge(new MPointFromTextGenerator());
            this.Merge(new MPointFromWKBGenerator());
            this.Merge(new MPolyFromTextGenerator());
            this.Merge(new MPolyFromWKBGenerator());

            this.Merge(new AsBinaryGenerator());
            this.Merge(new AsTextGenerator());
            this.Merge(new GeometryTypeGenerator());

            this.Merge(new AreaGenerator());
            this.Merge(new LengthGenerator());
            this.Merge(new XGenerator());
            this.Merge(new YGenerator());

            this.Merge(new SRIDGenerator());
            this.Merge(new DimensionGenerator());
            this.Merge(new NumGeometriesGenerator());
            this.Merge(new NumInteriorRingsGenerator());
            this.Merge(new NumPointsGenerator());

            this.Merge(new RelateGenerator());
        }
    }

    public class RelationsGenerator : SpatialMethodGenerator<Geometry, bool>
    {
        public RelationsGenerator()
            : base(
                g => g.Contains(null),
                g => g.CoveredBy(null),
                g => g.Covers(null),
                g => g.Crosses(null),
                g => g.Disjoint(null),
                g => g.Equals(null),
                g => g.Intersects(null),
                g => g.Overlaps(null),
                g => g.Touches(null),
                g => g.Within(null)
                ) { }
    }

    public class AnalysisGenerator : SpatialMethodGenerator<Geometry, Geometry>
    {
        public AnalysisGenerator()
            : base(
                g => g.Buffer(0),
                g => g.ConvexHull(),
                g => g.Difference(null),
                g => g.Intersection(null),
                g => g.Union(null)
                ) { }
    }

    public class AnalysisDistanceGenerator : SpatialMethodGenerator<Geometry, double>
    {
        public AnalysisDistanceGenerator()
            : base(g => g.Distance(null))
        {
        }
    }

    public class AnalysisSymDifferenceGenerator : SpatialMethodGenerator<Geometry, Geometry>
    {
        public AnalysisSymDifferenceGenerator()
            : base("SymDifference", g => g.SymmetricDifference(null))
        {
        }
    }

    public class ValidationsGenerator : SpatialPropertyGenerator<Geometry, bool>
    {
        public ValidationsGenerator()
            : base(
                g => g.IsSimple,
                g => g.IsValid,
                g => g.IsEmpty
                ) { }
    }

    public class ValidationsOfCurveGenerator : SpatialPropertyGenerator<LineString, bool>
    {
        public ValidationsOfCurveGenerator()
            : base(
                g => g.IsClosed,
                g => g.IsRing
                ) { }
    }

    public class BoundaryGenerator : SpatialPropertyGenerator<Geometry, Geometry>
    {
        public BoundaryGenerator()
            : base(g => g.Boundary)
        {
        }
    }

    public class CentroidGenerator : SpatialPropertyGenerator<Geometry, Point>
    {
        public CentroidGenerator()
            : base(g => g.Centroid)
        {
        }
    }

    public class EndPointGenerator : SpatialPropertyGenerator<LineString, Point>
    {
        public EndPointGenerator()
            : base(g => g.EndPoint)
        {
        }
    }

    public class EnvelopeGenerator : SpatialPropertyGenerator<Geometry, Geometry>
    {
        public EnvelopeGenerator()
            : base(g => g.Envelope)
        {
        }
    }

    public class ExteriorRingGenerator : SpatialPropertyGenerator<Polygon, LineString>
    {
        public ExteriorRingGenerator()
            : base(g => g.ExteriorRing)
        {
        }
    }

    public class GeometryNGenerator : SpatialMethodGenerator<Geometry, Geometry>
    {
        public GeometryNGenerator()
            : base("GeometryN", g => g.GetGeometryN(0))
        {
        }
    }

    public class InteriorRingNGenerator : SpatialMethodGenerator<Polygon, LineString>
    {
        public InteriorRingNGenerator()
            : base("InteriorRingN", g => g.GetInteriorRingN(0))
        {
        }
    }

    public class PointNGenerator : SpatialMethodGenerator<LineString, Point>
    {
        public PointNGenerator()
            : base("PointN", g => g.GetPointN(0))
        {
        }
    }

    public class PointOnSurfaceGenerator : SpatialPropertyGenerator<Geometry, Point>
    {
        public PointOnSurfaceGenerator()
            : base(g => g.PointOnSurface)
        {
        }
    }

    public class SimplifyGenerator : SpatialMethodGenerator<Geometry, Geometry>
    {
        public SimplifyGenerator()
            : base(g => g.Simplify(0))
        {
        }
    }

    public class StartPointGenerator : SpatialPropertyGenerator<LineString, Point>
    {
        public StartPointGenerator()
            : base(g => g.StartPoint)
        {
        }
    }

    public class TransformGenerator : SpatialMethodGenerator<Geometry, Geometry>
    {
        public TransformGenerator()
            : base(g => g.Transform(0))
        {
        }
    }

    public class GeomCollFromTextGenerator : SpatialMethodGenerator<string, GeometryCollection>
    {
        public GeomCollFromTextGenerator()
            : base("GeomCollFromText", g => g.ToGeometryCollection(0))
        {
        }
    }

    public class GeomCollFromWKBGenerator : SpatialMethodGenerator<byte[], GeometryCollection>
    {
        public GeomCollFromWKBGenerator()
            : base("GeomCollFromWKB", g => g.ToGeometryCollection(0))
        {
        }
    }

    public class GeomFromTextGenerator : SpatialMethodGenerator<string, Geometry>
    {
        public GeomFromTextGenerator()
            : base("GeomFromText", g => g.ToGeometry(0))
        {
        }
    }

    public class GeomFromWKBGenerator : SpatialMethodGenerator<byte[], Geometry>
    {
        public GeomFromWKBGenerator()
            : base("GeomFromWKB", g => g.ToGeometry(0))
        {
        }
    }

    public class LineFromTextGenerator : SpatialMethodGenerator<string, LineString>
    {
        public LineFromTextGenerator()
            : base("LineFromText", g => g.ToLineString(0))
        {
        }
    }

    public class LineFromWKBGenerator : SpatialMethodGenerator<byte[], LineString>
    {
        public LineFromWKBGenerator()
            : base("LineFromWKB", g => g.ToLineString(0))
        {
        }
    }

    public class PointFromTextGenerator : SpatialMethodGenerator<string, Point>
    {
        public PointFromTextGenerator()
            : base("PointFromText", g => g.ToPoint(0))
        {
        }
    }

    public class PointFromWKBGenerator : SpatialMethodGenerator<byte[], Point>
    {
        public PointFromWKBGenerator()
            : base("PointFromWKB", g => g.ToPoint(0))
        {
        }
    }

    public class PolyFromTextGenerator : SpatialMethodGenerator<string, Polygon>
    {
        public PolyFromTextGenerator()
            : base("PolyFromText", g => g.ToPolygon(0))
        {
        }
    }

    public class PolyFromWKBGenerator : SpatialMethodGenerator<byte[], Polygon>
    {
        public PolyFromWKBGenerator()
            : base("PolyFromWKB", g => g.ToPolygon(0))
        {
        }
    }

    public class MLineFromTextGenerator : SpatialMethodGenerator<string, MultiLineString>
    {
        public MLineFromTextGenerator()
            : base("MLineFromText", g => g.ToMultiLineString(0))
        {
        }
    }

    public class MLineFromWKBGenerator : SpatialMethodGenerator<byte[], MultiLineString>
    {
        public MLineFromWKBGenerator()
            : base("MLineFromWKB", g => g.ToMultiLineString(0))
        {
        }
    }

    public class MPointFromTextGenerator : SpatialMethodGenerator<string, MultiPoint>
    {
        public MPointFromTextGenerator()
            : base("MPointFromText", g => g.ToMultiPoint(0))
        {
        }
    }

    public class MPointFromWKBGenerator : SpatialMethodGenerator<byte[], MultiPoint>
    {
        public MPointFromWKBGenerator()
            : base("MPointFromWKB", g => g.ToMultiPoint(0))
        {
        }
    }

    public class MPolyFromTextGenerator : SpatialMethodGenerator<string, MultiPolygon>
    {
        public MPolyFromTextGenerator()
            : base("MPolyFromText", g => g.ToMultiPolygon(0))
        {
        }
    }

    public class MPolyFromWKBGenerator : SpatialMethodGenerator<byte[], MultiPolygon>
    {
        public MPolyFromWKBGenerator()
            : base("MPolyFromWKB", g => g.ToMultiPolygon(0))
        {
        }
    }

    public class AsBinaryGenerator : SpatialMethodGenerator<Geometry, byte[]>
    {
        public AsBinaryGenerator()
            : base(g => g.AsBinary())
        {
        }
    }

    public class AsTextGenerator : SpatialMethodGenerator<Geometry, string>
    {
        public AsTextGenerator()
            : base(g => g.AsText())
        {
        }
    }

    public class GeometryTypeGenerator : SpatialPropertyGenerator<Geometry, string>
    {
        public GeometryTypeGenerator()
            : base(g => g.GeometryType)
        {
        }
    }

    public class AreaGenerator : SpatialPropertyGenerator<Geometry, double>
    {
        public AreaGenerator()
            : base(g => g.Area)
        {
        }
    }

    public class LengthGenerator : SpatialPropertyGenerator<Geometry, double>
    {
        public LengthGenerator()
            : base(g => g.Length)
        {
        }
    }

    public class XGenerator : SpatialPropertyGenerator<Point, double>
    {
        public XGenerator()
            : base(g => g.X)
        {
        }
    }

    public class YGenerator : SpatialPropertyGenerator<Point, double>
    {
        public YGenerator()
            : base(g => g.Y)
        {
        }
    }

    public class SRIDGenerator : SpatialPropertyGenerator<Geometry, int>
    {
        public SRIDGenerator()
            : base(g => g.SRID)
        {
        }
    }

    public class DimensionGenerator : SpatialMethodGenerator<Geometry, int>
    {
        public DimensionGenerator()
            : base("Dimension", g => g.GetDimension())
        {
        }
    }

    public class NumGeometriesGenerator : SpatialPropertyGenerator<Geometry, int>
    {
        public NumGeometriesGenerator()
            : base(g => g.NumGeometries)
        {
        }
    }

    public class NumInteriorRingsGenerator : SpatialPropertyGenerator<Polygon, int>
    {
        public NumInteriorRingsGenerator()
            : base(g => g.NumInteriorRings)
        {
        }
    }

    public class NumPointsGenerator : SpatialPropertyGenerator<Geometry, int>
    {
        public NumPointsGenerator()
            : base(g => g.NumPoints)
        {
        }
    }

    public class RelateGenerator : SpatialMethodGenerator<Geometry, bool>
    {
        public RelateGenerator()
            : base(g => g.Relate(null, ""))
        {
        }
    }
}