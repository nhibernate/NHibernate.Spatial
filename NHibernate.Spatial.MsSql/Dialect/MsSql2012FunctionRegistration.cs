
using NHibernate.SqlCommand;
using NHibernate.Type;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHibernate.Spatial.Dialect
{
	class MsSql2012FunctionRegistration : MsSql2008FunctionRegistration
	{
		public MsSql2012FunctionRegistration(IRegisterationAdaptor adaptor, string sqlTypeName, string geometryColumnsViewName,IType geometryType)
			: base(adaptor, sqlTypeName, geometryColumnsViewName,geometryType)
		{

		}


		/// <summary>
		/// Gets the spatial aggregate string.
		/// </summary>
		/// <param name="geometry">The geometry.</param>
		/// <param name="aggregate">The aggregate.</param>
		/// <returns></returns>
		public override SqlString GetSpatialAggregateString(object geometry, SpatialAggregate aggregate)
		{
			string aggregateFunction;
			switch (aggregate)
			{
				case SpatialAggregate.Collect:
					aggregateFunction = "CollectionAggregate";
					break;

				case SpatialAggregate.Envelope:
					aggregateFunction = "EnvelopeAggregate";
					break;

				case SpatialAggregate.ConvexHull:
					aggregateFunction = "ConvexHullAggregate";
					break;

				case SpatialAggregate.Union:
					aggregateFunction = "UnionAggregate";
					break;

				default:
					throw new ArgumentException("Invalid spatial aggregate argument");
			}

			aggregateFunction = sqlTypeName + "::" + aggregateFunction;
			return new SqlStringBuilder()
				.Add(aggregateFunction)
				.Add("(")
				.AddObject(geometry)
				.Add(")")
				.ToSqlString();
		}
	}
}
