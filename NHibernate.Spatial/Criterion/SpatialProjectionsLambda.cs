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
using System.Linq.Expressions;
using NHibernate.Impl;

namespace NHibernate.Spatial.Criterion
{
	/// <summary>
	/// Factory class for NHibernate query projections
	/// </summary>
	/// <remarks>
	/// In the GIS context, this class name could be misleading,
	/// but it has nothing to do with cartographic planar projections.
	/// </remarks>
	public static partial class SpatialProjections
	{

		private static string GetPropertyName<T>(Expression<Func<T, object>> expression)
		{
			return ExpressionProcessor.FindMemberExpression(expression.Body);
		}

		#region Aggregates

		/// <summary>
		/// Aggregates collection of the specified property.
		/// </summary>
		/// <param name="expression">Name of the property.</param>
		/// <returns></returns>
		public static SpatialAggregateProjection Collect<T>(Expression<Func<T, object>> expression)
		{
			return Collect(GetPropertyName(expression));
		}

		/// <summary>
		/// Aggregates envelope of the specified property.
		/// </summary>
		/// <param name="expression">Name of the property.</param>
		/// <returns></returns>
		public static SpatialAggregateProjection Envelope<T>(Expression<Func<T, object>> expression)
		{
			return Envelope(GetPropertyName(expression));
		}

		/// <summary>
		/// Aggregates intersection of the specified property.
		/// </summary>
		/// <param name="expression">Name of the property.</param>
		/// <returns></returns>
		public static SpatialAggregateProjection Intersection<T>(Expression<Func<T, object>> expression)
		{
			return Intersection(GetPropertyName(expression));
		}

		/// <summary>
		/// Aggregates union of the specified property.
		/// </summary>
		/// <param name="expression">Name of the property.</param>
		/// <returns></returns>
		public static SpatialAggregateProjection Union<T>(Expression<Func<T, object>> expression)
		{
			return Union(GetPropertyName(expression));
		}

		#endregion

		#region Analysis

		/// <summary>
		/// Buffers the specified property.
		/// </summary>
		/// <param name="expression">Name of the property.</param>
		/// <param name="distance">Name of another property.</param>
		/// <returns></returns>
		public static SpatialProjection Buffer<T>(Expression<Func<T, object>> expression, double distance)
		{
			return Buffer(GetPropertyName(expression), distance);
		}

		/// <summary>
		/// ConvexHull for the specified property.
		/// </summary>
		/// <param name="expression">Name of the property.</param>
		/// <returns></returns>
		public static SpatialProjection ConvexHull<T>(Expression<Func<T, object>> expression)
		{
			return ConvexHull(GetPropertyName(expression));
		}

		/// <summary>
		/// Difference of the specified properties.
		/// </summary>
		/// <param name="expression">Name of the property.</param>
		/// <param name="anotherExpression">Name of another property.</param>
		/// <returns></returns>
		public static SpatialProjection Difference<T>(Expression<Func<T, object>> expression, Expression<Func<T, object>> anotherExpression)
		{
			return Difference(GetPropertyName(expression), GetPropertyName(anotherExpression));
		}

		/// <summary>
		/// Distance of the specified properties.
		/// </summary>
		/// <param name="expression">Name of the property.</param>
		/// <param name="anotherExpression">Name of another property.</param>
		/// <returns></returns>
		public static SpatialProjection Distance<T>(Expression<Func<T, object>> expression, Expression<Func<T, object>> anotherExpression)
		{
			return Distance(GetPropertyName(expression), GetPropertyName(anotherExpression));
		}

		/// <summary>
		/// Intersection of the specified properties.
		/// </summary>
		/// <param name="expression">Name of the property.</param>
		/// <param name="anotherExpression">Name of another property.</param>
		/// <returns></returns>
		public static SpatialProjection Intersection<T>(Expression<Func<T, object>> expression, Expression<Func<T, object>> anotherExpression)
		{
			return Intersection(GetPropertyName(expression), GetPropertyName(anotherExpression));
		}

		/// <summary>
		/// Symmetric difference of the specified properties.
		/// </summary>
		/// <param name="expression">Name of the property.</param>
		/// <param name="anotherExpression">Name of another property.</param>
		/// <returns></returns>
		public static SpatialProjection SymDifference<T>(Expression<Func<T, object>> expression, Expression<Func<T, object>> anotherExpression)
		{
			return SymDifference(GetPropertyName(expression), GetPropertyName(anotherExpression));
		}

		/// <summary>
		/// Union of the specified properties.
		/// </summary>
		/// <param name="expression">Name of the property.</param>
		/// <param name="anotherExpression">Name of another property.</param>
		/// <returns></returns>
		public static SpatialProjection Union<T>(Expression<Func<T, object>> expression, Expression<Func<T, object>> anotherExpression)
		{
			return Union(GetPropertyName(expression), GetPropertyName(anotherExpression));
		}

		#endregion

		#region Relations

		/// <summary>
		/// Determines whether the specified geometry property contains another geometry property.
		/// </summary>
		/// <param name="expression">Name of the property.</param>
		/// <param name="anotherExpression">Name of another property.</param>
		/// <returns></returns>
		public static SpatialProjection Contains<T>(Expression<Func<T, object>> expression, Expression<Func<T, object>> anotherExpression)
		{
			return Contains(GetPropertyName(expression), GetPropertyName(anotherExpression));
		}

		/// <summary>
		/// Determines whether the specified geometry property is covered by another geometry property.
		/// </summary>
		/// <param name="expression">Name of the property.</param>
		/// <param name="anotherExpression">Name of another property.</param>
		/// <returns></returns>
		public static SpatialProjection CoveredBy<T>(Expression<Func<T, object>> expression, Expression<Func<T, object>> anotherExpression)
		{
			return CoveredBy(GetPropertyName(expression), GetPropertyName(anotherExpression));
		}

		/// <summary>
		/// Determines whether the specified geometry property covers another geometry property.
		/// </summary>
		/// <param name="expression">Name of the property.</param>
		/// <param name="anotherExpression">Name of another property.</param>
		/// <returns></returns>
		public static SpatialProjection Covers<T>(Expression<Func<T, object>> expression, Expression<Func<T, object>> anotherExpression)
		{
			return Covers(GetPropertyName(expression), GetPropertyName(anotherExpression));
		}

		/// <summary>
		/// Determines whether the specified geometry property crosses another geometry property.
		/// </summary>
		/// <param name="expression">Name of the property.</param>
		/// <param name="anotherExpression">Name of another property.</param>
		/// <returns></returns>
		public static SpatialProjection Crosses<T>(Expression<Func<T, object>> expression, Expression<Func<T, object>> anotherExpression)
		{
			return Crosses(GetPropertyName(expression), GetPropertyName(anotherExpression));
		}

		/// <summary>
		/// Determines whether the specified geometry property is disjoint with another geometry property.
		/// </summary>
		/// <param name="expression">Name of the property.</param>
		/// <param name="anotherExpression">Name of another property.</param>
		/// <returns></returns>
		public static SpatialProjection Disjoint<T>(Expression<Func<T, object>> expression, Expression<Func<T, object>> anotherExpression)
		{
			return Disjoint(GetPropertyName(expression), GetPropertyName(anotherExpression));
		}

		/// <summary>
		/// Determines whether the specified geometry property equals to another geometry property.
		/// </summary>
		/// <param name="expression">Name of the property.</param>
		/// <param name="anotherExpression">Name of another property.</param>
		/// <returns></returns>
		public static SpatialProjection Equals<T>(Expression<Func<T, object>> expression, Expression<Func<T, object>> anotherExpression)
		{
			return Equals(GetPropertyName(expression), GetPropertyName(anotherExpression));
		}

		/// <summary>
		/// Determines whether the specified geometry property intersects another geometry property.
		/// </summary>
		/// <param name="expression">Name of the property.</param>
		/// <param name="anotherExpression">Name of another property.</param>
		/// <returns></returns>
		public static SpatialProjection Intersects<T>(Expression<Func<T, object>> expression, Expression<Func<T, object>> anotherExpression)
		{
			return Intersects(GetPropertyName(expression), GetPropertyName(anotherExpression));
		}

		/// <summary>
		/// Determines whether the specified geometry property overlaps another geometry property.
		/// </summary>
		/// <param name="expression">Name of the property.</param>
		/// <param name="anotherExpression">Name of another property.</param>
		/// <returns></returns>
		public static SpatialProjection Overlaps<T>(Expression<Func<T, object>> expression, Expression<Func<T, object>> anotherExpression)
		{
			return Overlaps(GetPropertyName(expression), GetPropertyName(anotherExpression));
		}

		/// <summary>
		/// Determines whether the specified geometry property touches another geometry property.
		/// </summary>
		/// <param name="expression">Name of the property.</param>
		/// <param name="anotherExpression">Name of another property.</param>
		/// <returns></returns>
		public static SpatialProjection Touches<T>(Expression<Func<T, object>> expression, Expression<Func<T, object>> anotherExpression)
		{
			return Touches(GetPropertyName(expression), GetPropertyName(anotherExpression));
		}

		/// <summary>
		/// Determines whether the specified geometry property is within another geometry property.
		/// </summary>
		/// <param name="expression">Name of the property.</param>
		/// <param name="anotherExpression">Name of another property.</param>
		/// <returns></returns>
		public static SpatialProjection Within<T>(Expression<Func<T, object>> expression, Expression<Func<T, object>> anotherExpression)
		{
			return Within(GetPropertyName(expression), GetPropertyName(anotherExpression));
		}


		/// <summary>
		/// Determines whether the specified geometry property relates to another geometry property.
		/// </summary>
		/// <param name="expression">Name of the property.</param>
		/// <param name="anotherExpression">Name of another property.</param>
		/// <returns></returns>
		public static SpatialProjection Relate<T>(Expression<Func<T, object>> expression, Expression<Func<T, object>> anotherExpression)
		{
			return Relate(GetPropertyName(expression), GetPropertyName(anotherExpression));
		}

		/// <summary>
		/// Determines whether the specified geometry property relates to another geometry property.
		/// </summary>
		/// <param name="expression">Name of the property.</param>
		/// <param name="anotherExpression">Name of another property.</param>
		/// <param name="pattern">The pattern.</param>
		/// <returns></returns>
		public static SpatialProjection Relate<T>(Expression<Func<T, object>> expression, Expression<Func<T, object>> anotherExpression, string pattern)
		{
			return Relate(GetPropertyName(expression), GetPropertyName(anotherExpression), pattern);
		}

		#endregion

		#region Validations

		/// <summary>
		/// Determines whether the specified geometry property is closed.
		/// </summary>
		/// <param name="expression">Name of the property.</param>
		/// <returns></returns>
		public static SpatialProjection IsClosed<T>(Expression<Func<T, object>> expression)
		{
			return IsClosed(GetPropertyName(expression));
		}

		/// <summary>
		/// Determines whether the specified geometry property is empty.
		/// </summary>
		/// <param name="expression">Name of the property.</param>
		/// <returns></returns>
		public static SpatialProjection IsEmpty<T>(Expression<Func<T, object>> expression)
		{
			return IsEmpty(GetPropertyName(expression));
		}

		/// <summary>
		/// Determines whether the specified geometry property is ring.
		/// </summary>
		/// <param name="expression">Name of the property.</param>
		/// <returns></returns>
		public static SpatialProjection IsRing<T>(Expression<Func<T, object>> expression)
		{
			return IsRing(GetPropertyName(expression));
		}

		/// <summary>
		/// Determines whether the specified geometry property is simple.
		/// </summary>
		/// <param name="expression">Name of the property.</param>
		/// <returns></returns>
		public static SpatialProjection IsSimple<T>(Expression<Func<T, object>> expression)
		{
			return IsSimple(GetPropertyName(expression));
		}

		/// <summary>
		/// Determines whether the specified geometry property is valid.
		/// </summary>
		/// <param name="expression">Name of the property.</param>
		/// <returns></returns>
		public static SpatialProjection IsValid<T>(Expression<Func<T, object>> expression)
		{
			return IsValid(GetPropertyName(expression));
		}

		#endregion

		#region Functions

		/// <summary>
		/// Transforms the coordinate reference system of the specified geometry property.
		/// </summary>
		/// <param name="expression">Name of the property.</param>
		/// <param name="srid">The srid.</param>
		/// <returns></returns>
		public static SpatialProjection Transform<T>(Expression<Func<T, object>> expression, int srid)
		{
			return Transform(GetPropertyName(expression), srid);
		}

		#endregion

	}

}
