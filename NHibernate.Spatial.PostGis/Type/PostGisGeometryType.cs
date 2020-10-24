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

using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using System;
using System.Data.Common;
using NHibernate.Engine;
using NHibernate.SqlTypes;
using NHibernate.Type;
using Npgsql;
using NpgsqlTypes;

namespace NHibernate.Spatial.Type
{
    [Serializable]
    public class PostGisGeometryType : GeometryTypeBase<byte[]>
    {
        private static readonly NullableType GeometryType = new CustomGeometryType();

        /// <summary>
        /// Initializes a new instance of the <see cref="PostGisGeometryType"/> class.
        /// </summary>
        public PostGisGeometryType()
            : base(GeometryType)
        {
        }

        /// <summary>
        /// Converts from GeoAPI geometry type to database geometry type.
        /// </summary>
        /// <param name="value">The GeoAPI geometry value.</param>
        /// <returns></returns>
        protected override byte[] FromGeometry(object value)
        {
            Geometry geometry = value as Geometry;
            if (geometry == null)
            {
                return null;
            }
            // PostGIS can't parse a WKB of any empty geometry other than GeomtryCollection
            // (throws the error: "geometry requires more points")
            // and parses WKT of empty geometries always as GeometryCollection
            // (ie. "select AsText(GeomFromText('LINESTRING EMPTY', -1)) = 'GEOMETRYCOLLECTION EMPTY'").
            // Force GeometryCollection.Empty to avoid the error.
            if (!(geometry is GeometryCollection) && geometry.IsEmpty)
            {
                geometry = GeometryCollection.Empty;
            }

            this.SetDefaultSRID(geometry);

            // Determine the ordinality of the geometry to ensure 3D and 4D geometries are
            // correctly serialized by PostGisWriter (see issue #66)
            // NOTE: Cannot use InteriorPoint here as that always returns a 2D point (see #120)
            // TODO: Is there a way of getting the ordinates directly from the geometry?
            var ordinates = Ordinates.XY;
            var coordinate = geometry.Coordinate;
            if (coordinate != null && !double.IsNaN(coordinate.Z))
            {
                ordinates |= Ordinates.Z;
            }
            if (coordinate != null && !double.IsNaN(coordinate.M))
            {
                ordinates |= Ordinates.M;
            }

            var postGisWriter = new PostGisWriter
            {
                HandleOrdinates = ordinates
            };
            return postGisWriter.Write(geometry);
        }

        /// <summary>
        /// Converts to GeoAPI geometry type from database geometry type.
        /// </summary>
        /// <param name="value">The databse geometry value.</param>
        /// <returns></returns>
        protected override Geometry ToGeometry(object value)
        {
            var bytes = value as byte[];
            if (bytes == null)
            {
                return null;
            }

            PostGisReader reader = new PostGisReader();
            Geometry geometry = reader.Read(bytes);
            this.SetDefaultSRID(geometry);
            return geometry;
        }

        [Serializable]
        private class CustomGeometryType : MutableType
        {
            internal CustomGeometryType() : base(new BinarySqlType())
            {
            }

            public override object Get(DbDataReader rs, int index, ISessionImplementor session)
            {
                // Npgsql 3 from the received bytes creates his own PostGisGeometry type.
                // As we need to return a byte array that represents the geometry object,
                // we will retrive the bytes from the reader instead.
                var length = (int)rs.GetBytes(index, 0, null, 0, 0);
                var buffer = new byte[length];
                if (length > 0)
                {
                    rs.GetBytes(index, 0, buffer, 0, length);
                }
                return buffer;
            }

            public override object Get(DbDataReader rs, string name, ISessionImplementor session)
            {
                return Get(rs, rs.GetOrdinal(name), session);
            }

            public override System.Type ReturnedClass => typeof(Geometry);

            public override void Set(DbCommand cmd, object value, int index, ISessionImplementor session)
            {
                var parameter = (NpgsqlParameter)cmd.Parameters[index];
                parameter.NpgsqlDbType = NpgsqlDbType.Geometry;
                parameter.Value = value;
            }

            public override string Name => "Geometry";

            public override object DeepCopyNotNull(object value)
            {
                var arr = (byte[]) value;
                return arr.Clone();
            }
        }
    }
}