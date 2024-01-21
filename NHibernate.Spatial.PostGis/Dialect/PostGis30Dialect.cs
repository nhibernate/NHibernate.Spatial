using NHibernate.SqlCommand;
using System;

namespace NHibernate.Spatial.Dialect
{
    /// <summary>
    /// 
    /// </summary>
    public class PostGis30Dialect : PostGis20Dialect
    {
        public override SqlString GetSpatialRelationString(object geometry, SpatialRelation relation, object anotherGeometry, bool criterion)
        {
            switch (relation)
            {
                case SpatialRelation.Covers:
                case SpatialRelation.CoveredBy:
                case SpatialRelation.EqualsExact:
                    return base.GetSpatialRelationString(geometry, relation, anotherGeometry, criterion);

                default:
                    return new SqlStringBuilder(6)
                        .Add(SpatialDialect.IsoPrefix)
                        .Add(relation.ToString())
                        .Add("(")
                        .AddObject(geometry)
                        .Add(", ")
                        .AddObject(anotherGeometry)
                        .Add(")")
                        .ToSqlString();
            }
        }

        /// <summary>
        /// Gets the spatial analysis string.
        /// </summary>
        /// <param name="geometry">The geometry.</param>
        /// <param name="analysis">The analysis.</param>
        /// <param name="extraArgument">The extra argument.</param>
        /// <returns></returns>
        public override SqlString GetSpatialAnalysisString(object geometry, SpatialAnalysis analysis, object extraArgument)
        {
            switch (analysis)
            {
                case SpatialAnalysis.Buffer:
                case SpatialAnalysis.ConvexHull:
                    return base.GetSpatialAnalysisString(geometry, analysis, extraArgument);

                case SpatialAnalysis.Difference:
                case SpatialAnalysis.Distance:
                case SpatialAnalysis.Intersection:
                case SpatialAnalysis.SymDifference:
                case SpatialAnalysis.Union:
                    return new SqlStringBuilder()
                        .Add(SpatialDialect.IsoPrefix)
                        .Add(analysis.ToString())
                        .Add("(")
                        .AddObject(geometry)
                        .Add(",")
                        .AddObject(extraArgument)
                        .Add(")")
                        .ToSqlString();

                default:
                    throw new ArgumentException("Invalid spatial analysis argument");
            }
        }
    }
}
