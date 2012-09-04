// Copyright 2008 - Ricardo Stuven (rstuven@gmail.com)
//
// This file is part of NHibernate.Spatial.
// NHibernate.Spatial is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// NHibernate.Spatial is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.

// You should have received a copy of the GNU Lesser General Public License
// along with NHibernate.Spatial; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

using NHibernate.Spatial.Type;
using NHibernate.Type;

namespace NHibernate.Spatial.Dialect
{
	public class MsSql2008GeographyDialect : MsSql2008SpatialDialect
	{
		private static readonly IType geometryType = new CustomType(typeof(MsSql2008GeographyType), null);

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
			return new MsSql2008GeographyType();
		}

		/// <summary>
		/// Gets the SQL Server spatial datatype name.
		/// </summary>
		protected override string SqlTypeName
		{
			get { return "geography"; }
		}

		/// <summary>
		/// Gets the columns catalog view name.
		/// </summary>
		protected override string GeometryColumnsViewName
		{
			get { return "NHSP_GEOGRAPHY_COLUMNS"; }
		}
	}
}
