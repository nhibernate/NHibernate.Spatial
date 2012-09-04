using GeoAPI.Geometries;
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

	public class RelationsGenerator : SpatialMethodGenerator<IGeometry, bool>
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

	public class AnalysisGenerator : SpatialMethodGenerator<IGeometry, IGeometry>
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

	public class AnalysisDistanceGenerator : SpatialMethodGenerator<IGeometry, double>
	{
		public AnalysisDistanceGenerator() : base(g => g.Distance(null)) { }
	}

	public class AnalysisSymDifferenceGenerator : SpatialMethodGenerator<IGeometry, IGeometry>
	{
		public AnalysisSymDifferenceGenerator() : base("SymDifference", g => g.SymmetricDifference(null)) { }
	}

	public class ValidationsGenerator : SpatialPropertyGenerator<IGeometry, bool>
	{
		public ValidationsGenerator()
			: base(
				g => g.IsSimple,
				g => g.IsValid,
				g => g.IsEmpty
				) { }
	}

	public class ValidationsOfCurveGenerator : SpatialPropertyGenerator<ICurve, bool>
	{
		public ValidationsOfCurveGenerator()
			: base(
				g => g.IsClosed,
				g => g.IsRing
				) { }
	}

	public class BoundaryGenerator : SpatialPropertyGenerator<IGeometry, IGeometry>
	{
		public BoundaryGenerator() : base(g => g.Boundary) { }
	}

	public class CentroidGenerator : SpatialPropertyGenerator<IGeometry, IPoint>
	{
		public CentroidGenerator() : base(g => g.Centroid) { }
	}

	public class EndPointGenerator : SpatialPropertyGenerator<ICurve, IPoint>
	{
		public EndPointGenerator() : base(g => g.EndPoint) { }
	}

	public class EnvelopeGenerator : SpatialPropertyGenerator<IGeometry, IGeometry>
	{
		public EnvelopeGenerator() : base(g => g.Envelope) { }
	}

	public class ExteriorRingGenerator : SpatialPropertyGenerator<IPolygon, ILineString>
	{
		public ExteriorRingGenerator() : base(g => g.ExteriorRing) { }
	}

	public class GeometryNGenerator : SpatialMethodGenerator<IGeometry, IGeometry>
	{
		public GeometryNGenerator() : base("GeometryN", g => g.GetGeometryN(0)) { }
	}

	public class InteriorRingNGenerator : SpatialMethodGenerator<IPolygon, ILineString>
	{
		public InteriorRingNGenerator() : base("InteriorRingN", g => g.GetInteriorRingN(0)) { }
	}

	public class PointNGenerator : SpatialMethodGenerator<ILineString, IPoint>
	{
		public PointNGenerator() : base("PointN", g => g.GetPointN(0)) { }
	}

	public class PointOnSurfaceGenerator : SpatialPropertyGenerator<IGeometry, IPoint>
	{
		public PointOnSurfaceGenerator() : base(g => g.PointOnSurface) { }
	}

	public class SimplifyGenerator : SpatialMethodGenerator<IGeometry, IGeometry>
	{
		public SimplifyGenerator() : base(g => g.Simplify(0)) { }
	}

	public class StartPointGenerator : SpatialPropertyGenerator<ICurve, IPoint>
	{
		public StartPointGenerator() : base(g => g.StartPoint) { }
	}

	public class TransformGenerator : SpatialMethodGenerator<IGeometry, IGeometry>
	{
		public TransformGenerator() : base(g => g.Transform(0)) { }
	}

	public class GeomCollFromTextGenerator : SpatialMethodGenerator<string, IGeometryCollection>
	{
		public GeomCollFromTextGenerator() : base("GeomCollFromText", g => g.ToGeometryCollection(0)) { }
	}

	public class GeomCollFromWKBGenerator : SpatialMethodGenerator<byte[], IGeometryCollection>
	{
		public GeomCollFromWKBGenerator() : base("GeomCollFromWKB", g => g.ToGeometryCollection(0)) { }
	}

	public class GeomFromTextGenerator : SpatialMethodGenerator<string, IGeometry>
	{
		public GeomFromTextGenerator() : base("GeomFromText", g => g.ToGeometry(0)) { }
	}

	public class GeomFromWKBGenerator : SpatialMethodGenerator<byte[], IGeometry>
	{
		public GeomFromWKBGenerator() : base("GeomFromWKB", g => g.ToGeometry(0)) { }
	}

	public class LineFromTextGenerator : SpatialMethodGenerator<string, ILineString>
	{
		public LineFromTextGenerator() : base("LineFromText", g => g.ToLineString(0)) { }
	}

	public class LineFromWKBGenerator : SpatialMethodGenerator<byte[], ILineString>
	{
		public LineFromWKBGenerator() : base("LineFromWKB", g => g.ToLineString(0)) { }
	}

	public class PointFromTextGenerator : SpatialMethodGenerator<string, IPoint>
	{
		public PointFromTextGenerator() : base("PointFromText", g => g.ToPoint(0)) { }
	}

	public class PointFromWKBGenerator : SpatialMethodGenerator<byte[], IPoint>
	{
		public PointFromWKBGenerator() : base("PointFromWKB", g => g.ToPoint(0)) { }
	}

	public class PolyFromTextGenerator : SpatialMethodGenerator<string, IPolygon>
	{
		public PolyFromTextGenerator() : base("PolyFromText", g => g.ToPolygon(0)) { }
	}

	public class PolyFromWKBGenerator : SpatialMethodGenerator<byte[], IPolygon>
	{
		public PolyFromWKBGenerator() : base("PolyFromWKB", g => g.ToPolygon(0)) { }
	}

	public class MLineFromTextGenerator : SpatialMethodGenerator<string, IMultiLineString>
	{
		public MLineFromTextGenerator() : base("MLineFromText", g => g.ToMultiLineString(0)) { }
	}

	public class MLineFromWKBGenerator : SpatialMethodGenerator<byte[], IMultiLineString>
	{
		public MLineFromWKBGenerator() : base("MLineFromWKB", g => g.ToMultiLineString(0)) { }
	}

	public class MPointFromTextGenerator : SpatialMethodGenerator<string, IMultiPoint>
	{
		public MPointFromTextGenerator() : base("MPointFromText", g => g.ToMultiPoint(0)) { }
	}

	public class MPointFromWKBGenerator : SpatialMethodGenerator<byte[], IMultiPoint>
	{
		public MPointFromWKBGenerator() : base("MPointFromWKB", g => g.ToMultiPoint(0)) { }
	}

	public class MPolyFromTextGenerator : SpatialMethodGenerator<string, IMultiPolygon>
	{
		public MPolyFromTextGenerator() : base("MPolyFromText", g => g.ToMultiPolygon(0)) { }
	}

	public class MPolyFromWKBGenerator : SpatialMethodGenerator<byte[], IMultiPolygon>
	{
		public MPolyFromWKBGenerator() : base("MPolyFromWKB", g => g.ToMultiPolygon(0)) { }
	}

	public class AsBinaryGenerator : SpatialMethodGenerator<IGeometry, byte[]>
	{
		public AsBinaryGenerator() : base(g => g.AsBinary()) { }
	}

	public class AsTextGenerator : SpatialMethodGenerator<IGeometry, string>
	{
		public AsTextGenerator() : base(g => g.AsText()) { }
	}

	public class GeometryTypeGenerator : SpatialPropertyGenerator<IGeometry, string>
	{
		public GeometryTypeGenerator() : base(g => g.GeometryType) { }
	}

	public class AreaGenerator : SpatialPropertyGenerator<IGeometry, double>
	{
		public AreaGenerator() : base(g => g.Area) { }
	}

	public class LengthGenerator : SpatialPropertyGenerator<IGeometry, double>
	{
		public LengthGenerator() : base(g => g.Length) { }
	}

	public class XGenerator : SpatialPropertyGenerator<IPoint, double>
	{
		public XGenerator() : base(g => g.X) { }
	}

	public class YGenerator : SpatialPropertyGenerator<IPoint, double>
	{
		public YGenerator() : base(g => g.Y) { }
	}

	public class SRIDGenerator : SpatialPropertyGenerator<IGeometry, int>
	{
		public SRIDGenerator() : base(g => g.SRID) { }
	}

	public class DimensionGenerator : SpatialMethodGenerator<IGeometry, int>
	{
		public DimensionGenerator() : base("Dimension", g => g.GetDimension()) { }
	}

	public class NumGeometriesGenerator : SpatialPropertyGenerator<IGeometry, int>
	{
		public NumGeometriesGenerator() : base(g => g.NumGeometries) { }
	}

	public class NumInteriorRingsGenerator : SpatialPropertyGenerator<IPolygon, int>
	{
		public NumInteriorRingsGenerator() : base(g => g.NumInteriorRings) { }
	}

	public class NumPointsGenerator : SpatialPropertyGenerator<IGeometry, int>
	{
		public NumPointsGenerator() : base(g => g.NumPoints) { }
	}

	public class RelateGenerator : SpatialMethodGenerator<IGeometry, bool>
	{
		public RelateGenerator() : base(g => g.Relate(null, "")) { }
	}

}
