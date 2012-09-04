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
using System.Data.SqlTypes;
using NHibernate.Spatial.Oracle;
using NHibernate.SqlTypes;
using NHibernate.Type;
using Oracle.DataAccess.Client;

namespace NHibernate.Spatial.Oracle
{
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

		public override object NullSafeGet(IDataReader rs, string name)
		{
			object value = base.NullSafeGet(rs, name);
			return value ?? SdoGeometry.Null;
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
			return (SdoGeometry)rs[index];
		}

		public override object FromStringValue(string xml)
		{
			// TODO:
			//return SdoGeometry.STGeomFromText(new SqlChars(xml), 0);
			return null;
		}

		public override System.Type ReturnedClass
		{
			get { return typeof(SdoGeometry); }
		}


		public override void Set(IDbCommand cmd, object value, int index)
		{
			object parameterValue = (value as INullable).IsNull ? DBNull.Value : value;

			OracleParameter oracleParameter = (OracleParameter)cmd.Parameters[index];
			oracleParameter.OracleDbType = OracleDbType.Object;
			oracleParameter.UdtTypeName = "MDSYS.SDO_GEOMETRY";
			oracleParameter.Value = parameterValue;
		}
	}
}