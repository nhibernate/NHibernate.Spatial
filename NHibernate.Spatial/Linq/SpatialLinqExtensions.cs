using NetTopologySuite.Geometries;

namespace NHibernate.Spatial.Linq
{
    public static class SpatialLinqExtensions
    {
        /// <summary>
        /// A fully compatible null checking. Use instead of " == null " expression.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Using an equality to null lambda expression throws an exception in SQL Server
        /// ("Invalid operator for data type. Operator equals equal to, type equals geometry.")
        /// because NHibernate is generating an HQL expression like this:
        /// </para>
        /// <code>
        ///     (t.geom is null) and (null is null) or t.geom = null
        /// </code>
        /// <para>
        /// Using this extension method, we generate just the following HQL:
        /// </para>
        /// <code>
        ///     t.geom is null
        /// </code>
        /// </remarks>
        /// <param name="geometry"></param>
        /// <returns></returns>
        public static bool IsNull(this Geometry geometry)
        { return geometry == null; }

        public static int GetDimension(this Geometry geometry)
        { throw new SpatialLinqMethodException(); }

        public static Geometry Simplify(this Geometry geometry, double distance)
        { throw new SpatialLinqMethodException(); }

        public static Geometry Transform(this Geometry geometry, int srid)
        { throw new SpatialLinqMethodException(); }

        public static GeometryCollection ToGeometryCollection(this string text, int srid)
        { throw new SpatialLinqMethodException(); }

        public static GeometryCollection ToGeometryCollection(this byte[] wkb, int srid)
        { throw new SpatialLinqMethodException(); }

        public static Geometry ToGeometry(this string text, int srid)
        { throw new SpatialLinqMethodException(); }

        public static Geometry ToGeometry(this byte[] wkb, int srid)
        { throw new SpatialLinqMethodException(); }

        public static LineString ToLineString(this string text, int srid)
        { throw new SpatialLinqMethodException(); }

        public static LineString ToLineString(this byte[] wkb, int srid)
        { throw new SpatialLinqMethodException(); }

        public static Point ToPoint(this string text, int srid)
        { throw new SpatialLinqMethodException(); }

        public static Point ToPoint(this byte[] wkb, int srid)
        { throw new SpatialLinqMethodException(); }

        public static Polygon ToPolygon(this string text, int srid)
        { throw new SpatialLinqMethodException(); }

        public static Polygon ToPolygon(this byte[] wkb, int srid)
        { throw new SpatialLinqMethodException(); }

        public static MultiLineString ToMultiLineString(this string text, int srid)
        { throw new SpatialLinqMethodException(); }

        public static MultiLineString ToMultiLineString(this byte[] wkb, int srid)
        { throw new SpatialLinqMethodException(); }

        public static MultiPoint ToMultiPoint(this string text, int srid)
        { throw new SpatialLinqMethodException(); }

        public static MultiPoint ToMultiPoint(this byte[] wkb, int srid)
        { throw new SpatialLinqMethodException(); }

        public static MultiPolygon ToMultiPolygon(this string text, int srid)
        { throw new SpatialLinqMethodException(); }

        public static MultiPolygon ToMultiPolygon(this byte[] wkb, int srid)
        { throw new SpatialLinqMethodException(); }
    }
}