using NHibernate.Spatial.Type;
using NHibernate.Type;

namespace NHibernate.Spatial.Dialect
{
	public class MsSql2012GeometryDialect : MsSql2012SpatialDialect
	{
		private static readonly IType geometryType = new CustomType(typeof(MsSql2008GeometryType), null);

		public MsSql2012GeometryDialect()
			: base("geometry", "NHSP_GEOMETRY_COLUMNS")
		{
		}

		/// <summary>
		/// Gets the type of the geometry.
		/// </summary>
		/// <value>The type of the geometry.</value>
		public override IType GeometryType
		{
			get { return geometryType; }
		}

		/// <summary>
		/// Creates the geometry user type.
		/// </summary>
		/// <returns></returns>
		public override IGeometryUserType CreateGeometryUserType()
		{
			return new MsSql2008GeometryType();
		}
	}
}