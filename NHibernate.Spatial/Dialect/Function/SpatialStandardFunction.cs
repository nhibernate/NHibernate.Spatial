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

using System.Collections;
using NHibernate.Type;
using NHibernate.Engine;
using NHibernate.Dialect.Function;
using NHibernate.SqlCommand;

namespace NHibernate.Spatial.Dialect.Function
{
	/// <summary>
	/// 
	/// </summary>
	public class SpatialStandardFunction : ISQLFunction
	{
		private readonly IType returnType;
		protected readonly string name;

		/// <summary>
		/// Initializes a new instance of the <see cref="SpatialStandardFunction"/> class.
		/// </summary>
		/// <param name="name">The name.</param>
		public SpatialStandardFunction(string name)
		{
			this.name = name;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SpatialStandardFunction"/> class.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="typeValue">The type value.</param>
		public SpatialStandardFunction(string name, IType typeValue)
			: this(name)
		{
			this.returnType = typeValue;
		}

		#region ISQLFunction Members

		public virtual IType ReturnType(IType columnType, IMapping mapping)
		{
			return (returnType == null) ? columnType : returnType;
		}

		/// <summary>
		/// Does this function have any arguments?
		/// </summary>
		/// <value></value>
		public bool HasArguments
		{
			get { return true; }
		}

		/// <summary>
		/// If there are no arguments, are parens required?
		/// </summary>
		/// <value></value>
		public bool HasParenthesesIfNoArguments
		{
			get { return true; }
		}

		/// <summary>
		/// Render the function call as SQL.
		/// </summary>
		/// <param name="args">List of arguments</param>
		/// <param name="factory"></param>
		/// <returns>SQL fragment for the function.</returns>
		public virtual SqlString Render(IList args, ISessionFactoryImplementor factory)
		{
			SqlStringBuilder builder = new SqlStringBuilder();
			builder.Add(name);
			builder.Add("(");
			for (int i = 0; i < args.Count; i++)
			{
				builder.AddObject(args[i]);
				if (i < (args.Count - 1))
				{
					builder.Add(", ");
				}
			}
			builder.Add(")");
			return builder.ToSqlString();
		}

		#endregion

		/// <summary>
		/// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
		/// </returns>
		public override string ToString()
		{
			return name;
		}

	}
}
