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

using NHibernate.SqlTypes;
using NHibernate.Type;
using System;
using System.Data;
using System.Data.Common;
using System.Text;
using NHibernate.Engine;

namespace NHibernate.Spatial.Type
{
    /// <summary>
    /// Maps a <see cref="DbType.Binary" /> column to a <see cref="System.String" />.
    /// </summary>
    /// <remarks>
    /// AsText and GeometryType returns an special kind of string that
    /// MySql.Data reads as a DbType.Binary which needs to be converted to String.
    /// </remarks>
    [Serializable]
    public class MySQLGeometryStringType : ImmutableType, IDiscriminatorType
    {
        public static readonly MySQLGeometryStringType Instance = new MySQLGeometryStringType();

        /// <summary></summary>
        internal MySQLGeometryStringType()
            : base(new BinarySqlType())
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="rs"></param>
        /// <param name="index"></param>
        /// <param name="session"></param>
        /// <returns></returns>
        public override object Get(DbDataReader rs, int index, ISessionImplementor session)
        {
            return Encoding.ASCII.GetString((byte[])rs[index]);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="rs"></param>
        /// <param name="name"></param>
        /// <param name="session"></param>
        /// <returns></returns>
        public override object Get(DbDataReader rs, string name, ISessionImplementor session)
        {
            return Encoding.ASCII.GetString((byte[])rs[name]);
        }

        /// <summary></summary>
        public override System.Type ReturnedClass
        {
            get { return typeof(string); }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="st"></param>
        /// <param name="value"></param>
        /// <param name="index"></param>
        /// <param name="session"></param>
        public override void Set(DbCommand st, object value, int index, ISessionImplementor session)
        {
            IDataParameter parm = st.Parameters[index] as IDataParameter;
            parm.Value = value;
        }

        /// <summary></summary>
        public override string Name
        {
            get { return "String"; }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public string ObjectToSQLString(object value)
        {
            return "'" + (string)value + "'";
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public object StringToObject(string xml)
        {
            return xml;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [Obsolete("This method has no more usages and will be removed in a future version. Override ToLoggableString instead.")]
        public override string ToString(object value)
        {
            return (string)value;
        }

        [Obsolete("This method has no more usages and will be removed in a future version.")]
        public override object FromStringValue(string xml)
        {
            return xml;
        }

        public string ObjectToSQLString(object value, NHibernate.Dialect.Dialect dialect)
        {
            return (string)value;
        }
    }
}