using System;
using Oracle.DataAccess.Types;

namespace NHibernate.Spatial.Oracle
{
	public abstract class OracleArrayTypeFactoryBase<T> : IOracleArrayTypeFactory
	{
		public Array CreateArray(int numElems)
		{
			return new T[numElems];
		}

		public Array CreateStatusArray(int numElems)
		{
			return null;
		}
	}
}