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
using System.Collections.Generic;
using System.Text;
using GeoAPI.Geometries;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;

namespace NHibernate.Spatial.Type
{
	/// <summary>
	/// This PostGIS geometry type implementation uses strings to represent
	/// byte arrays in hexadecimal, instead of use byte array directly,
	/// due to a limitation in Npgsql driver.
	/// See http://gborg.postgresql.org/project/npgsql/bugs/bugupdate.php?1409
	/// </summary>
    [Serializable]
    public class PostGisGeometryType : GeometryTypeBase<string>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PostGisGeometryType"/> class.
		/// </summary>
		public PostGisGeometryType()
			: base(NHibernateUtil.StringClob)
		{
		}

		/// <summary>
		/// Converts from GeoAPI geometry type to database geometry type.
		/// </summary>
		/// <param name="value">The GeoAPI geometry value.</param>
		/// <returns></returns>
		protected override string FromGeometry(object value)
		{
			IGeometry geometry = value as IGeometry;
			if (geometry == null)
			{
				return null;
			}
			// PostGIS can't parse a WKB of any empty geometry other than GeomtryCollection
			// (throws the error: "geometry requires more points")
			// and parses WKT of empty geometries always as GeometryCollection
			// (ie. "select AsText(GeomFromText('LINESTRING EMPTY', -1)) = 'GEOMETRYCOLLECTION EMPTY'").
			// Force GeometryCollection.Empty to avoid the error.
			if (!(geometry is IGeometryCollection) && geometry.IsEmpty)
			{
				geometry = GeometryCollection.Empty;
			}

			this.SetDefaultSRID(geometry);
			byte[] bytes = new PostGisWriter().Write(geometry);
			return ToString(bytes);
		}

		/// <summary>
		/// Converts to GeoAPI geometry type from database geometry type.
		/// </summary>
		/// <param name="value">The databse geometry value.</param>
		/// <returns></returns>
		protected override IGeometry ToGeometry(object value)
		{
			string bytes = value as string;

			if (string.IsNullOrEmpty(bytes))
			{
				return null;
			}

			// Bounding boxes are not serialized as hexadecimal string (?) 
			const string boxToken = "BOX(";
			if (bytes.StartsWith(boxToken))
			{
				// TODO: Optimize?
				bytes = bytes.Substring(boxToken.Length, bytes.Length - boxToken.Length - 1);
				string[] parts = bytes.Split(',');
				string[] min = parts[0].Split(' ');
				string[] max = parts[1].Split(' ');
				string wkt = string.Format(
					"POLYGON(({0} {1},{0} {3},{2} {3},{2} {1},{0} {1}))",
					min[0], min[1], max[0], max[1]);
				return new WKTReader().Read(wkt);
			}

			PostGisReader reader = new PostGisReader();
			IGeometry geometry = reader.Read(ToByteArray(bytes));
			this.SetDefaultSRID(geometry);
			return geometry;
		}

        private static byte[] ToByteArray(string hex)
        {
            if (hex.Length % 2 == 1)
                throw new ArgumentException("Invalid input. It must have an even length.", "hex");

            byte[] data = new byte[hex.Length / 2];
            for (int i = 0; i < hex.Length; i += 2)
            {
                char c1 = hex[i];
                char c2 = hex[i + 1];

                int result = (c1 < 'A') ? (c1 - '0') : (10 + (c1 - 'A'));
                result = result << 4;
                result |= (c2 < 'A') ? (c2 - '0') : (10 + (c2 - 'A'));
                data[i / 2] = (byte) (result);
            }
            return data;
        }

        private static string ToString(byte[] bytes)
        {
            char[] data = new char[bytes.Length * 2];
            int idx = 0;
            for (int i = 0; i < bytes.Length; i++)
            {
                int n1 = bytes[i] >> 4;
                int n2 = bytes[i] & 0xF;
                data[idx++] = (char) ((n1) < 10 ? '0' + n1 : n1 - 10 + 'A');
                data[idx++] = (char) ((n2) < 10 ? '0' + n2 : n2 - 10 + 'A');
            }
            return new string(data);
        }

	}
}
