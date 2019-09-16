using NetTopologySuite.Geometries;
using NHibernate.Hql.Ast;
using NHibernate.Linq.Functions;
using NHibernate.Linq.Visitors;
using NHibernate.Util;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Reflection;

namespace NHibernate.Spatial.Linq.Functions
{
    public class IsNullGenerator : BaseHqlGeneratorForMethod
    {
        public IsNullGenerator()
        {
            SupportedMethods = new[]
            {
                ReflectHelper.GetMethodDefinition<Geometry>(g => g.IsNull())
            };
        }

        public override HqlTreeNode BuildHql(MethodInfo method, Expression targetObject,
            ReadOnlyCollection<Expression> arguments, HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
        {
            var isNull = treeBuilder.IsNull(visitor.Visit(arguments[0]).AsExpression());
            return isNull;
        }
    }
}
