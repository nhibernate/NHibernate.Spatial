using NHibernate.Impl;
using System;
using System.Linq.Expressions;

namespace NHibernate.Spatial.Criterion.Lambda
{
    public static class QueryOverSpatialExtensions
    {
        public static QueryOverSpatialRestrictionBuilder<TRoot, TSubType>
            WhereSpatialRestrictionOn<TRoot, TSubType>(
            this IQueryOver<TRoot, TSubType> root,
            Expression<Func<TSubType, object>> expression)
        {
            return new QueryOverSpatialRestrictionBuilder<TRoot, TSubType>(
                root,
                ExpressionProcessor.FindMemberExpression(expression.Body));
        }
    }
}