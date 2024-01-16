using NetTopologySuite.Geometries;
using NHibernate.Criterion;

namespace NHibernate.Spatial.Criterion.Lambda
{
    public class QueryOverSpatialRestrictionBuilderBase<TReturn, TRoot, TSubType>
        where TReturn : IQueryOver<TRoot, TSubType>
    {
        protected bool isNot;
        private readonly string propertyName;
        private readonly TReturn root;

        /// <summary>
        /// Constructed with property name
        /// </summary>
        public QueryOverSpatialRestrictionBuilderBase(TReturn root, string propertyName)
        {
            this.root = root;
            this.propertyName = propertyName;
        }

        private TReturn Add(ICriterion criterion)
        {
            if (isNot)
            {
                criterion = Restrictions.Not(criterion);
            }
            return (TReturn) root.And(criterion);
        }

        #region Filter

        /// <summary>
        /// Apply a "filter" constraint to the named property
        /// </summary>
        public TReturn Filter(Geometry value)
        {
            return Add(SpatialRestrictions.Filter(propertyName, value));
        }

        /// <summary>
        /// Apply a "filter" constraint to the named property
        /// </summary>
        public TReturn Filter(Envelope value, int srid)
        {
            return Add(SpatialRestrictions.Filter(propertyName, value, srid));
        }

        #endregion Filter

        #region Relations

        /// <summary>
        /// Apply a "contains" constraint to the named property
        /// </summary>
        public TReturn Contains(object value)
        {
            return Add(SpatialRestrictions.Contains(propertyName, value));
        }

        /// <summary>
        /// Apply a "covered by" constraint to the named property
        /// </summary>
        public TReturn CoveredBy(object value)
        {
            return Add(SpatialRestrictions.CoveredBy(propertyName, value));
        }

        /// <summary>
        /// Apply a "covers" constraint to the named property
        /// </summary>
        public TReturn Covers(object value)
        {
            return Add(SpatialRestrictions.Covers(propertyName, value));
        }

        /// <summary>
        /// Apply a "crosses" constraint to the named property
        /// </summary>
        public TReturn Crosses(object value)
        {
            return Add(SpatialRestrictions.Crosses(propertyName, value));
        }

        /// <summary>
        /// Apply a "disjoint" constraint to the named property
        /// </summary>
        public TReturn Disjoint(object value)
        {
            return Add(SpatialRestrictions.Disjoint(propertyName, value));
        }

        /// <summary>
        /// Apply a "eq" constraint to the named property
        /// </summary>
        public TReturn Eq(object value)
        {
            return Add(SpatialRestrictions.Eq(propertyName, value));
        }

        /// <summary>
        /// Apply a "eq exact" constraint to the named property
        /// </summary>
        public TReturn EqExact(object value, double tolerance)
        {
            return Add(SpatialRestrictions.EqExact(propertyName, value, tolerance));
        }

        /// <summary>
        /// Apply a "intersects" constraint to the named property
        /// </summary>
        public TReturn Intersects(object value)
        {
            return Add(SpatialRestrictions.Intersects(propertyName, value));
        }

        /// <summary>
        /// Apply a "overlaps" constraint to the named property
        /// </summary>
        public TReturn Overlaps(object value)
        {
            return Add(SpatialRestrictions.Overlaps(propertyName, value));
        }

        /// <summary>
        /// Apply a "relate" constraint to the named property
        /// </summary>
        public TReturn Relate(object value, string intersectionPatternMatrix)
        {
            return Add(SpatialRestrictions.Relate(propertyName, value, intersectionPatternMatrix));
        }

        /// <summary>
        /// Apply a "touches" constraint to the named property
        /// </summary>
        public TReturn Touches(object value)
        {
            return Add(SpatialRestrictions.Touches(propertyName, value));
        }

        /// <summary>
        /// Apply a "within" constraint to the named property
        /// </summary>
        public TReturn Within(object value)
        {
            return Add(SpatialRestrictions.Within(propertyName, value));
        }

        /// <summary>
        /// Apply a "is Within distance" constraint to the named property
        /// </summary>
        public TReturn IsWithinDistance(object value, double distance)
        {
            return Add(SpatialRestrictions.IsWithinDistance(propertyName, value, distance));
        }

        #endregion Relations

        #region Validations

        /// <summary>
        /// Apply an "is closed" constraint to the named property
        /// </summary>
        public TReturn IsClosed => Add(SpatialRestrictions.IsClosed(propertyName));

        /// <summary>
        /// Apply an "is empty" constraint to the named property
        /// </summary>
        public TReturn IsEmpty => Add(SpatialRestrictions.IsEmpty(propertyName));

        /// <summary>
        /// Apply an "is ring" constraint to the named property
        /// </summary>
        public TReturn IsRing => Add(SpatialRestrictions.IsRing(propertyName));

        /// <summary>
        /// Apply an "is simple" constraint to the named property
        /// </summary>
        public TReturn IsSimple => Add(SpatialRestrictions.IsSimple(propertyName));

        /// <summary>
        /// Apply an "is valid" constraint to the named property
        /// </summary>
        public TReturn IsValid => Add(SpatialRestrictions.IsValid(propertyName));

        #endregion Validations
    }
}
