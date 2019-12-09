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
using System;
using System.IO;

namespace NHibernate.Spatial.Type
{
    public class MySQLWriter : WKBWriter
    {
        protected override int SetByteStream(Geometry geometry)
        {
            return base.SetByteStream(geometry) + 4; // sizeof(int)
        }

        public override byte[] Write(Geometry geometry)
        {
            byte[] bytes = new byte[SetByteStream(geometry)];
            Write(geometry, new MemoryStream(bytes));
            return bytes;
        }

        public override void Write(Geometry geometry, Stream stream)
        {
            BinaryWriter writer;
            if (this.EncodingType == ByteOrder.LittleEndian)
            {
                writer = new BinaryWriter(stream);
            }
            else
            {
                writer = new BEBinaryWriter(stream);
            }
            using (writer)
            {
                writer.Write(geometry.SRID < 0 ? 0 : geometry.SRID);
                if (geometry.IsEmpty)
                {
                    WriteGeometryCollectionEmpty(geometry, writer);
                }
                else
                {
                    Write(geometry, writer);
                }
            }
        }

        protected void WriteGeometryCollectionEmpty(Geometry geometry, BinaryWriter writer)
        {
            WriteByteOrder(writer);
            if (geometry.Coordinate == null || Double.IsNaN(geometry.Coordinate.Z))
            {
                writer.Write((int) WKBGeometryTypes.WKBGeometryCollection);
            }
            else
            {
                writer.Write((int) WKBGeometryTypes.WKBGeometryCollectionZ);
            }
            writer.Write(0);
        }
    }
}