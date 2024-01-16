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

using NHibernate.Criterion;
using NHibernate.Spatial.Dialect;
using NHibernate.SqlCommand;
using NHibernate.Type;
using System;

namespace NHibernate.Spatial.Criterion
{
    /// <summary>
    ///
    /// </summary>
    [Serializable]
    public class SpatialValidationProjection : SpatialProjection
    {
        private readonly SpatialValidation validation;

        /// <summary>
        /// Initializes a new instance of the <see cref="SpatialValidationProjection"/> class.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="validation">The validation.</param>
        public SpatialValidationProjection(string propertyName, SpatialValidation validation)
            : base(propertyName)
        {
            this.validation = validation;
        }

        /// <summary>
        /// Gets the types.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="criteriaQuery">The criteria query.</param>
        /// <returns></returns>
        public override IType[] GetTypes(ICriteria criteria, ICriteriaQuery criteriaQuery)
        {
            return new IType[] { NHibernateUtil.Boolean };
        }

        /// <summary>
        /// Render the SQL Fragment.
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="position"></param>
        /// <param name="criteriaQuery"></param>
        /// <returns></returns>
        public override SqlString ToSqlString(ICriteria criteria, int position, ICriteriaQuery criteriaQuery)
        {
            var spatialDialect = (ISpatialDialect) criteriaQuery.Factory.Dialect;
            string column1 = criteriaQuery.GetColumn(criteria, propertyName);
            var sqlString = spatialDialect.GetSpatialValidationString(column1, validation, false);
            return new SqlStringBuilder()
                .Add(sqlString)
                .Add(" as y")
                .Add(position.ToString())
                .Add("_")
                .ToSqlString();
        }
    }
}
