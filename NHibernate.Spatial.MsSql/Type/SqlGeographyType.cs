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

using Microsoft.SqlServer.Types;
using NHibernate.SqlTypes;
using NHibernate.Type;
using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using NHibernate.Engine;

namespace NHibernate.Spatial.Type
{
    /// <summary>
    /// Maps a <see cref="Microsoft.SqlServer.Types.SqlGeography" /> to a <see cref="geography" /> column.
    /// </summary>
    [Serializable]
    public class SqlGeographyType : ImmutableType
    {
        public SqlGeographyType()
            // Pass any SqlType to base class.
            : base(SqlTypeFactory.Byte)
        {
        }

        public override object NullSafeGet(DbDataReader rs, string name, ISessionImplementor session)
        {
            object value = base.NullSafeGet(rs, name, session);
            return value ?? SqlGeography.Null;
        }

        public override object Get(DbDataReader rs, string name, ISessionImplementor session)
        {
            return Get(rs, rs.GetOrdinal(name), session);
        }

        [Obsolete("This method has no more usages and will be removed in a future version. Override ToLoggableString instead.")]
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
            return (SqlGeography)rs[index];
        }

        [Obsolete("This method has no more usages and will be removed in a future version.")]
        public override object FromStringValue(string xml)
        {
            return SqlGeography.STGeomFromText(new SqlChars(xml), 0);
        }

        public override System.Type ReturnedClass
        {
            get { return typeof(SqlGeography); }
        }

        public override void Set(DbCommand cmd, object value, int index, ISessionImplementor session)
        {
            object parameterValue = ((INullable)value).IsNull ? DBNull.Value : value;

            SqlParameter sqlParameter = (SqlParameter)cmd.Parameters[index];
            sqlParameter.SqlDbType = SqlDbType.Udt;
            sqlParameter.UdtTypeName = "geography";
            sqlParameter.Value = parameterValue;
        }
    }
}