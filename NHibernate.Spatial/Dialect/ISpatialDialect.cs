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

using NHibernate.Spatial.Metadata;
using NHibernate.Spatial.Type;
using NHibernate.SqlCommand;
using NHibernate.Type;

namespace NHibernate.Spatial.Dialect
{
	/// <summary>
	/// 
	/// </summary>
	public interface ISpatialDialect
	{
		/// <summary>
		/// Gets the type of the geometry.
		/// </summary>
		/// <value>The type of the geometry.</value>
		IType GeometryType { get; }

		/// <summary>
		/// Creates the geometry user type.
		/// </summary>
		/// <returns></returns>
		IGeometryUserType CreateGeometryUserType();

		/// <summary>
		/// Gets the spatial transform string.
		/// </summary>
		/// <param name="geometry">The geometry.</param>
		/// <param name="srid">The srid.</param>
		/// <returns></returns>
		SqlString GetSpatialTransformString(object geometry, int srid);

		/// <summary>
		/// Gets the spatial aggregate string.
		/// </summary>
		/// <param name="geometry">The geometry.</param>
		/// <param name="aggregate">The aggregate.</param>
		/// <returns></returns>
		SqlString GetSpatialAggregateString(object geometry, SpatialAggregate aggregate);

		/// <summary>
		/// Gets the spatial analysis string.
		/// </summary>
		/// <param name="geometry">The geometry.</param>
		/// <param name="analysis">The analysis.</param>
		/// <param name="extraArgument">The extra argument.</param>
		/// <returns></returns>
		SqlString GetSpatialAnalysisString(object geometry, SpatialAnalysis analysis, object extraArgument);

		/// <summary>
		/// Gets the spatial validation string.
		/// </summary>
		/// <param name="geometry">The geometry.</param>
		/// <param name="validation">The validation.</param>
		/// <param name="criterion">if set to <c>true</c> [criterion].</param>
		/// <returns></returns>
		SqlString GetSpatialValidationString(object geometry, SpatialValidation validation, bool criterion);

		/// <summary>
		/// Gets the spatial relate string.
		/// </summary>
		/// <param name="geometry">The geometry.</param>
		/// <param name="anotherGeometry">Another geometry.</param>
		/// <param name="pattern">The pattern.</param>
		/// <param name="isStringPattern">if set to <c>true</c> [is string pattern].</param>
		/// <param name="criterion">if set to <c>true</c> [criterion].</param>
		/// <returns></returns>
		SqlString GetSpatialRelateString(object geometry, object anotherGeometry, object pattern, bool isStringPattern, bool criterion);

		/// <summary>
		/// It builds a SQL spatial relation expression.
		/// </summary>
		/// <remarks>
		/// One parameter placeholder must be added to the SqlString.
		/// </remarks>
		/// <param name="geometry">SQL expression returning a geometry</param>
		/// <param name="relation">Spatial relation</param>
		/// <param name="anotherGeometry">A string SQL geometry expression or a Parameter.Placeholder</param>
		/// <returns>A <c><SqlString/c> object containing
		/// a SQL spatial relation expression</returns>
		SqlString GetSpatialRelationString(object geometry, SpatialRelation relation, object anotherGeometry, bool criterion);

		/// <summary>
		/// It builds a SQL spatial filter expression.
		/// </summary>
		/// <remarks>
		/// One parameter placeholder must be added to the SqlString.
		/// </remarks>
		/// <returns>A <c><SqlString/c> object containing
		/// a SQL spatial relation expression</returns>
		SqlString GetSpatialFilterString(string tableAlias, string geometryColumnName, string primaryKeyColumnName, string tableName);

		/// <summary>
		/// Gets the spatial create string.
		/// </summary>
		/// <param name="schema">The schema.</param>
		/// <returns></returns>
		string GetSpatialCreateString(string schema);

        /// <summary>
        /// Gets the spatial create string.
        /// </summary>
        /// <param name="schema">The schema.</param>
        /// <param name="table">The table.</param>
        /// <param name="column">The column.</param>
        /// <param name="srid">The srid.</param>
        /// <param name="subtype">The subtype.</param>
        /// <param name="dimension">[3DIS] The dimension</param>
        /// <returns></returns>
        string GetSpatialCreateString(string schema, string table, string column, int srid, string subtype, int dimension);

		/// <summary>
		/// Gets the spatial drop string.
		/// </summary>
		/// <param name="schema">The schema.</param>
		/// <returns></returns>
		string GetSpatialDropString(string schema);

		/// <summary>
		/// Gets the spatial drop string.
		/// </summary>
		/// <param name="schema">The schema.</param>
		/// <param name="table">The table.</param>
		/// <param name="column">The column.</param>
		/// <returns></returns>
		string GetSpatialDropString(string schema, string table, string column);

		/// <summary>
		/// Gets a value indicating whether it supports spatial metadata.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if it supports spatial metadata; otherwise, <c>false</c>.
		/// </value>
		bool SupportsSpatialMetadata(MetadataClass metadataClass);


		// TODO: Use ISessionFactory.ConnectionProvider.Driver.MultipleQueriesSeparator
		string MultipleQueriesSeparator { get; }
	}
}
