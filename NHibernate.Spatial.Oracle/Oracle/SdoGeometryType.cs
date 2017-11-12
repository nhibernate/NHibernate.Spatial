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

using System.Data.Common;
using NHibernate.Engine;

namespace NHibernate.Spatial.Oracle
{
    using System;
    using System.Data;

    using global::Oracle.DataAccess.Client;

    using NHibernate.SqlTypes;
    using NHibernate.Type;

    /// <summary>
    /// Maps a <see cref="SdoGeometry" /> to a <see cref="SdoGeometry.SDO_GEOMETRY" /> column.
    /// </summary>
    public class SqlGeometryType : ImmutableType
    {
        public SqlGeometryType()
            // Pass any SqlType to base class.
            : base(SqlTypeFactory.Byte)
        {
        }

        public override object NullSafeGet(DbDataReader rs, string name, ISessionImplementor session)
        {
            object value = base.NullSafeGet(rs, name, session);
            return value ?? SdoGeometry.Null;
        }

        public override object Get(DbDataReader rs, string name, ISessionImplementor session)
        {
            return Get(rs, rs.GetOrdinal(name), session);
        }

        public override string ToString(object value)
        {
            return value.ToString();
        }

        public override string Name
        {
            get { return ReturnedClass.Name; }
        }

        public override object Get(DbDataReader rs, int index, ISessionImplementor session)
        {
            return (SdoGeometry)rs[index];
        }

        public override object FromStringValue(string xml)
        {
            // TODO:
            //return SdoGeometry.STGeomFromText(new SqlChars(xml), 0);
            return null;
        }

        public override Type ReturnedClass
        {
            get { return typeof(SdoGeometry); }
        }

        public override void Set(DbCommand cmd, object value, int index, ISessionImplementor session)
        {
            object parameterValue = (value as SdoGeometry) == null ? DBNull.Value : value;

            OracleParameter oracleParameter = (OracleParameter)cmd.Parameters[index];
            oracleParameter.OracleDbType = OracleDbType.Object;
            oracleParameter.UdtTypeName = "MDSYS.SDO_GEOMETRY";
            oracleParameter.Value = parameterValue;
        }
    }
}