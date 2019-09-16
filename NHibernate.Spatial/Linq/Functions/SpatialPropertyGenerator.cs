using NetTopologySuite.Geometries;
using NHibernate.Hql.Ast;
using NHibernate.Linq.Functions;
using NHibernate.Linq.Visitors;
using NHibernate.Spatial.Dialect;
using NHibernate.Util;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace NHibernate.Spatial.Linq.Functions
{
    public abstract class SpatialPropertyGenerator<TGeometry, TResult> : BaseHqlGeneratorForProperty
        where TGeometry : Geometry
    {
        protected SpatialPropertyGenerator(params Expression<Func<TGeometry, TResult>>[] expressions)
        {
            SupportedProperties = expressions.Select(o => ReflectHelper.GetProperty(o)).ToArray();
        }

        public override HqlTreeNode BuildHql(MemberInfo member, Expression expression, HqlTreeBuilder treeBuilder,
            IHqlExpressionVisitor visitor)
        {
            var methodCall = treeBuilder.MethodCall(SpatialDialect.HqlPrefix + member.Name, new[]
            {
                visitor.Visit(expression).AsExpression()
            });
            if (typeof(TResult) == typeof(bool))
            {
                return treeBuilder.Equality(methodCall, treeBuilder.True());
            }
            return methodCall;
        }
    }
}
