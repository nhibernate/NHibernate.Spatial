﻿// Copyright 2008 - Ricardo Stuven (rstuven@gmail.com)
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

using NHibernate.Spatial.Dialect.Function;
using NHibernate.Spatial.Metadata;
using NHibernate.Spatial.Type;
using NHibernate.SqlCommand;
using NHibernate.Type;
using NHibernate.Util;
using System;
using System.Globalization;
using System.Text;

namespace NHibernate.Spatial.Dialect
{
    internal class MsSql2012FunctionRegistration : ISpatialDialect
    {
        private const string DialectPrefix = "ST";
        private readonly string sqlTypeName;

        private readonly IRegisterationAdaptor adaptor;
        private readonly string geometryColumnsViewName;

        public MsSql2012FunctionRegistration(IRegisterationAdaptor adaptor, string sqlTypeName, string geometryColumnsViewName, IType geometryType)
        {
            this.adaptor = adaptor;
            this.sqlTypeName = sqlTypeName;
            this.geometryColumnsViewName = geometryColumnsViewName;
            GeometryType = geometryType;

            RegisterBasicFunctions();
            RegisterFunctions();
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

        private void RegisterFunctions()
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
            RegisterSpatialFunction("Simplify", "Reduce", 2);
            RegisterSpatialFunction("StartPoint");
            RegisterSpatialFunction("Transform", 2);

            RegisterSpatialFunctionStatic("GeomCollFromText", 2);
            RegisterSpatialFunctionStatic("GeomCollFromWKB", 2);
            RegisterSpatialFunctionStatic("GeomFromText", 2);
            RegisterSpatialFunctionStatic("GeomFromWKB", 2);
            RegisterSpatialFunctionStatic("LineFromText", 2);
            RegisterSpatialFunctionStatic("LineFromWKB", 2);
            RegisterSpatialFunctionStatic("PointFromText", 2);
            RegisterSpatialFunctionStatic("PointFromWKB", 2);
            RegisterSpatialFunctionStatic("PolyFromText", 2);
            RegisterSpatialFunctionStatic("PolyFromWKB", 2);
            RegisterSpatialFunctionStatic("MLineFromText", 2);
            RegisterSpatialFunctionStatic("MLineFromWKB", 2);
            RegisterSpatialFunctionStatic("MPointFromText", 2);
            RegisterSpatialFunctionStatic("MPointFromWKB", 2);
            RegisterSpatialFunctionStatic("MPolyFromText", 2);
            RegisterSpatialFunctionStatic("MPolyFromWKB", 2);

            RegisterSpatialFunction("AsBinary", NHibernateUtil.Binary);

            RegisterSpatialFunction("AsText", NHibernateUtil.String);
            RegisterSpatialFunction("AsGML", NHibernateUtil.String);
            RegisterSpatialFunction("GeometryType", NHibernateUtil.String);

            RegisterSpatialFunction("Area", NHibernateUtil.Double);
            RegisterSpatialFunction("Length", NHibernateUtil.Double);
            RegisterSpatialFunctionProperty("X", NHibernateUtil.Double);
            RegisterSpatialFunctionProperty("Y", NHibernateUtil.Double);

            RegisterSpatialFunctionProperty("SRID", "STSrid", NHibernateUtil.Int32);
            RegisterSpatialFunction("Dimension", NHibernateUtil.Int32);
            RegisterSpatialFunction("NumGeometries", NHibernateUtil.Int32);
            RegisterSpatialFunction("NumInteriorRings", "STNumInteriorRing", NHibernateUtil.Int32);
            RegisterSpatialFunction("NumPoints", NHibernateUtil.Int32);

            RegisterSpatialFunction("Relate", NHibernateUtil.Boolean, 3);
        }

        private void RegisterSpatialFunction(string standardName, string dialectName, IType returnedType, int allowedArgsCount)
        {
            adaptor.RegisterFunction(SpatialDialect.HqlPrefix + standardName, new SpatialMethodSafeFunction(dialectName, returnedType, allowedArgsCount));
        }

        private void RegisterSpatialFunction(string standardName, string dialectName, IType returnedType)
        {
            RegisterSpatialFunction(standardName, dialectName, returnedType, 1);
        }

        private void RegisterSpatialFunction(string name, IType returnedType, int allowedArgsCount)
        {
            RegisterSpatialFunction(name, DialectPrefix + name, returnedType, allowedArgsCount);
        }

        private void RegisterSpatialFunction(string name, IType returnedType)
        {
            RegisterSpatialFunction(name, DialectPrefix + name, returnedType);
        }

        private void RegisterSpatialFunction(string name, int allowedArgsCount)
        {
            RegisterSpatialFunction(name, GeometryType, allowedArgsCount);
        }

        private void RegisterSpatialFunctionStatic(string name, int allowedArgsCount)
        {
            string standardName = name;
            string dialectName = DialectPrefix + name;
            var returnedType = GeometryType;
            adaptor.RegisterFunction(
                SpatialDialect.HqlPrefix + standardName,
                new SpatialStandardSafeFunction(sqlTypeName + "::" + dialectName, returnedType, allowedArgsCount)
            );
        }

        private void RegisterSpatialFunctionProperty(string name, IType returnedType)
        {
            RegisterSpatialFunctionProperty(name, DialectPrefix + name, returnedType);
        }

        private void RegisterSpatialFunctionProperty(string standardName, string dialectName, IType returnedType)
        {
            adaptor.RegisterFunction(
                SpatialDialect.HqlPrefix + standardName,
                new SpatialPropertyFunction(dialectName, returnedType)
            );
        }

        private void RegisterSpatialFunction(string name)
        {
            RegisterSpatialFunction(name, GeometryType);
        }

        private void RegisterSpatialFunction(string standardName, string dialectName, int allowedArgsCount)
        {
            RegisterSpatialFunction(standardName, dialectName, GeometryType);
        }

        private void RegisterSpatialFunction(SpatialRelation relation)
        {
            adaptor.RegisterFunction(SpatialDialect.HqlPrefix + relation, new SpatialRelationFunction(this, relation));
        }

        private void RegisterSpatialFunction(SpatialValidation validation)
        {
            adaptor.RegisterFunction(SpatialDialect.HqlPrefix + validation, new SpatialValidationFunction(this, validation));
        }

        private void RegisterSpatialFunction(SpatialAnalysis analysis)
        {
            adaptor.RegisterFunction(SpatialDialect.HqlPrefix + analysis, new SpatialAnalysisFunction(this, analysis));
        }

        #endregion Functions registration

        #region ISpatialDialect Members

        /// <summary>
        /// Gets the type of the geometry.
        /// </summary>
        /// <value>The type of the geometry.</value>
        public IType GeometryType { get; }

        /// <summary>
        /// Creates the geometry user type.
        /// </summary>
        /// <returns></returns>
        public IGeometryUserType CreateGeometryUserType()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the spatial transform string.
        /// </summary>
        /// <param name="geometry">The geometry.</param>
        /// <param name="srid">The srid.</param>
        /// <returns></returns>
        public SqlString GetSpatialTransformString(object geometry, int srid)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the spatial aggregate string.
        /// </summary>
        /// <param name="geometry">The geometry.</param>
        /// <param name="aggregate">The aggregate.</param>
        /// <returns></returns>
        public SqlString GetSpatialAggregateString(object geometry, SpatialAggregate aggregate)
        {
            string aggregateFunction;
            switch (aggregate)
            {
                case SpatialAggregate.Collect:
                    aggregateFunction = "CollectionAggregate";
                    break;

                case SpatialAggregate.Envelope:
                    aggregateFunction = "EnvelopeAggregate";
                    break;

                case SpatialAggregate.ConvexHull:
                    aggregateFunction = "ConvexHullAggregate";
                    break;

                case SpatialAggregate.Union:
                    aggregateFunction = "UnionAggregate";
                    break;

                default:
                    throw new ArgumentException("Invalid spatial aggregate argument");
            }

            aggregateFunction = sqlTypeName + "::" + aggregateFunction;
            return new SqlStringBuilder()
                .Add(aggregateFunction)
                .Add("(")
                .AddObject(geometry)
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
        public SqlString GetSpatialAnalysisString(object geometry, SpatialAnalysis analysis, object extraArgument)
        {
            switch (analysis)
            {
                case SpatialAnalysis.ConvexHull:
                    return new SqlStringBuilder()
                        .AddObject(geometry)
                        .Add(".STConvexHull()")
                        .ToSqlString();

                case SpatialAnalysis.Distance:
                case SpatialAnalysis.Buffer:
                case SpatialAnalysis.Difference:
                case SpatialAnalysis.Intersection:
                case SpatialAnalysis.SymDifference:
                case SpatialAnalysis.Union:
                    if (analysis == SpatialAnalysis.Buffer && !(extraArgument is Parameter || new SqlString(Parameter.Placeholder).Equals(extraArgument)))
                    {
                        extraArgument = Convert.ToString(extraArgument, NumberFormatInfo.InvariantInfo);
                    }
                    return new SqlStringBuilder()
                        .AddObject(geometry)
                        .Add(".ST")
                        .Add(analysis.ToString())
                        .Add("(")
                        .AddObject(extraArgument)
                        .Add(")")
                        .ToSqlString();

                default:
                    throw new ArgumentException("Invalid spatial analysis argument");
            }
        }

        /// <summary>
        /// Gets the spatial validation string.
        /// </summary>
        /// <param name="geometry">The geometry.</param>
        /// <param name="validation">The validation.</param>
        /// <param name="criterion">if set to <c>true</c> [criterion].</param>
        /// <returns></returns>
        public SqlString GetSpatialValidationString(object geometry, SpatialValidation validation, bool criterion)
        {
            // the MSDN say that "STIsValid" should return a CLR-SqlBoolean but our test "TestBooleanUnaryOperation" say that it may return null
            // when the geometry is null.
            return new SqlStringBuilder()
                .Add("COALESCE(")
                .AddObject(geometry)
                .Add(".ST")
                .Add(validation.ToString())
                .Add("()")
                .Add(", 0)")
                .Add(criterion ? " = 1" : "")
                .ToSqlString();
        }

        /// <summary>
        /// Gets the spatial relate string.
        /// </summary>
        /// <param name="geometry">The geometry.</param>
        /// <param name="anotherGeometry">Another geometry.</param>
        /// <param name="pattern">The pattern.</param>
        /// <param name="isStringPattern">if set to <c>true</c> [is string pattern].</param>
        /// <param name="criterion">if set to <c>true</c> [criterion].</param>
        /// <returns></returns>
        public SqlString GetSpatialRelateString(object geometry, object anotherGeometry, object pattern, bool isStringPattern, bool criterion)
        {
            if (pattern == null)
            {
                throw new ArgumentNullException(nameof(pattern),
                    "MS SQL does not support the syntax returning a DE-9IM for the two geometries. For STRelate the 'intersection_pattern_matrix' is mandatory and the result is a boolean.");
            }
            var builder = new SqlStringBuilder();
            builder
                .AddObject(geometry)
                .Add(".STRelate(")
                .AddObject(anotherGeometry);
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
            return builder
                .Add(")")
                .Add(criterion ? " = 1" : "")
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
                    builder.Add("(CASE WHEN ");
                    for (int i = 0; i < patterns.Length; i++)
                    {
                        if (i > 0)
                        {
                            builder.Add(" OR ");
                        }
                        builder
                            .AddObject(geometry)
                            .Add(".STRelate(")
                            .AddObject(anotherGeometry)
                            .Add(", '")
                            .Add(patterns[i])
                            .Add("') = 1")
                            .ToSqlString();
                    }
                    builder.Add(" THEN 1 ELSE 0 END)");
                    builder.Add(criterion ? " = 1" : "");
                    return builder.ToSqlString();

                case SpatialRelation.CoveredBy:
                    return GetSpatialRelationString(anotherGeometry, SpatialRelation.Covers, geometry, criterion);

                default:
                    // NOTE: Cast is only required if "geometry" is passed in as a parameter
                    //       directly, rather than as a column name. This is because parameter
                    //       will be passed as binary and SQL Server can't call methods on
                    //       binary data.
                    return new SqlStringBuilder()
                        .Add("CAST(")
                        .AddObject(geometry)
                        .Add(" AS ")
                        .Add(sqlTypeName)
                        .Add(")")
                        .Add(".ST")
                        .Add(relation.ToString())
                        .Add("(")
                        .AddObject(anotherGeometry)
                        .Add(")")
                        .Add(criterion ? " = 1" : "")
                        .ToSqlString();
            }
        }

        public SqlString GetSpatialRelationString(object geometry, SpatialRelation relation, object anotherGeometry, object parameter, bool criterion)
        {
            switch (relation)
            {
                case SpatialRelation.IsWithinDistance:
                    return new SqlStringBuilder()
                        .Add("CASE WHEN ")
                        .AddObject(geometry)
                        .Add(".STDistance(")
                        .AddObject(anotherGeometry)
                        .Add(")")
                        .Add(" <= ")
                        .Add(parameter.ToString())
                        .Add(" THEN 1 ELSE 0 END")
                        .Add(criterion ? " = 1" : "")
                        .ToSqlString();
                default:
                    throw new ArgumentOutOfRangeException(nameof(relation), relation, "Unsupported spatial relation");
            }
        }

        public SqlString GetSpatialFilterString(string tableAlias, string geometryColumnName, string primaryKeyColumnName, string tableName, Parameter parameter)
        {
            return new SqlStringBuilder(6)
                .Add(tableAlias)
                .Add(".")
                .Add(geometryColumnName)
                .Add(".Filter(")
                .Add(parameter)
                .Add(") = 1")
                .ToSqlString();
        }

        /// <summary>
        /// Gets the spatial create string.
        /// </summary>
        /// <param name="schema">The schema.</param>
        /// <returns></returns>
        public string GetSpatialCreateString(string schema)
        {
            string viewScript = string.Format(@"
				CREATE VIEW {0}{1} AS
				SELECT
				F_TABLE_CATALOG = cols.TABLE_CATALOG,
				F_TABLE_SCHEMA = cols.TABLE_SCHEMA,
				F_TABLE_NAME = cols.TABLE_NAME,
				F_GEOMETRY_COLUMN = cols.COLUMN_NAME,
				SRID = COALESCE(srid_checks.SRID, 0),
				TYPE = COALESCE(type_checks.TYPE, 'GEOMETRY'),
				COORD_DIMENSION = 2 -- TO-DO
				FROM INFORMATION_SCHEMA.COLUMNS cols
				LEFT JOIN (
					SELECT
					 TABLE_NAME = t.name
					,TABLE_SCHEMA = sc.name
					,COLUMN_NAME = c.name
					,SRID = CAST(
						SUBSTRING(
							ck.definition,
							CHARINDEX('].[STSrid]=(', ck.definition) + 12,
							LEN(ck.definition) - CHARINDEX('].[STSrid]=(', ck.definition) - 13
						)
						AS INTEGER
					)
					FROM sys.check_constraints ck, sys.schemas sc, sys.columns c, sys.tables t
					WHERE ck.schema_id = sc.schema_id
					AND ck.parent_column_id = c.column_id
					AND ck.parent_object_id = c.object_id
					AND CHARINDEX('].[STSrid]=(', ck.definition) <> 0
					AND c.object_id = t.object_id
				) as srid_checks
				ON cols.TABLE_SCHEMA = srid_checks.TABLE_SCHEMA
				AND cols.TABLE_NAME = srid_checks.TABLE_NAME
				AND cols.COLUMN_NAME = srid_checks.COLUMN_NAME

				LEFT JOIN (
					SELECT
					 TABLE_NAME = t.name
					,TABLE_SCHEMA = sc.name
					,COLUMN_NAME = c.name
					,TYPE = UPPER(
						SUBSTRING(
							ck.definition,
							CHARINDEX('].[STGeometryType]()=''', ck.definition) + 22,
							LEN(ck.definition) - CHARINDEX('].[STGeometryType]()=''', ck.definition) - 23
						)
					)
					FROM sys.check_constraints ck, sys.schemas sc, sys.columns c, sys.tables t
					WHERE ck.schema_id = sc.schema_id
					AND ck.parent_column_id = c.column_id
					AND ck.parent_object_id = c.object_id
					AND CHARINDEX('].[STGeometryType]()=''', ck.definition) <> 0
					AND c.object_id = t.object_id
				) as type_checks
				ON cols.TABLE_SCHEMA = type_checks.TABLE_SCHEMA
				AND cols.TABLE_NAME = type_checks.TABLE_NAME
				AND cols.COLUMN_NAME = type_checks.COLUMN_NAME

				WHERE DATA_TYPE IN ('{2}')

				"
                , adaptor.QuoteSchema(schema)
                , adaptor.Quote(geometryColumnsViewName)
                , sqlTypeName
            );

            string script = string.Format(@"
				IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'{0}{1}'))
				DROP VIEW {0}{1}

				{2}

				EXECUTE('{3}')

				{2}
				"
                , adaptor.QuoteSchema(schema)
                , adaptor.Quote(geometryColumnsViewName)
                , MultipleQueriesSeparator
                , viewScript.Replace("'", "''")
            );

            return script;
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
        public string GetSpatialCreateString(string schema, string table, string column, int srid, string subtype, int dimension, bool isNullable)
        {
            var builder = new StringBuilder();

            string quotedSchema = adaptor.QuoteSchema(schema);
            string quoteForTableName = adaptor.QuoteForTableName(table);
            string quoteForColumnName = adaptor.QuoteForColumnName(column);

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
                , sqlTypeName
                , isNullable ? "NULL" : "NOT NULL"
            );

            builder.Append(MultipleQueriesSeparator);

            if (srid > 0)
            {
                // EXECUTE is needed to avoid the error "The multi-part identifier could not be bound."
                builder.AppendFormat("EXECUTE('ALTER TABLE {0}{1} WITH CHECK ADD  CONSTRAINT {2} CHECK ({3}.{4} = {5})')"
                    , quotedSchema
                    , quoteForTableName
                    , adaptor.Quote("CK_NHSP_" + table + "_" + column + "_SRID")
                    , quoteForColumnName
                    , adaptor.Quote("STSrid")
                    , srid
                );

                builder.Append(MultipleQueriesSeparator);
            }

            if (!string.IsNullOrEmpty(subtype) && string.Compare(subtype, "GEOMETRY") != 0)
            {
                builder.AppendFormat("ALTER TABLE {0}{1} WITH CHECK ADD  CONSTRAINT {2} CHECK ({3}.{4}() = '{5}')"
                    , quotedSchema
                    , quoteForTableName
                    , adaptor.Quote("CK_NHSP_" + table + "_" + column + "_TYPE")
                    , quoteForColumnName
                    , adaptor.Quote("STGeometryType")
                    , subtype
                );

                builder.Append(MultipleQueriesSeparator);
            }

            return builder.ToString();
        }

        /// <summary>
        /// Gets the spatial drop string.
        /// </summary>
        /// <param name="schema">The schema.</param>
        /// <returns></returns>
        public string GetSpatialDropString(string schema)
        {
            string script = string.Format(@"
				IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'{0}{1}'))
				DROP VIEW {0}{1}
				"
                , adaptor.QuoteSchema(schema)
                , adaptor.Quote(geometryColumnsViewName)
            );
            return script;
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
            var builder = new StringBuilder();

            string quotedSchema = null;
            if (!string.IsNullOrEmpty(schema))
            {
                quotedSchema = adaptor.QuoteForSchemaName(schema) + StringHelper.Dot;
            }

            builder.AppendFormat("ALTER TABLE {0}{1} DROP COLUMN {2}"
                , quotedSchema
                , adaptor.QuoteForTableName(table)
                , adaptor.QuoteForColumnName(column)
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
        public bool SupportsSpatialMetadata(MetadataClass metadataClass)
        {
            //return metadataClass == MetadataClass.GeometryColumn;
            return false;
        }

        // TODO: Use ISessionFactory.ConnectionProvider.Driver.MultipleQueriesSeparator
        public string MultipleQueriesSeparator => ";";

        #endregion ISpatialDialect Members
    }
}
