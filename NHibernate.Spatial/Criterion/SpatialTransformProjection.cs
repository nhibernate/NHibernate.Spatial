// Copyright 2007 - Ricardo Stuven (rstuven@gmail.com)
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

using System;
using NHibernate.SqlCommand;
using NHibernate.Spatial.Dialect;

namespace NHibernate.Spatial.Criterion
{
	/// <summary>
	/// 
	/// </summary>
	[Serializable]
	public class SpatialTransformProjection : SpatialProjection
	{
		private readonly int targetSrid;

		/// <summary>
		/// Initializes a new instance of the <see cref="SpatialTransformProjection"/> class.
		/// </summary>
		/// <param name="propertyName">Name of the property.</param>
		/// <param name="targetSrid">The target srid.</param>
		public SpatialTransformProjection(string propertyName, int targetSrid)
			: base(propertyName)
		{
			this.targetSrid = targetSrid;
		}

		/// <summary>
		/// Render the SQL Fragment.
		/// </summary>
		/// <param name="column">The column.</param>
		/// <param name="spatialDialect">The spatial dialect.</param>
		/// <returns></returns>
		public override SqlString ToSqlString(string column, ISpatialDialect spatialDialect)
		{
			return spatialDialect.GetSpatialTransformString(column, this.targetSrid);
		}

		/// <summary>
		/// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
		/// </returns>
		public override string ToString()
		{
			return "Transform(" + this.propertyName + ", " + this.targetSrid.ToString() + ")";
		}
	}
}
