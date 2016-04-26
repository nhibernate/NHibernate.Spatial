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


using GeoAPI.Geometries;
using MySql.Data.MySqlClient;
using MySql.Data.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHibernate.Spatial.Type
{
    /// <summary>
    /// MySQL geometry type (to be used in models) that supports the changes introduced in MySQL 5.7
    /// </summary>
    public class MySQL57GeometryType : GeometryTypeBase<MySqlGeometry>
    {
        public MySQL57GeometryType()
            : base(new MySQL57GeometryAdapterType())
        {
        }

        /// <summary>
        /// Converts from GeoAPI geometry type to database geometry type.
        /// </summary>
        /// <param name="value">The GeoAPI geometry value.</param>
        /// <returns></returns>
        protected override MySqlGeometry FromGeometry(object value)
        {
            IGeometry geometry = value as IGeometry;
            if (geometry == null)
            {
                return default(MySqlGeometry);
            }
            // MySQL parses empty geometry as NULL
            if (geometry.IsEmpty)
            {
                return default(MySqlGeometry);
            }

            this.SetDefaultSRID(geometry);
            byte[] bytes = new MySQLWriter().Write(geometry);
            return new MySqlGeometry(MySqlDbType.Geometry, bytes);
        }

        /// <summary>
        /// Converts to GeoAPI geometry type from database geometry type.
        /// </summary>
        /// <param name="value">The databse geometry value.</param>
        /// <returns></returns>
        protected override IGeometry ToGeometry(object value)
        {
            MySqlGeometry bytes = (MySqlGeometry)value;

            if (EqualityComparer<MySqlGeometry>.Default.Equals(bytes, default(MySqlGeometry))
                || bytes.Value.Length == 0)
            {
                return null;
            }

            MySQLReader reader = new MySQLReader();
            IGeometry geometry = reader.Read(bytes.Value);
            this.SetDefaultSRID(geometry);
            return geometry;
        }
    }
}
