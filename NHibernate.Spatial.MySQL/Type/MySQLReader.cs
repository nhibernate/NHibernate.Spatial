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

using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using System.IO;

namespace NHibernate.Spatial.Type
{
    public class MySQLReader : WKBReader
    {
        public override Geometry Read(Stream stream)
        {
            // MySQL stores geometry values using the first 4 bytes to indicate the SRID
            // followed by the WKB representation of the value; see:
            // https://dev.mysql.com/doc/refman/5.7/en/gis-data-formats.html#gis-wkb-format
            using (var reader1 = new BinaryReader(stream))
            {
                int srid = -1;
                try
                {
                    srid = reader1.ReadInt32();
                }
                catch
                {
                    // Ignored
                }
                if (srid == 0)
                {
                    srid = -1;
                }

                using (var reader2 = new BiEndianBinaryReader(stream))
                {
                    var geometry = Read(reader2);
                    geometry.SRID = srid;
                    return geometry;
                }
            }
        }
    }
}