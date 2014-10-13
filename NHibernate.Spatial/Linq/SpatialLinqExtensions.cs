using GeoAPI.Geometries;

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
        public static bool IsNull(this IGeometry geometry)
        { return geometry == null; }

        public static int GetDimension(this IGeometry geometry)
        { throw new SpatialLinqMethodException(); }

        public static IGeometry Simplify(this IGeometry geometry, double distance)
        { throw new SpatialLinqMethodException(); }

        public static IGeometry Transform(this IGeometry geometry, int srid)
        { throw new SpatialLinqMethodException(); }

        public static IGeometryCollection ToGeometryCollection(this string text, int srid)
        { throw new SpatialLinqMethodException(); }

        public static IGeometryCollection ToGeometryCollection(this byte[] wkb, int srid)
        { throw new SpatialLinqMethodException(); }

        public static IGeometry ToGeometry(this string text, int srid)
        { throw new SpatialLinqMethodException(); }

        public static IGeometry ToGeometry(this byte[] wkb, int srid)
        { throw new SpatialLinqMethodException(); }

        public static ILineString ToLineString(this string text, int srid)
        { throw new SpatialLinqMethodException(); }

        public static ILineString ToLineString(this byte[] wkb, int srid)
        { throw new SpatialLinqMethodException(); }

        public static IPoint ToPoint(this string text, int srid)
        { throw new SpatialLinqMethodException(); }

        public static IPoint ToPoint(this byte[] wkb, int srid)
        { throw new SpatialLinqMethodException(); }

        public static IPolygon ToPolygon(this string text, int srid)
        { throw new SpatialLinqMethodException(); }

        public static IPolygon ToPolygon(this byte[] wkb, int srid)
        { throw new SpatialLinqMethodException(); }

        public static IMultiLineString ToMultiLineString(this string text, int srid)
        { throw new SpatialLinqMethodException(); }

        public static IMultiLineString ToMultiLineString(this byte[] wkb, int srid)
        { throw new SpatialLinqMethodException(); }

        public static IMultiPoint ToMultiPoint(this string text, int srid)
        { throw new SpatialLinqMethodException(); }

        public static IMultiPoint ToMultiPoint(this byte[] wkb, int srid)
        { throw new SpatialLinqMethodException(); }

        public static IMultiPolygon ToMultiPolygon(this string text, int srid)
        { throw new SpatialLinqMethodException(); }

        public static IMultiPolygon ToMultiPolygon(this byte[] wkb, int srid)
        { throw new SpatialLinqMethodException(); }
    }
}