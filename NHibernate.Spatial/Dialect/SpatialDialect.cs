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

using System;
using NHibernate.Engine;
using NHibernate.Type;

namespace NHibernate.Spatial.Dialect
{
	/// <summary>
	/// 
	/// </summary>
	public static class SpatialDialect
	{
		/// <summary>
		/// Function prefix for HQL queries.
		/// </summary>
		public const string HqlPrefix = "NHSP."; // Simply the acronym of "NHibernate.Spatial"

		/// <summary>
		/// Fuction prefix for dialects implementing ISO/IEC SQL/MM Spatial.
		/// </summary>
		public const string IsoPrefix = "ST_";

		/// <summary>
		/// The last spatial dialect instantiated in the current AppDomain.
		/// </summary>
		/// 
		/// <remarks>
		/// A class implementing <see cref="ISpatialDialect" /> must assign 
		/// itself to this field in its constructor.
		/// 
		/// <para>
		/// It has not been tested yet the behavior of this field in an
		/// environment of multiple spatial dialects instantiated, and 
		/// geometry columns associated, in the same application domain.
		/// Hopefully, the corresponding dialect always will be instantiated
		/// before the geometry type, but this assumption has to be analyzed 
		/// in the NHibernate source code and tested in a real scenario.
		/// </para>
		/// 
		/// <para>
		/// This may be an ugly trick but it could be avoided if NHibernate
		/// provides more context of the IUserType instatiation. For example, 
		/// providing the current NHibernate.Cfg.Settings object through
		/// a new interface method.
		/// </para>
		/// 
		/// </remarks>
		private static ISpatialDialect lastInstantiated;

		public static ISpatialDialect LastInstantiated
		{
			get { return lastInstantiated; }
			set
			{
				lastInstantiated = value;

				// Set Linq-to-HQL generator registry by default.
				((NHibernate.Dialect.Dialect)value)
					.DefaultProperties["linqtohql.generatorsregistry"] =
					typeof (Linq.Functions.SpatialLinqToHqlGeneratorsRegistry)
						.AssemblyQualifiedName;
			}
		}

		#region Utility methods

		/// <summary>
		/// Returns the geometry type associated to the session factory.
		/// </summary>
		/// <param name="sessionFactory"></param>
		/// <returns></returns>
		public static IType GeometryTypeOf(ISessionFactory sessionFactory)
		{
			return ((ISpatialDialect)((ISessionFactoryImplementor)sessionFactory).Dialect).GeometryType;
		}

		/// <summary>
		/// Returns the geometry type associated to the session.
		/// </summary>
		/// <param name="session"></param>
		/// <returns></returns>
		public static IType GeometryTypeOf(ISession session)
		{
			return ((ISpatialDialect)((ISessionFactoryImplementor)session.SessionFactory).Dialect).GeometryType;
		}

		#endregion

	}
}
