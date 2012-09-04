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

using GeoAPI.Geometries;

namespace NHibernate.Spatial.Type
{
	/// <summary>
	/// </summary>
	public class MySQLGeometryType : GeometryTypeBase<byte[]>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="MySQLGeometryType"/> class.
		/// </summary>
		public MySQLGeometryType()
			: base(NHibernateUtil.BinaryBlob)
		{
		}

		/// <summary>
		/// Converts from GeoAPI geometry type to database geometry type.
		/// </summary>
		/// <param name="value">The GeoAPI geometry value.</param>
		/// <returns></returns>
		protected override byte[] FromGeometry(object value)
		{
			IGeometry geometry = value as IGeometry;
			if (geometry == null)
			{
				return null;
			}
			// MySQL parses empty geometry as NULL
			if (geometry.IsEmpty)
			{
				return null;
			}

			this.SetDefaultSRID(geometry);
			byte[] bytes = new MySQLWriter().Write(geometry);
			return bytes;
		}

		/// <summary>
		/// Converts to GeoAPI geometry type from database geometry type.
		/// </summary>
		/// <param name="value">The databse geometry value.</param>
		/// <returns></returns>
		protected override IGeometry ToGeometry(object value)
		{
			byte[] bytes = value as byte[];

			if (bytes == null || bytes.Length == 0)
			{
				return null;
			}
			
			MySQLReader reader = new MySQLReader();
			IGeometry geometry = reader.Read(bytes);
			this.SetDefaultSRID(geometry);
			return geometry;
		}
	}
}
