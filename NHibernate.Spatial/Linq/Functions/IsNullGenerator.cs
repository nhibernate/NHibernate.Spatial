using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Reflection;
using GeoAPI.Geometries;
using NHibernate.Hql.Ast;
using NHibernate.Linq;
using NHibernate.Linq.Functions;
using NHibernate.Linq.Visitors;

namespace NHibernate.Spatial.Linq.Functions
{
	public class IsNullGenerator : BaseHqlGeneratorForMethod
	{
		public IsNullGenerator()
		{
			SupportedMethods = new []
			{
				ReflectionHelper.GetMethodDefinition<IGeometry>(g => g.IsNull())
			};
		}

		public override HqlTreeNode BuildHql(MethodInfo method, Expression targetObject, ReadOnlyCollection<Expression> arguments, HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
		{
			var isNull = treeBuilder.IsNull(visitor.Visit(arguments[0]).AsExpression());
			return isNull;
		}
	}
}