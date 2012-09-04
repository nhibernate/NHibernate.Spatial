using System;

namespace NHibernate.Spatial.Linq
{
	public class SpatialLinqMethodException : Exception
	{
		public SpatialLinqMethodException()
			: base("Method to use only in Linq expressions")
		{
		}
	}
}