using NetTopologySuite.Geometries;
using NHibernate.Criterion;

namespace NHibernate.Spatial.Criterion.Lambda
{
    public class LambdaSpatialRestrictionBuilder
    {
        private bool isNot;
        private string propertyName;

        /// <summary>
        /// Constructed with property name
        /// </summary>
        public LambdaSpatialRestrictionBuilder(string propertyName)
        {
            this.propertyName = propertyName;
        }

        private AbstractCriterion Process(AbstractCriterion criterion)
        {
            if (this.isNot)
            {
                return Restrictions.Not(criterion);
            }
            return criterion;
        }

        public LambdaSpatialRestrictionBuilder Not
        {
            get
            {
                this.isNot = !this.isNot;
                return this;
            }
        }

        #region Filter

        /// <summary>
        /// Apply a "filter" constraint to the named property
        /// </summary>
        public AbstractCriterion Filter(Geometry value)
        {
            return this.Process(SpatialRestrictions.Filter(this.propertyName, value));
        }

        /// <summary>
        /// Apply a "filter" constraint to the named property
        /// </summary>
        public AbstractCriterion Filter(Envelope value, int srid)
        {
            return this.Process(SpatialRestrictions.Filter(this.propertyName, value, srid));
        }

        #endregion Filter

        #region Relations

        /// <summary>
        /// Apply a "contains" constraint to the named property
        /// </summary>
        public AbstractCriterion Contains(object value)
        {
            return this.Process(SpatialRestrictions.Contains(this.propertyName, value));
        }

        /// <summary>
        /// Apply a "covered by" constraint to the named property
        /// </summary>
        public AbstractCriterion CoveredBy(object value)
        {
            return this.Process(SpatialRestrictions.CoveredBy(this.propertyName, value));
        }

        /// <summary>
        /// Apply a "covers" constraint to the named property
        /// </summary>
        public AbstractCriterion Covers(object value)
        {
            return this.Process(SpatialRestrictions.Covers(this.propertyName, value));
        }

        /// <summary>
        /// Apply a "crosses" constraint to the named property
        /// </summary>
        public AbstractCriterion Crosses(object value)
        {
            return this.Process(SpatialRestrictions.Crosses(this.propertyName, value));
        }

        /// <summary>
        /// Apply a "disjoint" constraint to the named property
        /// </summary>
        public AbstractCriterion Disjoint(object value)
        {
            return this.Process(SpatialRestrictions.Disjoint(this.propertyName, value));
        }

        /// <summary>
        /// Apply a "eq" constraint to the named property
        /// </summary>
        public AbstractCriterion Eq(object value)
        {
            return this.Process(SpatialRestrictions.Eq(this.propertyName, value));
        }

        /// <summary>
        /// Apply a "eq exact" constraint to the named property
        /// </summary>
        public AbstractCriterion EqExact(object value, double tolerance)
        {
            return this.Process(SpatialRestrictions.EqExact(this.propertyName, value, tolerance));
        }

        /// <summary>
        /// Apply a "intersects" constraint to the named property
        /// </summary>
        public AbstractCriterion Intersects(object value)
        {
            return this.Process(SpatialRestrictions.Intersects(this.propertyName, value));
        }

        /// <summary>
        /// Apply a "overlaps" constraint to the named property
        /// </summary>
        public AbstractCriterion Overlaps(object value)
        {
            return this.Process(SpatialRestrictions.Overlaps(this.propertyName, value));
        }

        /// <summary>
        /// Apply a "relate" constraint to the named property
        /// </summary>
        public AbstractCriterion Relate(object value, string intersectionPatternMatrix)
        {
            return this.Process(SpatialRestrictions.Relate(this.propertyName, value, intersectionPatternMatrix));
        }

        /// <summary>
        /// Apply a "touches" constraint to the named property
        /// </summary>
        public AbstractCriterion Touches(object value)
        {
            return this.Process(SpatialRestrictions.Touches(this.propertyName, value));
        }

        /// <summary>
        /// Apply a "within" constraint to the named property
        /// </summary>
        public AbstractCriterion Within(object value)
        {
            return this.Process(SpatialRestrictions.Within(this.propertyName, value));
        }

        /// <summary>
        /// Apply a "is Within distance" constraint to the named property
        /// </summary>
        public AbstractCriterion IsWithinDistance(object value, double distance)
        {
            return this.Process(SpatialRestrictions.IsWithinDistance(this.propertyName, value, distance));
        }

        #endregion Relations

        #region Validations

        /// <summary>
        /// Apply an "is closed" constraint to the named property
        /// </summary>
        public AbstractCriterion IsClosed
        {
            get { return this.Process(SpatialRestrictions.IsClosed(this.propertyName)); }
        }

        /// <summary>
        /// Apply an "is empty" constraint to the named property
        /// </summary>
        public AbstractCriterion IsEmpty
        {
            get { return this.Process(SpatialRestrictions.IsEmpty(this.propertyName)); }
        }

        /// <summary>
        /// Apply an "is ring" constraint to the named property
        /// </summary>
        public AbstractCriterion IsRing
        {
            get { return this.Process(SpatialRestrictions.IsRing(this.propertyName)); }
        }

        /// <summary>
        /// Apply an "is simple" constraint to the named property
        /// </summary>
        public AbstractCriterion IsSimple
        {
            get { return this.Process(SpatialRestrictions.IsSimple(this.propertyName)); }
        }

        /// <summary>
        /// Apply an "is valid" constraint to the named property
        /// </summary>
        public AbstractCriterion IsValid
        {
            get { return this.Process(SpatialRestrictions.IsValid(this.propertyName)); }
        }

        #endregion Validations
    }
}