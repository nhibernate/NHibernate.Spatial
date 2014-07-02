
using NHibernate.SqlCommand;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHibernate.Spatial.Dialect
{
	class MsSql2012FunctionRegistration : MsSql2008FunctionRegistration
	{
		public MsSql2012FunctionRegistration(IRegisterationAdaptor adaptor, string sqlTypeName, string geometryColumnsViewName)
			: base(adaptor, sqlTypeName, geometryColumnsViewName)
		{

		}


		/// <summary>
		/// Gets the spatial aggregate string.
		/// </summary>
		/// <param name="geometry">The geometry.</param>
		/// <param name="aggregate">The aggregate.</param>
		/// <returns></returns>
		public SqlString GetSpatialAggregateString(object geometry, SpatialAggregate aggregate)
		{
			//TO DO Implement Spatial Aggregate for sql2012
			throw new NotImplementedException();
		}
	}
}
