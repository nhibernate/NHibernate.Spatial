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

using MySql.Data.MySqlClient;
using MySql.Data.Types;
using NHibernate.SqlTypes;
using NHibernate.Type;
using System;
using System.Data;
using System.Data.Common;
using NHibernate.Engine;

namespace NHibernate.Spatial.Type
{
	/// <summary>
	/// This class maps MySQLDbType.Geometry to and from Geometry
	/// </summary>
	[Serializable]
	public class MySQL57GeometryAdapterType : ImmutableType
	{
		public MySQL57GeometryAdapterType()
			: base(SqlTypeFactory.Byte) // Any arbitrary type can be passed as parameter
		{
		}

		public override object NullSafeGet(DbDataReader rs, string name, ISessionImplementor session)
		{
			object value = base.NullSafeGet(rs, name, session);
			return value ?? default(MySqlGeometry);
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
			return new MySqlGeometry(MySqlDbType.Geometry, (byte[])rs[index]);
		}

		[Obsolete("This method has no more usages and will be removed in a future version.")]
        public override object FromStringValue(string xml)
		{
			return MySqlGeometry.Parse(xml);
		}

		public override System.Type ReturnedClass
		{
			get { return typeof(MySqlGeometry); }
		}

		public override object NullSafeGet(DbDataReader rs, string[] names, ISessionImplementor session)
		{
			var result = base.NullSafeGet(rs, names, session);

			return result;
		}

		public override void Set(DbCommand cmd, object value, int index, ISessionImplementor session)
		{
			byte[] internalValue = ((MySqlGeometry)value).Value;

			var parameter = cmd.Parameters[index];

			parameter.DbType = DbType.Binary;

			// set the parameter value before the size check, since ODBC changes the size automatically
			parameter.Value = internalValue;

			// Avoid silent truncation which happens in ADO.NET if the parameter size is set.
			if (parameter.Size > 0 && internalValue.Length > parameter.Size)
				throw new HibernateException("The length of the byte[] value exceeds the length configured in the mapping/parameter.");
		}
	}
}
