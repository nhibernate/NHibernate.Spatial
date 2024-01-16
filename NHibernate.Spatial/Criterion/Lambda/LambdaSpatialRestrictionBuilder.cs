using NetTopologySuite.Geometries;
using NHibernate.Criterion;

namespace NHibernate.Spatial.Criterion.Lambda
{
    public class LambdaSpatialRestrictionBuilder
    {
        private readonly string propertyName;
        private bool isNot;

        /// <summary>
        /// Constructed with property name
        /// </summary>
        public LambdaSpatialRestrictionBuilder(string propertyName)
        {
            this.propertyName = propertyName;
        }

        public LambdaSpatialRestrictionBuilder Not
        {
            get
            {
                isNot = !isNot;
                return this;
            }
        }

        private AbstractCriterion Process(AbstractCriterion criterion)
        {
            if (isNot)
            {
                return Restrictions.Not(criterion);
            }
            return criterion;
        }

        #region Filter

        /// <summary>
        /// Apply a "filter" constraint to the named property
        /// </summary>
        public AbstractCriterion Filter(Geometry value)
        {
            return Process(SpatialRestrictions.Filter(propertyName, value));
        }

        /// <summary>
        /// Apply a "filter" constraint to the named property
        /// </summary>
        public AbstractCriterion Filter(Envelope value, int srid)
        {
            return Process(SpatialRestrictions.Filter(propertyName, value, srid));
        }

        #endregion Filter

        #region Relations

        /// <summary>
        /// Apply a "contains" constraint to the named property
        /// </summary>
        public AbstractCriterion Contains(object value)
        {
            return Process(SpatialRestrictions.Contains(propertyName, value));
        }

        /// <summary>
        /// Apply a "covered by" constraint to the named property
        /// </summary>
        public AbstractCriterion CoveredBy(object value)
        {
            return Process(SpatialRestrictions.CoveredBy(propertyName, value));
        }

        /// <summary>
        /// Apply a "covers" constraint to the named property
        /// </summary>
        public AbstractCriterion Covers(object value)
        {
            return Process(SpatialRestrictions.Covers(propertyName, value));
        }

        /// <summary>
        /// Apply a "crosses" constraint to the named property
        /// </summary>
        public AbstractCriterion Crosses(object value)
        {
            return Process(SpatialRestrictions.Crosses(propertyName, value));
        }

        /// <summary>
        /// Apply a "disjoint" constraint to the named property
        /// </summary>
        public AbstractCriterion Disjoint(object value)
        {
            return Process(SpatialRestrictions.Disjoint(propertyName, value));
        }

        /// <summary>
        /// Apply a "eq" constraint to the named property
        /// </summary>
        public AbstractCriterion Eq(object value)
        {
            return Process(SpatialRestrictions.Eq(propertyName, value));
        }

        /// <summary>
        /// Apply a "eq exact" constraint to the named property
        /// </summary>
        public AbstractCriterion EqExact(object value, double tolerance)
        {
            return Process(SpatialRestrictions.EqExact(propertyName, value, tolerance));
        }

        /// <summary>
        /// Apply a "intersects" constraint to the named property
        /// </summary>
        public AbstractCriterion Intersects(object value)
        {
            return Process(SpatialRestrictions.Intersects(propertyName, value));
        }

        /// <summary>
        /// Apply a "overlaps" constraint to the named property
        /// </summary>
        public AbstractCriterion Overlaps(object value)
        {
            return Process(SpatialRestrictions.Overlaps(propertyName, value));
        }

        /// <summary>
        /// Apply a "relate" constraint to the named property
        /// </summary>
        public AbstractCriterion Relate(object value, string intersectionPatternMatrix)
        {
            return Process(SpatialRestrictions.Relate(propertyName, value, intersectionPatternMatrix));
        }

        /// <summary>
        /// Apply a "touches" constraint to the named property
        /// </summary>
        public AbstractCriterion Touches(object value)
        {
            return Process(SpatialRestrictions.Touches(propertyName, value));
        }

        /// <summary>
        /// Apply a "within" constraint to the named property
        /// </summary>
        public AbstractCriterion Within(object value)
        {
            return Process(SpatialRestrictions.Within(propertyName, value));
        }

        /// <summary>
        /// Apply a "is Within distance" constraint to the named property
        /// </summary>
        public AbstractCriterion IsWithinDistance(object value, double distance)
        {
            return Process(SpatialRestrictions.IsWithinDistance(propertyName, value, distance));
        }

        #endregion Relations

        #region Validations

        /// <summary>
        /// Apply an "is closed" constraint to the named property
        /// </summary>
        public AbstractCriterion IsClosed => Process(SpatialRestrictions.IsClosed(propertyName));

        /// <summary>
        /// Apply an "is empty" constraint to the named property
        /// </summary>
        public AbstractCriterion IsEmpty => Process(SpatialRestrictions.IsEmpty(propertyName));

        /// <summary>
        /// Apply an "is ring" constraint to the named property
        /// </summary>
        public AbstractCriterion IsRing => Process(SpatialRestrictions.IsRing(propertyName));

        /// <summary>
        /// Apply an "is simple" constraint to the named property
        /// </summary>
        public AbstractCriterion IsSimple => Process(SpatialRestrictions.IsSimple(propertyName));

        /// <summary>
        /// Apply an "is valid" constraint to the named property
        /// </summary>
        public AbstractCriterion IsValid => Process(SpatialRestrictions.IsValid(propertyName));

        #endregion Validations
    }
}
