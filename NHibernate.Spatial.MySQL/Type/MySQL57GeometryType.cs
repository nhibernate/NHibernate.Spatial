// Copyright 2016 - Andreas Ravnestad (andreas.ravnestad@gmail.com)
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


using MySql.Data.MySqlClient;
using MySql.Data.Types;
using NetTopologySuite.Geometries;
using NHibernate.Type;

namespace NHibernate.Spatial.Type
{
	/// <summary>
	/// MySQL geometry type (to be used in models) that supports the changes introduced in MySQL 5.7
	/// </summary>
	public class MySQL57GeometryType : GeometryTypeBase<MySqlGeometry?>
	{
		private static readonly NullableType MySQL57GeometryAdapterType = new MySQL57GeometryAdapterType();

		public MySQL57GeometryType()
			: base(MySQL57GeometryAdapterType)
		{
		}

		protected override void SetDefaultSRID(Geometry geometry)
		{
			base.SetDefaultSRID(geometry);
			if (geometry.SRID == -1)
			{
				geometry.SRID = 0;
			}
		}

		/// <summary>
		/// Converts from GeoAPI geometry type to database geometry type.
		/// </summary>
		/// <param name="value">The GeoAPI geometry value.</param>
		/// <returns></returns>
		protected override MySqlGeometry? FromGeometry(object value)
		{
			Geometry geometry = value as Geometry;
			if (geometry == null)
			{
				return null;
			}
			// MySQL parses empty geometry as NULL
			if (geometry.IsEmpty)
			{
                return null;    // TODO: Somehow specify an empty geometry and not just null.
                                // MySqlGeometry does not support empty geometry collections
                                // so we simply return null for now. At some point we should to
                                // use GeometryCollection.Empty here, when MySQL supports it.
			}

            SetDefaultSRID(geometry);
			byte[] bytes = new MySQLWriter().Write(geometry);
			return new MySqlGeometry(MySqlDbType.Geometry, bytes);
		}

		/// <summary>
		/// Converts to GeoAPI geometry type from database geometry type.
		/// </summary>
		/// <param name="value">The database geometry value.</param>
		/// <returns></returns>
		protected override Geometry ToGeometry(object value)
		{
			MySqlGeometry? bytes = value as MySqlGeometry?;

            if (!bytes.HasValue)
		    {
		        return null;
		    }

			MySQLReader reader = new MySQLReader();
			Geometry geometry = reader.Read(bytes.Value.Value);
			SetDefaultSRID(geometry);
			return geometry;
		}
	}
}
