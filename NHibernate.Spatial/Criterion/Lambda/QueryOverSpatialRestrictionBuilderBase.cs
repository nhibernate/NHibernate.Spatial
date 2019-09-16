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
            if (this.isNot)
            {
                criterion = Restrictions.Not(criterion);
            }
            return (TReturn)this.root.And(criterion);
        }

        #region Filter

        /// <summary>
        /// Apply a "filter" constraint to the named property
        /// </summary>
        public TReturn Filter(Geometry value)
        {
            return this.Add(SpatialRestrictions.Filter(this.propertyName, value));
        }

        /// <summary>
        /// Apply a "filter" constraint to the named property
        /// </summary>
        public TReturn Filter(Envelope value, int srid)
        {
            return this.Add(SpatialRestrictions.Filter(this.propertyName, value, srid));
        }

        #endregion Filter

        #region Relations

        /// <summary>
        /// Apply a "contains" constraint to the named property
        /// </summary>
        public TReturn Contains(object value)
        {
            return this.Add(SpatialRestrictions.Contains(this.propertyName, value));
        }

        /// <summary>
        /// Apply a "covered by" constraint to the named property
        /// </summary>
        public TReturn CoveredBy(object value)
        {
            return this.Add(SpatialRestrictions.CoveredBy(this.propertyName, value));
        }

        /// <summary>
        /// Apply a "covers" constraint to the named property
        /// </summary>
        public TReturn Covers(object value)
        {
            return this.Add(SpatialRestrictions.Covers(this.propertyName, value));
        }

        /// <summary>
        /// Apply a "crosses" constraint to the named property
        /// </summary>
        public TReturn Crosses(object value)
        {
            return this.Add(SpatialRestrictions.Crosses(this.propertyName, value));
        }

        /// <summary>
        /// Apply a "disjoint" constraint to the named property
        /// </summary>
        public TReturn Disjoint(object value)
        {
            return this.Add(SpatialRestrictions.Disjoint(this.propertyName, value));
        }

        /// <summary>
        /// Apply a "eq" constraint to the named property
        /// </summary>
        public TReturn Eq(object value)
        {
            return this.Add(SpatialRestrictions.Eq(this.propertyName, value));
        }

        /// <summary>
        /// Apply a "eq exact" constraint to the named property
        /// </summary>
        public TReturn EqExact(object value, double tolerance)
        {
            return this.Add(SpatialRestrictions.EqExact(this.propertyName, value, tolerance));
        }

        /// <summary>
        /// Apply a "intersects" constraint to the named property
        /// </summary>
        public TReturn Intersects(object value)
        {
            return this.Add(SpatialRestrictions.Intersects(this.propertyName, value));
        }

        /// <summary>
        /// Apply a "overlaps" constraint to the named property
        /// </summary>
        public TReturn Overlaps(object value)
        {
            return this.Add(SpatialRestrictions.Overlaps(this.propertyName, value));
        }

        /// <summary>
        /// Apply a "relate" constraint to the named property
        /// </summary>
        public TReturn Relate(object value, string intersectionPatternMatrix)
        {
            return this.Add(SpatialRestrictions.Relate(this.propertyName, value, intersectionPatternMatrix));
        }

        /// <summary>
        /// Apply a "touches" constraint to the named property
        /// </summary>
        public TReturn Touches(object value)
        {
            return this.Add(SpatialRestrictions.Touches(this.propertyName, value));
        }

        /// <summary>
        /// Apply a "within" constraint to the named property
        /// </summary>
        public TReturn Within(object value)
        {
            return this.Add(SpatialRestrictions.Within(this.propertyName, value));
        }

        /// <summary>
        /// Apply a "is Within distance" constraint to the named property
        /// </summary>
        public TReturn IsWithinDistance(object value, double distance)
        {
            return this.Add(SpatialRestrictions.IsWithinDistance(this.propertyName, value, distance));
        }

        #endregion Relations

        #region Validations

        /// <summary>
        /// Apply an "is closed" constraint to the named property
        /// </summary>
        public TReturn IsClosed
        {
            get { return this.Add(SpatialRestrictions.IsClosed(this.propertyName)); }
        }

        /// <summary>
        /// Apply an "is empty" constraint to the named property
        /// </summary>
        public TReturn IsEmpty
        {
            get { return this.Add(SpatialRestrictions.IsEmpty(this.propertyName)); }
        }

        /// <summary>
        /// Apply an "is ring" constraint to the named property
        /// </summary>
        public TReturn IsRing
        {
            get { return this.Add(SpatialRestrictions.IsRing(this.propertyName)); }
        }

        /// <summary>
        /// Apply an "is simple" constraint to the named property
        /// </summary>
        public TReturn IsSimple
        {
            get { return this.Add(SpatialRestrictions.IsSimple(this.propertyName)); }
        }

        /// <summary>
        /// Apply an "is valid" constraint to the named property
        /// </summary>
        public TReturn IsValid
        {
            get { return this.Add(SpatialRestrictions.IsValid(this.propertyName)); }
        }

        #endregion Validations
    }
}