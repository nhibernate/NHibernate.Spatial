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
using NHibernate.Criterion;
using NHibernate.Engine;
using NHibernate.Spatial.Dialect;
using NHibernate.SqlCommand;
using NHibernate.Type;
using System;
using System.Linq;

namespace NHibernate.Spatial.Criterion
{
    /// <summary>
    ///
    /// </summary>
    [Serializable]
    public class SpatialAnalysisProjection : SpatialProjection
    {
        private readonly SpatialAnalysis analysis;
        private readonly string anotherPropertyName;

        //private readonly object[] arguments;
        private readonly double value;

        private readonly Geometry geometry;

        /// <summary>
        /// Initializes a new instance of the <see cref="SpatialAnalysisProjection"/> class.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="analysis">The analysis.</param>
        /// <param name="anotherPropertyName">Name of another property.</param>
        public SpatialAnalysisProjection(string propertyName, SpatialAnalysis analysis, string anotherPropertyName)
            : base(propertyName)
        {
            this.analysis = analysis;
            this.anotherPropertyName = anotherPropertyName;
        }

        ///// <summary>
        ///// Initializes a new instance of the <see cref="SpatialAnalysisProjection"/> class.
        ///// </summary>
        ///// <param name="propertyName">Name of the property.</param>
        ///// <param name="analysis">The analysis.</param>
        ///// <param name="arguments">The arguments.</param>
        //public SpatialAnalysisProjection(string propertyName, SpatialAnalysis analysis, params object[] arguments)
        //    : base(propertyName)
        //{
        //    this.analysis = analysis;
        //    this.arguments = arguments;
        //}

        /// <summary>
        /// Initializes a new instance of the <see cref="SpatialAnalysisProjection"/> class.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="analysis">The analysis.</param>
        public SpatialAnalysisProjection(string propertyName, SpatialAnalysis analysis)
            : base(propertyName)
        {
            this.analysis = analysis;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SpatialAnalysisProjection"/> class.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="analysis">The analysis.</param>
        /// <param name="value"></param>
        public SpatialAnalysisProjection(string propertyName, SpatialAnalysis analysis, double value)
            : base(propertyName)
        {
            this.analysis = analysis;
            this.value = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SpatialAnalysisProjection"/> class.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="analysis">The analysis.</param>
        /// <param name="geometry">The arguments.</param>
        public SpatialAnalysisProjection(string propertyName, SpatialAnalysis analysis, Geometry geometry)
            : base(propertyName)
        {
            this.analysis = analysis;
            this.geometry = geometry;
        }

        /// <summary>
        /// Gets the types.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="criteriaQuery">The criteria query.</param>
        /// <returns></returns>
        public override IType[] GetTypes(ICriteria criteria, ICriteriaQuery criteriaQuery)
        {
            if (this.analysis == SpatialAnalysis.Distance)
            {
                return new IType[] { NHibernateUtil.Double };
            }
            return base.GetTypes(criteria, criteriaQuery);
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
            var spatialDialect = (ISpatialDialect)criteriaQuery.Factory.Dialect;
            string column1 = criteriaQuery.GetColumn(criteria, this.propertyName);
            SqlString sqlString;

            if (IsBinaryOperation())
            {
                if (geometry != null)
                {
                    Parameter[] parameters = criteriaQuery.NewQueryParameter(this.GetTypedValues(criteria, criteriaQuery)[0]).ToArray();
                    sqlString = spatialDialect.GetSpatialAnalysisString(column1, this.analysis, parameters.Single());
                }
                else
                {
                    string column2 = criteriaQuery.GetColumn(criteria, this.anotherPropertyName);
                    sqlString = spatialDialect.GetSpatialAnalysisString(column1, this.analysis, column2);
                }
            }
            else
            {
                sqlString = spatialDialect.GetSpatialAnalysisString(column1, this.analysis, null);
            }

            return new SqlStringBuilder()
                .Add(sqlString)
                .Add(" as y")
                .Add(position.ToString())
                .Add("_")
                .ToSqlString();
        }

        /// <summary>
        /// Determines whether is binary operation.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if is binary operation; otherwise, <c>false</c>.
        /// </returns>
        private bool IsBinaryOperation()
        {
            return (analysis != SpatialAnalysis.Buffer && analysis != SpatialAnalysis.ConvexHull);
        }

        /// <summary>
        /// Return typed values for all parameters in the rendered SQL fragment
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="criteriaQuery"></param>
        /// <returns>
        /// An array of TypedValues for the Expression.
        /// </returns>
        public override TypedValue[] GetTypedValues(ICriteria criteria, ICriteriaQuery criteriaQuery)
        {
            return new[] { criteriaQuery.GetTypedValue(criteria, this.propertyName, this.geometry) };
        }
    }
}