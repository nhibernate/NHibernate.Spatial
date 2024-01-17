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

using NetTopologySuite.Geometries;
using NHibernate.Cfg;
using NHibernate.Engine;
using NHibernate.Mapping;
using NHibernate.Spatial.Dialect;
using NHibernate.Spatial.Type;
using NHibernate.Type;
using System;
using System.Collections.Generic;
using System.Text;

namespace NHibernate.Spatial.Mapping
{
    /// <summary>
    ///
    /// </summary>
    ///
    /// <example>
    ///
    /// Sample usage:
    ///
    /// Declare a geometry property specifying SRID (system reference ID):
    ///
    /// <code>
    ///      <class name="MyGeoTableA">
    ///          <property name="MyGeoColumn">
    ///              <type name="NHibernate.Spatial.Type.GeometryType, NHibernate.Spatial">
    ///                  <param name="srid">1234</param>
    ///              </type>
    ///          </property>
    ///      </class>
    ///      <class name="MyGeoTableB">
    ///          <property name="MyGeoColumn">
    ///              <type name="NHibernate.Spatial.Type.GeometryType, NHibernate.Spatial">
    ///                  <param name="subtype">POLYGON</param>
    ///              </type>
    ///          </property>
    ///      </class>
    /// </code>
    ///
    ///  * SpatialAuxiliaryDatabaseObject generates spatial schema for all configured classes
    ///
    /// <code>
    ///     <database-object>
    ///         <definition class="NHibernate.Spatial.Mapping.SpatialAuxiliaryDatabaseObject, NHibernate.Spatial" />
    ///     </database-object>
    /// </code>
    ///
    ///  * For example, for PostGIS the schema generation code would look like:
    ///
    /// <code>
    ///         ALTER TABLE dbo.MyGeoTableA DROP COLUMN MyGeoColumn;
    ///         SELECT AddGeometryColumn('dbo', 'MyGeoTableA', 'MyGeoColumn', 1234, 'GEOMETRY');
    ///
    ///         ALTER TABLE dbo.MyGeoTableB DROP COLUMN MyGeoColumn;
    ///         SELECT AddGeometryColumn('dbo', 'MyGeoTableB', 'MyGeoColumn', -1, 'POLYGON');
    /// </code>
    ///
    ///  (Compare the values defined in the property type parameters with generated SQL code)
    ///
    ///  </example>
    [Serializable]
    public class SpatialAuxiliaryDatabaseObject : AbstractAuxiliaryDatabaseObject
    {
        private readonly Configuration configuration;
        private Dictionary<string, int> SridMap;

        /// <summary>
        /// Initializes a new instance of the <see cref="SpatialAuxiliaryDatabaseObject"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <remarks>
        /// To programmatically configure the auxiliary object, we follow
        /// the advice of passing the configuration in the constructor given at:
        /// http://forum.hibernate.org/viewtopic.php?p=2320370#2320370
        /// However, this approach doesn't allow to configure it fully declaratively
        /// using the &lt;database-object&gt; element plus the option
        /// "hibernate.hbm2ddl.auto" set to "create" or "create-drop",
        /// Alternative solutions would be:
        /// 1) Add a Configuration property to IMapping, or
        /// 2) Add a Configuration parameter to IAuxiliaryDatabaseObject::SqlCreateString, or
        /// 3) Add a SetConfiguration method to IAuxiliaryDatabaseObject.
        /// The same should apply to IAuxiliaryDatabaseObject::SqlDropString.
        /// </remarks>
        public SpatialAuxiliaryDatabaseObject(Configuration configuration)
        {
            this.configuration = configuration;
            SridMap = new Dictionary<string, int>();
        }

        /// <summary>
        /// Creates SQL to create auxiliary database objects.
        /// </summary>
        /// <param name="dialect">The dialect.</param>
        /// <param name="mapping">The mapping.</param>
        /// <param name="defaultCatalog">The default catalog.</param>
        /// <param name="defaultSchema">The default schema.</param>
        /// <returns></returns>
        public override string SqlCreateString(NHibernate.Dialect.Dialect dialect, IMapping mapping, string defaultCatalog, string defaultSchema)
        {
            var spatialDialect = (ISpatialDialect) dialect;
            var builder = new StringBuilder();

            // Create general objects
            builder.Append(spatialDialect.GetSpatialCreateString(defaultSchema));

            // Create objects per column
            VisitGeometryColumns((tbl, col) => ColumnVisitorSQLCreate(tbl, col, builder, defaultSchema, spatialDialect));

            return builder.ToString();
        }

        /// <summary>
        /// Creates SQL to drop auxiliary database objects.
        /// </summary>
        /// <param name="dialect">The dialect.</param>
        /// <param name="defaultCatalog">The default catalog.</param>
        /// <param name="defaultSchema">The default schema.</param>
        /// <returns></returns>
        public override string SqlDropString(NHibernate.Dialect.Dialect dialect, string defaultCatalog, string defaultSchema)
        {
            var spatialDialect = (ISpatialDialect) dialect;
            var builder = new StringBuilder();

            // Drop objects per column
            VisitGeometryColumns((table, column) => builder.Append(spatialDialect.GetSpatialDropString(defaultSchema, table.Name, column.Name)));

            // Drop general objects
            builder.Append(spatialDialect.GetSpatialDropString(defaultSchema));

            return builder.ToString();
        }

        public void SetSRID(Table table, Column column, int srid)
        {
            SetSRID(table.Name, column.Name, srid);
        }

        public void SetSRID(string tableName, string columnName, int srid)
        {
            SridMap[tableName + "." + columnName] = srid;
        }

        //private delegate void VisitGeometryColumnDelegate(Table table, Column column);

        private void VisitGeometryColumns(Action<Table, Column> visitGeometryColumn)
        {
            // This would be quite illegal, because "configuration" is a private
            // field of Mapping class, the most probable implementation of IMapping.
            //
            //this.configuration = mapping.configuration;

            foreach (var persistentClass in configuration.ClassMappings)
            {
                var table = persistentClass.Table;
                foreach (var column in table.ColumnIterator)
                {
                    if (typeof(Geometry).IsAssignableFrom(column.Value.Type.ReturnedClass))
                    {
                        visitGeometryColumn(table, column);
                    }
                }
            }
        }

        private void ColumnVisitorSQLCreate(Table table, Column column, StringBuilder builder, string defaultSchema, ISpatialDialect spatialDialect)
        {
            var geometryType = (IGeometryUserType) ((CustomType) column.Value.Type).UserType;
            int srid = geometryType.SRID;
            string key = table.Name + "." + column.Name;
            if (SridMap.ContainsKey(key) && SridMap[key] > 0)
            {
                srid = SridMap[key];
            }
            string subtype = geometryType.Subtype;
            int dimension = geometryType.Dimension;

            builder.Append(spatialDialect.GetSpatialCreateString(defaultSchema, table.Name, column.Name, srid, subtype, dimension, column.IsNullable));
        }
    }
}
