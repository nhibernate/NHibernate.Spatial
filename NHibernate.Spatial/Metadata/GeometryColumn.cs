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

namespace NHibernate.Spatial.Metadata
{
    /// <summary>
    ///
    /// </summary>
    [Serializable]
    public class GeometryColumn
    {
        // caching the hashCode initalizing to 0 since in most cases,
        // hashing table name and column name is enough. we will set it
        // whenever the table name or column name will change
        private int hashCode;

        private string tableCatalog;

        private string tableSchema;

        private string tableName;

        private string name;

        private int srid;

        private string subtype;

        private int dimension;

        /// <summary>
        /// Gets or sets the table catalog.
        /// </summary>
        /// <value>The table catalog.</value>
        public virtual string TableCatalog
        {
            get => tableCatalog;
            set => tableCatalog = value;
        }

        /// <summary>
        /// Gets or sets the table schema.
        /// </summary>
        /// <value>The table schema.</value>
        public virtual string TableSchema
        {
            get => tableSchema;
            set => tableSchema = value;
        }

        /// <summary>
        /// Gets or sets the name of the table.
        /// </summary>
        /// <value>The name of the table.</value>
        public virtual string TableName
        {
            get => tableName;
            set
            {
                tableName = value;
                SetHashCode();
            }
        }

        /// <summary>
        /// Gets or sets the name of the geometry column.
        /// </summary>
        /// <value>The name.</value>
        public virtual string Name
        {
            get => name;
            set
            {
                name = value;
                SetHashCode();
            }
        }

        /// <summary>
        /// Gets or sets the SRID.
        /// </summary>
        /// <value>The SRID.</value>
        public virtual int SRID
        {
            get => srid;
            set => srid = value;
        }

        /// <summary>
        /// Gets or sets the geometry subtype.
        /// </summary>
        /// <value>The subtype.</value>
        public virtual string Subtype
        {
            get => subtype;
            set => subtype = value;
        }

        /// <summary>
        /// Gets or sets the geometry dimension.
        /// </summary>
        /// <value>The dimension.</value>
        public virtual int Dimension
        {
            get => dimension;
            set => dimension = value;
        }

        private void SetHashCode()
        {
            hashCode = (TableName + '.' + Name).GetHashCode();
        }

        #region System.Object Members

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>.</param>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
        /// </returns>
        /// <exception cref="T:System.NullReferenceException">The <paramref name="obj"/> parameter is null.</exception>
        public override bool Equals(object obj)
        {
            var other = obj as GeometryColumn;
            return other != null
                   && TableCatalog == other.TableCatalog
                   && TableSchema == other.TableSchema
                   && TableName == other.TableName
                   && Name == other.Name;
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        public override int GetHashCode()
        {
            return hashCode;
        }

        #endregion System.Object Members
    }
}
