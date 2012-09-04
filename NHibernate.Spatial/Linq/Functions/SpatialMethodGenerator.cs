using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate.Hql.Ast;
using NHibernate.Linq;
using NHibernate.Linq.Functions;
using NHibernate.Linq.Visitors;
using NHibernate.Spatial.Dialect;

namespace NHibernate.Spatial.Linq.Functions
{
	public abstract class SpatialMethodGenerator<TSource, TResult> : BaseHqlGeneratorForMethod
	{
		private readonly string methodName;

		protected SpatialMethodGenerator(string methodName, params Expression<Action<TSource>>[] expressions)
		{
			this.methodName = methodName;
			SupportedMethods = expressions.Select(o => ReflectionHelper.GetMethodDefinition(o)).ToArray();
		}

		protected SpatialMethodGenerator(params Expression<Action<TSource>>[] expressions)
			: this(null, expressions)
		{
		}

		public override HqlTreeNode BuildHql(MethodInfo method, Expression targetObject, ReadOnlyCollection<Expression> arguments, HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
		{
			var isExtensionMethod = (targetObject == null);
			var expressions = isExtensionMethod ? arguments : new[] { targetObject }.Concat(arguments);
			var parameters = expressions.Select(o => visitor.Visit(o).AsExpression());
			var methodCall = treeBuilder.MethodCall(SpatialDialect.HqlPrefix + (methodName ?? method.Name), parameters);
			if (typeof(TResult) == typeof(bool))
			{
				return treeBuilder.Equality(methodCall, treeBuilder.True());
			}
			return methodCall;
		}
	}
}