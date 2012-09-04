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

using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Types;
using NHibernate.SqlTypes;
using NHibernate.Type;

namespace NHibernate.Spatial.Type
{
	/// <summary>
	/// Maps a <see cref="Microsoft.SqlServer.Types.SqlGeometry" /> to a <see cref="geometry" /> column.
	/// </summary>
	[Serializable]
	public class SqlGeometryType : ImmutableType
	{
		public SqlGeometryType()
			// Pass any SqlType to base class.
			: base(SqlTypeFactory.Byte)
		{
		}

		public override object NullSafeGet(IDataReader rs, string name)
		{
			object value = base.NullSafeGet(rs, name);
			return value ?? SqlGeometry.Null;
		}

		public override object Get(IDataReader rs, string name)
		{
			return Get(rs, rs.GetOrdinal(name));
		}

		public override string ToString(object value)
		{
			return value.ToString();
		}

		public override string Name
		{
			get { return ReturnedClass.Name; }
		}

		public override object Get(IDataReader rs, int index)
		{
			return (SqlGeometry)rs[index];
		}

		public override object FromStringValue(string xml)
		{
			return SqlGeometry.STGeomFromText(new SqlChars(xml), 0);
		}

		public override System.Type ReturnedClass
		{
			get { return typeof(SqlGeometry); }
		}


		public override void Set(IDbCommand cmd, object value, int index)
		{
			object parameterValue = ((INullable) value).IsNull ? DBNull.Value : value;

			SqlParameter sqlParameter = (SqlParameter)cmd.Parameters[index];
			sqlParameter.SqlDbType = SqlDbType.Udt;
			sqlParameter.UdtTypeName = "geometry";
			sqlParameter.Value = parameterValue;
		}
	}
}
