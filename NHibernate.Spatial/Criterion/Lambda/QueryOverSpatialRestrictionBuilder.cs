namespace NHibernate.Spatial.Criterion.Lambda
{
	public class QueryOverSpatialRestrictionBuilder<TRoot, TSubType> : QueryOverSpatialRestrictionBuilderBase<IQueryOver<TRoot, TSubType>, TRoot, TSubType>
	{
		public QueryOverSpatialRestrictionBuilder(IQueryOver<TRoot, TSubType> root, string propertyName)
			: base(root, propertyName)
		{
		}

		public QueryOverSpatialRestrictionBuilder<TRoot, TSubType> Not
		{
			get
			{
				isNot = !isNot;
				return this;
			}
		}


	}
}