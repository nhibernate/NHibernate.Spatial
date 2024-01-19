// Copyright 2016 - Andreas Ravnestad (andreas.ravnestad@gmail.com)
//
// This file is part of NHibernate.Spatial.
// NHibernate.Spatial is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// NHibernate.Spatial is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.

// You should have received a copy of the GNU Lesser General Public License
// along with NHibernate.Spatial; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

using NHibernate.Dialect;
using NHibernate.Spatial.Dialect.Function;
using NHibernate.Spatial.Metadata;
using NHibernate.Spatial.Type;
using NHibernate.SqlCommand;
using NHibernate.Type;
using NHibernate.Util;
using System;
using System.Text;

namespace NHibernate.Spatial.Dialect
{
    /// <summary>
    ///
    /// </summary>
    public class MySQL57SpatialDialect : MySQL5Dialect, ISpatialDialect
    {
        protected const string DialectPrefix = SpatialDialect.IsoPrefix;
        private static readonly IType geometryType = new CustomType(typeof(MySQLGeometryType), null);

        /// <summary>
        /// Initializes a new instance of the <see cref="MySQLDialect"/> class.
        /// </summary>
        public MySQL57SpatialDialect()
        {
            SpatialDialect.LastInstantiated = this;
            RegisterBasicFunctions();
            RegisterFunctions();
        }

        // TODO: Use ISessionFactory.ConnectionProvider.Driver.MultipleQueriesSeparator
        public string MultipleQueriesSeparator => ";";

        public override string ToBooleanValueString(bool value)
        {
            return value ? "true" : "false";
        }

        #region Functions registration

        private void RegisterBasicFunctions()
        {
            // Relations
            RegisterSpatialFunction(SpatialRelation.Contains);
            RegisterSpatialFunction(SpatialRelation.CoveredBy);
            RegisterSpatialFunction(SpatialRelation.Covers);
            RegisterSpatialFunction(SpatialRelation.Crosses);
            RegisterSpatialFunction(SpatialRelation.Disjoint);
            RegisterSpatialFunction(SpatialRelation.Equals);
            RegisterSpatialFunction(SpatialRelation.Intersects);
            RegisterSpatialFunction(SpatialRelation.Overlaps);
            RegisterSpatialFunction(SpatialRelation.Touches);
            RegisterSpatialFunction(SpatialRelation.Within);

            // Analysis
            RegisterSpatialFunction(SpatialAnalysis.Buffer);
            RegisterSpatialFunction(SpatialAnalysis.ConvexHull);
            RegisterSpatialFunction(SpatialAnalysis.Difference);
            RegisterSpatialFunction(SpatialAnalysis.Distance);
            RegisterSpatialFunction(SpatialAnalysis.Intersection);
            RegisterSpatialFunction(SpatialAnalysis.SymDifference);
            RegisterSpatialFunction(SpatialAnalysis.Union);

            // Validations
            RegisterSpatialFunction(SpatialValidation.IsClosed);
            RegisterSpatialFunction(SpatialValidation.IsEmpty);
            RegisterSpatialFunction(SpatialValidation.IsRing);
            RegisterSpatialFunction(SpatialValidation.IsSimple);
            RegisterSpatialFunction(SpatialValidation.IsValid);
        }

        protected override void RegisterFunctions()
        {
            RegisterSpatialFunction("Boundary");
            RegisterSpatialFunction("Centroid");
            RegisterSpatialFunction("EndPoint");
            RegisterSpatialFunction("Envelope");
            RegisterSpatialFunction("ExteriorRing");
            RegisterSpatialFunction("GeometryN", 2);
            RegisterSpatialFunction("InteriorRingN", 2);
            RegisterSpatialFunction("PointN", 2);
            RegisterSpatialFunction("PointOnSurface");
            RegisterSpatialFunction("Simplify", 2);
            RegisterSpatialFunction("StartPoint");
            RegisterSpatialFunction("Transform", 2);

            RegisterSpatialFunction("GeomCollFromText", 2);
            RegisterSpatialFunction("GeomCollFromWKB", 2);
            RegisterSpatialFunction("GeomFromText", 2);
            RegisterSpatialFunction("GeomFromWKB", 2);
            RegisterSpatialFunction("LineFromText", 2);
            RegisterSpatialFunction("LineFromWKB", 2);
            RegisterSpatialFunction("PointFromText", 2);
            RegisterSpatialFunction("PointFromWKB", 2);
            RegisterSpatialFunction("PolyFromText", 2);
            RegisterSpatialFunction("PolyFromWKB", 2);
            RegisterSpatialFunction("MLineFromText", 2);
            RegisterSpatialFunction("MLineFromWKB", 2);
            RegisterSpatialFunction("MPointFromText", 2);
            RegisterSpatialFunction("MPointFromWKB", 2);
            RegisterSpatialFunction("MPolyFromText", 2);
            RegisterSpatialFunction("MPolyFromWKB", 2);

            RegisterSpatialFunction("AsBinary", NHibernateUtil.Binary);

            RegisterSpatialFunction("AsText", NHibernateUtil.String);
            RegisterSpatialFunction("AsGML", NHibernateUtil.String);
            RegisterSpatialFunction("GeometryType", NHibernateUtil.String);

            RegisterSpatialFunction("Area", NHibernateUtil.Double);
            RegisterSpatialFunction("Length", NHibernateUtil.Double);
            RegisterSpatialFunction("X", NHibernateUtil.Double);
            RegisterSpatialFunction("Y", NHibernateUtil.Double);

            RegisterSpatialFunction("SRID", NHibernateUtil.Int32);
            RegisterSpatialFunction("Dimension", NHibernateUtil.Int32);
            RegisterSpatialFunction("NumGeometries", NHibernateUtil.Int32);
            RegisterSpatialFunction("NumInteriorRings", NHibernateUtil.Int32);
            RegisterSpatialFunction("NumPoints", NHibernateUtil.Int32);

            RegisterSpatialFunction("Relate", NHibernateUtil.Boolean, 3);
        }

        protected void RegisterSpatialFunction(string standardName, string dialectName, IType returnedType, int allowedArgsCount)
        {
            RegisterFunction(SpatialDialect.HqlPrefix + standardName, new SpatialStandardSafeFunction(dialectName, returnedType, allowedArgsCount));
        }

        protected void RegisterSpatialFunction(string standardName, string dialectName, IType returnedType)
        {
            RegisterSpatialFunction(standardName, dialectName, returnedType, 1);
        }

        protected void RegisterSpatialFunction(string name, IType returnedType, int allowedArgsCount)
        {
            RegisterSpatialFunction(name, SpatialDialect.IsoPrefix + name, returnedType, allowedArgsCount);
        }

        protected void RegisterSpatialFunction(string name, IType returnedType)
        {
            RegisterSpatialFunction(name, SpatialDialect.IsoPrefix + name, returnedType);
        }

        protected void RegisterSpatialFunction(string name, int allowedArgsCount)
        {
            RegisterSpatialFunction(name, GeometryType, allowedArgsCount);
        }

        protected void RegisterSpatialFunction(string name)
        {
            RegisterSpatialFunction(name, GeometryType);
        }

        protected void RegisterSpatialFunction(SpatialRelation relation)
        {
            RegisterFunction(SpatialDialect.HqlPrefix + relation, new SpatialRelationFunction(this, relation));
        }

        protected void RegisterSpatialFunction(SpatialValidation validation)
        {
            RegisterFunction(SpatialDialect.HqlPrefix + validation, new SpatialValidationFunction(this, validation));
        }

        protected void RegisterSpatialFunction(SpatialAnalysis analysis)
        {
            RegisterFunction(SpatialDialect.HqlPrefix + analysis, new SpatialAnalysisFunction(this, analysis));
        }

        #endregion Functions registration

        #region ISpatialDialect Members

        /// <summary>
        /// Creates the geometry user type.
        /// </summary>
        /// <returns></returns>
        public virtual IGeometryUserType CreateGeometryUserType()
        {
            return new MySQLGeometryType();
        }

        /// <summary>
        /// Gets the type of the geometry.
        /// </summary>
        /// <value>The type of the geometry.</value>
        public virtual IType GeometryType => geometryType;

        /// <summary>
        /// Gets the spatial transform string.
        /// </summary>
        /// <param name="geometry">The geometry.</param>
        /// <param name="srid">The srid.</param>
        /// <returns></returns>
        public virtual SqlString GetSpatialTransformString(object geometry, int srid)
        {
            throw new NotSupportedException("MySQL 5.7 does not support spatial transform");
        }

        /// <summary>
        /// Gets the spatial validation string.
        /// </summary>
        /// <param name="geometry">The geometry.</param>
        /// <param name="validation">The validation.</param>
        /// <param name="criterion">if set to <c>true</c> [criterion].</param>
        /// <returns></returns>
        public virtual SqlString GetSpatialValidationString(object geometry, SpatialValidation validation, bool criterion)
        {
            // In MySQL 5.7, the ST_Valid function requires geometries to have an SRID of 0
            // See: https://dev.mysql.com/doc/refman/5.7/en/spatial-convenience-functions.html#function_st-isvalid
            if (validation == SpatialValidation.IsValid)
            {
                return new SqlStringBuilder()
                    .Add(DialectPrefix)
                    .Add(validation.ToString())
                    .Add("(")
                    .Add("ST_GeomFromWKB(ST_AsWKB(")
                    .AddObject(geometry)
                    .Add("), 0)")
                    .Add(")")
                    .ToSqlString();
            }

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
        public virtual SqlString GetSpatialAggregateString(object geometry, SpatialAggregate aggregate)
        {
            string aggregateFunction;
            switch (aggregate)
            {
                // MySQL doesn't support spatial aggregate functions directly, therefore
                // we replicate it by grouping the geometry from each row into a geometry
                // collection and then performing the function on the geometry collection
                // See: https://forums.mysql.com/read.php?23,249284,249284#msg-249284
                case SpatialAggregate.Collect:
                    return new SqlStringBuilder()
                        .Add(DialectPrefix)
                        .Add("GeometryCollectionFromText(CONCAT(\"GEOMETRYCOLLECTION(\", GROUP_CONCAT(")
                        .Add(DialectPrefix)
                        .Add("AsText(")
                        .AddObject(geometry)
                        .Add(")), \")\"))")
                        .ToSqlString();

                case SpatialAggregate.ConvexHull:
                case SpatialAggregate.Envelope:
                    aggregateFunction = aggregate.ToString();
                    break;

                case SpatialAggregate.Intersection:
                case SpatialAggregate.Union:
                    throw new NotSupportedException($"MySQL does not support {aggregate} spatial aggregate function");

                default:
                    throw new ArgumentException("Invalid spatial aggregate argument");
            }

            var collectAggregate = GetSpatialAggregateString(geometry, SpatialAggregate.Collect);
            return new SqlStringBuilder()
                .Add(DialectPrefix)
                .Add(aggregateFunction)
                .Add("(")
                .Add(collectAggregate)
                .Add(")")
                .ToSqlString();
        }

        public SqlString GetSpatialRelationString(object geometry, SpatialRelation relation, object anotherGeometry, bool criterion)
        {
            switch (relation)
            {
                case SpatialRelation.Covers:
                    string[] patterns =
                    {
                        "T*****FF*",
                        "*T****FF*",
                        "***T**FF*",
                        "****T*FF*",
                    };
                    var builder = new SqlStringBuilder();
                    builder.Add("(");
                    for (int i = 0; i < patterns.Length; i++)
                    {
                        if (i > 0)
                        {
                            builder.Add(" OR ");
                        }
                        builder
                            .Add("Relate")
                            .Add("(")
                            .AddObject(geometry)
                            .Add(", ")
                            .AddObject(anotherGeometry)
                            .Add(", '")
                            .Add(patterns[i])
                            .Add("')")
                            .ToSqlString();
                    }
                    builder.Add(")");
                    return builder.ToSqlString();

                case SpatialRelation.CoveredBy:
                    return GetSpatialRelationString(anotherGeometry, SpatialRelation.Covers, geometry, criterion);

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

        public SqlString GetSpatialRelateString(object geometry, object anotherGeometry, object pattern, bool isStringPattern, bool criterion)
        {
            var builder = new SqlStringBuilder();
            builder
                .Add("Relate(")
                .AddObject(geometry)
                .Add(", ")
                .AddObject(anotherGeometry);
            if (pattern != null)
            {
                builder.Add(", ");
                if (isStringPattern)
                {
                    builder
                        .Add("'")
                        .Add((string) pattern)
                        .Add("'");
                }
                else
                {
                    builder.AddObject(pattern);
                }
            }
            return builder
                .Add(")")
                .Add(criterion ? " = 1" : "")
                .ToSqlString();
        }

        public SqlString GetSpatialFilterString(string tableAlias, string geometryColumnName, string primaryKeyColumnName, string tableName, Parameter parameter)
        {
            return new SqlStringBuilder(7)
                .Add("MBRIntersects(")
                .Add(tableAlias)
                .Add(".")
                .Add(geometryColumnName)
                .Add(",")
                .Add(parameter)
                .Add(")")
                .ToSqlString();
        }

        /// <summary>
        /// Gets the spatial analysis string.
        /// </summary>
        /// <param name="geometry">The geometry.</param>
        /// <param name="analysis">The analysis.</param>
        /// <param name="extraArgument">The extra argument.</param>
        /// <returns></returns>
        public virtual SqlString GetSpatialAnalysisString(object geometry, SpatialAnalysis analysis, object extraArgument)
        {
            switch (analysis)
            {
                case SpatialAnalysis.Buffer:
                    if (!(extraArgument is Parameter || new SqlString(Parameter.Placeholder).Equals(extraArgument)))
                    {
                        extraArgument = Convert.ToString(extraArgument, System.Globalization.NumberFormatInfo.InvariantInfo);
                    }
                    return new SqlStringBuilder(6)
                        .Add(DialectPrefix)
                        .Add("Buffer(")
                        .AddObject(geometry)
                        .Add(", ")
                        .AddObject(extraArgument)
                        .Add(")")
                        .ToSqlString();

                case SpatialAnalysis.ConvexHull:
                    return new SqlStringBuilder()
                        .Add(DialectPrefix)
                        .Add("ConvexHull(")
                        .AddObject(geometry)
                        .Add(")")
                        .ToSqlString();

                case SpatialAnalysis.Difference:
                    return new SqlStringBuilder()
                        .Add(DialectPrefix)
                        .Add("Difference(")
                        .AddObject(geometry)
                        .Add(",")
                        .AddObject(extraArgument)
                        .Add(")")
                        .ToSqlString();
                case SpatialAnalysis.Intersection:
                    return new SqlStringBuilder()
                        .Add(DialectPrefix)
                        .Add("Intersection(")
                        .AddObject(geometry)
                        .Add(",")
                        .AddObject(extraArgument)
                        .Add(")")
                        .ToSqlString();
                case SpatialAnalysis.SymDifference:
                    return new SqlStringBuilder()
                        .Add(DialectPrefix)
                        .Add("SymDifference(")
                        .AddObject(geometry)
                        .Add(",")
                        .AddObject(extraArgument)
                        .Add(")")
                        .ToSqlString();
                case SpatialAnalysis.Union:
                    return new SqlStringBuilder()
                        .Add(DialectPrefix)
                        .Add(analysis.ToString())
                        .Add("(")
                        .AddObject(geometry)
                        .Add(",")
                        .AddObject(extraArgument)
                        .Add(")")
                        .ToSqlString();
                case SpatialAnalysis.Distance:
                    return new SqlStringBuilder()
                        .Add(DialectPrefix)
                        .Add("Distance(")
                        .AddObject(geometry)
                        .Add(",")
                        .AddObject(extraArgument)
                        .Add(")")
                        .ToSqlString();
                default:
                    throw new ArgumentException("Invalid spatial analysis argument");
            }
        }

        /// <summary>
        /// Gets the spatial create string.
        /// </summary>
        /// <param name="schema">The schema.</param>
        /// <returns></returns>
        public string GetSpatialCreateString(string schema)
        {
            return null;
        }

        /// <summary>
        /// Quotes the schema.
        /// </summary>
        /// <param name="schema">The schema.</param>
        /// <returns></returns>
        protected string QuoteSchema(string schema)
        {
            if (string.IsNullOrEmpty(schema))
            {
                return null;
            }
            return QuoteForSchemaName(schema) + StringHelper.Dot;
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
        public virtual string GetSpatialCreateString(string schema, string table, string column, int srid, string subtype, int dimension, bool isNullable)
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

            builder.AppendFormat("ALTER TABLE {0}{1} ADD {2} {3} {4}"
                , quotedSchema
                , quoteForTableName
                , quoteForColumnName
                , subtype
                , isNullable ? "NULL" : "NOT NULL"
            );

            builder.Append(MultipleQueriesSeparator);

            return builder.ToString();
        }

        /// <summary>
        /// Gets the spatial drop string.
        /// </summary>
        /// <param name="schema">The schema.</param>
        /// <returns></returns>
        public string GetSpatialDropString(string schema)
        {
            return null;
        }

        /// <summary>
        /// Gets the spatial drop string.
        /// </summary>
        /// <param name="schema">The schema.</param>
        /// <param name="table">The table.</param>
        /// <param name="column">The column.</param>
        /// <returns></returns>
        public string GetSpatialDropString(string schema, string table, string column)
        {
            return null;
        }

        /// <summary>
        /// Gets a value indicating whether it supports spatial metadata.
        /// </summary>
        /// <value>
        /// <c>true</c> if it supports spatial metadata; otherwise, <c>false</c>.
        /// </value>
        public virtual bool SupportsSpatialMetadata(MetadataClass metadataClass)
        {
            return false;
        }

        #endregion ISpatialDialect Members
    }
}
