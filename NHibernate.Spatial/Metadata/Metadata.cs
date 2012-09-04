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

using NHibernate.Cfg;
using NHibernate.Engine;
using NHibernate.Spatial.Dialect;

namespace NHibernate.Spatial.Metadata
{
	/// <summary>
	/// 
	/// </summary>
	public enum MetadataClass
	{
		/// <summary>
		/// 
		/// </summary>
		GeometryColumn,
		/// <summary>
		/// 
		/// </summary>
		SpatialReferenceSystem,
	}

	public static class Metadata
	{
		/// <summary>
		/// Add a spatial metadata class mapping to NHibernate configuration.
		/// </summary>
		/// <param name="configuration">The configuration.</param>
		/// <param name="clazz">The clazz.</param>
		/// <remarks>
		/// DO NOT add metadata class mappings when using the SchemaExport utility.
		/// You could lose all contents of those tables.
		/// </remarks>
		public static void AddMapping(Configuration configuration, MetadataClass clazz)
		{
			NHibernate.Dialect.Dialect dialect = NHibernate.Dialect.Dialect.GetDialect(configuration.Properties);

			string resource = typeof(Metadata).Namespace
				+ "."
				+ clazz.ToString()
				+ "."
				+ dialect.GetType().Name
				+ ".hbm.xml";

			configuration.AddResource(resource, dialect.GetType().Assembly);
		}

        /// <summary>
        /// Gets a value indicating whether the session supports spatial metadata.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if it supports spatial metadata; otherwise, <c>false</c>.
        /// </value>
        /// <param name="session">The session</param>
        /// <param name="metadataClass">The metadata class</param>
        public static bool SupportsSpatialMetadata(ISession session, MetadataClass metadataClass)
        {
            ISpatialDialect spatialDialect = (ISpatialDialect)((ISessionFactoryImplementor)session.SessionFactory).Dialect;
            return spatialDialect.SupportsSpatialMetadata(metadataClass);
        }
	}
}
