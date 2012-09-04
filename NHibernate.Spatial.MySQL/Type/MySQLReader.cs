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

using System.IO;
using GeoAPI.Geometries;
using GeoAPI.IO;
using NetTopologySuite.IO;

namespace NHibernate.Spatial.Type
{
	public class MySQLReader : WKBReader
	{
		public override IGeometry Read(Stream stream)
		{
			using (BinaryReader reader1 = new BinaryReader(stream))
			{
				int srid = -1;
				try
				{
					srid = reader1.ReadInt32();
				}
				catch
				{
				}
				if (srid == 0)
				{
					srid = -1;
				}

				ByteOrder byteOrder = (ByteOrder)stream.ReadByte();
				BinaryReader reader2;
				if (byteOrder == ByteOrder.BigEndian)
				{
					reader2 = new BEBinaryReader(stream);
				}
				else
				{
					reader2 = new BinaryReader(stream);
				}
				using (reader2)
				{
					IGeometry geometry = Read(reader2);
					geometry.SRID = srid;
					return geometry;
				}
			}
		}

	}
}