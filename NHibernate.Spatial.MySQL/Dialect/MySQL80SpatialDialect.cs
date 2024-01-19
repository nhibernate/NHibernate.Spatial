using NHibernate.Spatial.Metadata;
using NHibernate.SqlCommand;
using System;
using System.Data;
using System.Text;

namespace NHibernate.Spatial.Dialect
{
    public class MySQL80SpatialDialect : MySQL57SpatialDialect
    {
        public MySQL80SpatialDialect()
        {
            // See: https://github.com/nhibernate/nhibernate-core/blob/master/src/NHibernate/Dialect/MySQL8Dialect.cs#L7C48-L7C73
            RegisterColumnType(DbType.Boolean, "BOOLEAN");
        }

        /// <summary>
        /// Gets the spatial transform string.
        /// </summary>
        /// <param name="geometry">The geometry.</param>
        /// <param name="srid">The srid.</param>
        /// <returns></returns>
        public override SqlString GetSpatialTransformString(object geometry, int srid)
        {
            return new SqlStringBuilder()
                .Add(DialectPrefix)
                .Add("Transform(")
                .AddObject(geometry)
                .Add(",")
                .Add(srid.ToString())
                .Add(")")
                .ToSqlString();
        }

        /// <summary>
        /// Gets the spatial validation string.
        /// </summary>
        /// <param name="geometry">The geometry.</param>
        /// <param name="validation">The validation.</param>
        /// <param name="criterion">if set to <c>true</c> [criterion].</param>
        /// <returns></returns>
        public override SqlString GetSpatialValidationString(object geometry, SpatialValidation validation, bool criterion)
        {
            return new SqlStringBuilder()
                .Add(DialectPrefix)
                .Add(validation.ToString())
                .Add("(")
                .AddObject(geometry)
                .Add(")")
                .ToSqlString();
        }

        /// <summary>
        /// Gets the spatial aggregate string.
        /// </summary>
        /// <param name="geometry">The geometry.</param>
        /// <param name="aggregate">The aggregate.</param>
        /// <returns></returns>
        public override SqlString GetSpatialAggregateString(object geometry, SpatialAggregate aggregate)
        {
            switch (aggregate)
            {
                case SpatialAggregate.Collect:
                    return new SqlStringBuilder()
                        .Add(DialectPrefix)
                        .Add(aggregate.ToString())
                        .Add("(")
                        .AddObject(geometry)
                        .Add(")")
                        .ToSqlString();

                case SpatialAggregate.ConvexHull:
                case SpatialAggregate.Envelope:
                    // MySQL only directly supports the ST_Collect spatial aggregate function, therefore
                    // we mimic these spatial agg functions by grouping the geometries from each row into
                    // a geometry collection and then performing the function on the geometry collection
                    // See: https://forums.mysql.com/read.php?23,249284,249284#msg-249284
                    var collectAggregate = GetSpatialAggregateString(geometry, SpatialAggregate.Collect);
                    return new SqlStringBuilder()
                        .Add(DialectPrefix)
                        .Add(aggregate.ToString())
                        .Add("(")
                        .Add(collectAggregate)
                        .Add(")")
                        .ToSqlString();

                case SpatialAggregate.Intersection:
                case SpatialAggregate.Union:
                    throw new NotSupportedException($"MySQL does not support {aggregate} spatial aggregate function");

                default:
                    throw new ArgumentException("Invalid spatial aggregate argument");
            }
        }

        /// <summary>
        /// Gets the spatial create string.
        /// </summary>
        /// <param name="schema">The schema.</param>
        /// <param name="table">The table.</param>
        /// <param name="column">The column.</param>
        /// <param name="srid">The srid.</param>
        /// <param name="subtype">The subtype.</param>
        /// <param name="dimension">The dimension.</param>
        /// <param name="isNullable">Whether or not the column is nullable</param>
        /// <returns></returns>
        public override string GetSpatialCreateString(string schema, string table, string column, int srid, string subtype, int dimension, bool isNullable)
        {
            var builder = new StringBuilder();

            string quotedSchema = QuoteSchema(schema);
            string quoteForTableName = QuoteForTableName(table);
            string quoteForColumnName = QuoteForColumnName(column);

            builder.AppendFormat("ALTER TABLE {0}{1} DROP COLUMN {2}"
                , quotedSchema
                , quoteForTableName
                , quoteForColumnName
            );

            builder.Append(MultipleQueriesSeparator);

            builder.AppendFormat("ALTER TABLE {0}{1} ADD {2} {3} {4} SRID {5}"
                , quotedSchema
                , quoteForTableName
                , quoteForColumnName
                , subtype
                , isNullable ? "NULL" : "NOT NULL"
                , srid
            );

            builder.Append(MultipleQueriesSeparator);

            return builder.ToString();
        }

        /// <summary>
        /// Gets a value indicating whether it supports spatial metadata.
        /// </summary>
        /// <value>
        /// <c>true</c> if it supports spatial metadata; otherwise, <c>false</c>.
        /// </value>
        public override bool SupportsSpatialMetadata(MetadataClass metadataClass)
        {
            return true;
        }
    }
}
