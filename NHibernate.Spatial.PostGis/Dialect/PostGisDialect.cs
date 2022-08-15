// Copyright 2007 - Ricardo Stuven (rstuven@gmail.com)
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
    public class PostGisDialect : PostgreSQL82Dialect, ISpatialDialect
    {
        private static readonly IType geometryType = new CustomType(typeof(PostGisGeometryType), null);

        protected const string IntersectionAggregateName = "NHSP_IntersectionAggregate";

        /// <summary>
        /// Initializes a new instance of the <see cref="PostGisDialect"/> class.
        /// </summary>
        public PostGisDialect()
        {
            SpatialDialect.LastInstantiated = this;
            RegisterBasicFunctions();
            RegisterFunctions();
        }

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

        protected virtual void RegisterSpatialFunction(string name, IType returnedType, int allowedArgsCount)
        {
            RegisterSpatialFunction(name, name, returnedType, allowedArgsCount);
        }

        protected virtual void RegisterSpatialFunction(string name, IType returnedType)
        {
            RegisterSpatialFunction(name, name, returnedType);
        }

        protected void RegisterSpatialFunction(string name, int allowedArgsCount)
        {
            RegisterSpatialFunction(name, this.GeometryType, allowedArgsCount);
        }

        protected void RegisterSpatialFunction(string name)
        {
            RegisterSpatialFunction(name, this.GeometryType);
        }

        private void RegisterSpatialFunction(SpatialRelation relation)
        {
            RegisterFunction(SpatialDialect.HqlPrefix + relation.ToString(), new SpatialRelationFunction(this, relation));
        }

        private void RegisterSpatialFunction(SpatialValidation validation)
        {
            RegisterFunction(SpatialDialect.HqlPrefix + validation.ToString(), new SpatialValidationFunction(this, validation));
        }

        private void RegisterSpatialFunction(SpatialAnalysis analysis)
        {
            RegisterFunction(SpatialDialect.HqlPrefix + analysis.ToString(), new SpatialAnalysisFunction(this, analysis));
        }

        #endregion Functions registration

        #region ISpatialDialect Members

        /// <summary>
        /// Creates the geometry user type.
        /// </summary>
        /// <returns></returns>
        public IGeometryUserType CreateGeometryUserType()
        {
            return new PostGisGeometryType();
        }

        /// <summary>
        /// Gets the type of the geometry.
        /// </summary>
        /// <value>The type of the geometry.</value>
        public IType GeometryType
        {
            get { return geometryType; }
        }

        /// <summary>
        /// Gets the spatial transform string.
        /// </summary>
        /// <param name="geometry">The geometry.</param>
        /// <param name="srid">The srid.</param>
        /// <returns></returns>
        public SqlString GetSpatialTransformString(object geometry, int srid)
        {
            return new SqlStringBuilder()
                .Add(SpatialDialect.IsoPrefix)
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
        public SqlString GetSpatialValidationString(object geometry, SpatialValidation validation, bool criterion)
        {
            return new SqlStringBuilder()
                .Add(SpatialDialect.IsoPrefix)
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
            // PostGIS aggregate functions do not need prefix
            string aggregateFunction;
            switch (aggregate)
            {
                case SpatialAggregate.Collect:
                    aggregateFunction = "Collect";
                    break;

                case SpatialAggregate.Envelope:
                    aggregateFunction = "Extent";
                    break;

                case SpatialAggregate.Intersection:
                    aggregateFunction = IntersectionAggregateName;
                    break;

                case SpatialAggregate.Union:
                    aggregateFunction = SpatialDialect.IsoPrefix + "Union";
                    break;

                default:
                    throw new ArgumentException("Invalid spatial aggregate argument");
            }
            var builder = new SqlStringBuilder();
            builder.Add(aggregateFunction)
                .Add("(")
                .AddObject(geometry)
                .Add(")");
            // Convert to geometry as there is no binary output function available for type box2d type
            if (aggregate == SpatialAggregate.Envelope)
            {
                builder.Add("::geometry");
            }
            return builder.ToSqlString();
        }

        public SqlString GetSpatialRelationString(object geometry, SpatialRelation relation, object anotherGeometry, bool criterion)
        {
            switch (relation)
            {
                case SpatialRelation.Covers:
                    string[] patterns = new string[] {
						"T*****FF*",
						"*T****FF*",
						"***T**FF*",
						"****T*FF*",
					};
                    SqlStringBuilder builder = new SqlStringBuilder();
                    builder.Add("(");
                    for (int i = 0; i < patterns.Length; i++)
                    {
                        if (i > 0)
                            builder.Add(" OR ");
                        builder
                            .Add(SpatialDialect.IsoPrefix)
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
                        .Add("::text")
                        .Add(", ")
                        .AddObject(anotherGeometry)
                        .Add("::text")
                        .Add(")")
                        .ToSqlString();
            }
        }

        public SqlString GetSpatialRelateString(object geometry, object anotherGeometry, object pattern, bool isStringPattern, bool criterion)
        {
            SqlStringBuilder builder = new SqlStringBuilder();
            builder
                .Add(SpatialDialect.IsoPrefix)
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
                        .Add((string)pattern)
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
            return new SqlStringBuilder(6)
                .Add("(")
                .Add(tableAlias)
                .Add(".")
                .Add(geometryColumnName)
                .Add(" && ")
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
        public SqlString GetSpatialAnalysisString(object geometry, SpatialAnalysis analysis, object extraArgument)
        {
            switch (analysis)
            {
                case SpatialAnalysis.Buffer:
                    if (!(extraArgument is Parameter || new SqlString(SqlCommand.Parameter.Placeholder).Equals(extraArgument)))
                    {
                        extraArgument = Convert.ToString(extraArgument, System.Globalization.NumberFormatInfo.InvariantInfo);
                    }
                    return new SqlStringBuilder(6)
                        .Add(SpatialDialect.IsoPrefix)
                        .Add("Buffer(")
                        .AddObject(geometry)
                        .Add(", ")
                        .AddObject(extraArgument)
                        .Add(")")
                        .ToSqlString();

                case SpatialAnalysis.ConvexHull:
                    return new SqlStringBuilder()
                        .Add(SpatialDialect.IsoPrefix)
                        .Add("ConvexHull(")
                        .AddObject(geometry)
                        .Add(")")
                        .ToSqlString();

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
                        .Add("::text")
                        .Add(",")
                        .AddObject(extraArgument)
                        .Add("::text")
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
        public virtual string GetSpatialCreateString(string schema)
        {
            return GetSpatialCreateString(schema, string.Empty);
        }

        protected string GetSpatialCreateString(string schema, string prefix)
        {
            // Intersection aggregate function by Mark Fenbers
            // See http://postgis.refractions.net/pipermail/postgis-users/2005-May/008156.html
            // NOTE: An additional paramter was added to ST_Intersection in PostGIS 3.1
            //       https://github.com/nhibernate/NHibernate.Spatial/issues/129
            string script = string.Format(@"
                CREATE OR REPLACE FUNCTION {0}NHSP_CreateIntersectionAggregate()
                RETURNS void AS $$
                    BEGIN
                        DROP AGGREGATE IF EXISTS {0}{1}(GEOMETRY);
                        IF (SELECT PostGIS_Lib_Version() < '3.1') THEN
                            CREATE AGGREGATE {0}{1}(basetype=geometry, sfunc={2}Intersection, stype=geometry);
                        ELSE
                            CREATE OR REPLACE FUNCTION {0}NHSP_Intersection(geometry, geometry)
                                RETURNS geometry
                                LANGUAGE SQL
                                AS 'SELECT {0}{2}Intersection($1, $2)'
                                IMMUTABLE STRICT
                                COST 10000;
                            CREATE AGGREGATE {0}{1}(basetype=geometry, sfunc={0}NHSP_Intersection, stype=geometry);
                        END IF;
                    END;
                $$ LANGUAGE plpgsql;

                -- NOTE: Cast to text required to avoid following error on PostgreSQL 9.0 and lower:
                --       Npgsql.PostgresException : 42883: no binary output function available for type void
                --       https://github.com/npgsql/npgsql/issues/818
                --       https://www.postgresql.org/message-id/21459.1437692282%40sss.pgh.pa.us
                SELECT {0}NHSP_CreateIntersectionAggregate()::text;"
                , this.QuoteSchema(schema)
                , IntersectionAggregateName
                , prefix
                );
            return script;
        }

        /// <summary>
        /// Quotes the schema.
        /// </summary>
        /// <param name="schema">The schema.</param>
        /// <returns></returns>
        private string QuoteSchema(string schema)
        {
            if (string.IsNullOrEmpty(schema))
            {
                return null;
            }
            return this.QuoteForSchemaName(schema) + StringHelper.Dot;
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
            StringBuilder builder = new StringBuilder();

            builder.AppendFormat("ALTER TABLE {0}{1} DROP COLUMN {2}"
                , this.QuoteSchema(schema)
                , this.QuoteForTableName(table)
                , this.QuoteForColumnName(column)
                );

            builder.Append(this.MultipleQueriesSeparator);

            builder.AppendFormat("SELECT AddGeometryColumn('{0}','{1}','{2}',{3},'{4}',{5})",
                schema, table, column, srid, subtype, dimension);

			if (!isNullable)
			{
				builder.Append(this.MultipleQueriesSeparator);
				builder.AppendFormat("ALTER TABLE {0}{1} ALTER COLUMN {2} SET NOT NULL"
					, this.QuoteSchema(schema)
					, this.QuoteForTableName(table)
					, this.QuoteForColumnName(column)
					);
			}

			builder.Append(this.MultipleQueriesSeparator);
			builder.Append(GetSpatialIndexCreateString(schema, table, column));

            return builder.ToString();
        }

        private string GetSpatialIndexCreateString(string schema, string table, string column)
        {
            StringBuilder builder = new StringBuilder();

            builder.Append(GetSpatialIndexDropString(schema, table, column));
            builder.Append(this.MultipleQueriesSeparator);

            if (string.IsNullOrEmpty(schema))
                builder.AppendFormat("CREATE INDEX {0}_{1}_idx ON {2} USING GIST ({3})",
                                table, column, this.QuoteForTableName(table), this.QuoteForColumnName(column));
            else
                builder.AppendFormat("CREATE INDEX {0}_{1}_idx ON {2}{3} USING GIST ({4})",
                                table, column, this.QuoteSchema(schema), this.QuoteForTableName(table), this.QuoteForColumnName(column));
            builder.Append(this.MultipleQueriesSeparator);

            return builder.ToString();
        }

        /// <summary>
        /// Gets the spatial drop string.
        /// </summary>
        /// <param name="schema">The schema.</param>
        /// <returns></returns>
        public string GetSpatialDropString(string schema)
        {
            string script = string.Format(
                "DROP AGGREGATE IF EXISTS {0}{1}(GEOMETRY);"
                , this.QuoteSchema(schema)
                , IntersectionAggregateName
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
            StringBuilder builder = new StringBuilder();

            builder.Append(GetSpatialIndexDropString(schema, table, column));
            if (string.IsNullOrEmpty(schema))
                builder.AppendFormat("SELECT DropGeometryColumn('{0}','{1}')",
                    table, column);
            else
                builder.AppendFormat("SELECT DropGeometryColumn('{0}','{1}','{2}')",
                    schema, table, column);
            builder.Append(this.MultipleQueriesSeparator);

            return builder.ToString();
        }

        private string GetSpatialIndexDropString(string schema, string table, string column)
        {
            StringBuilder builder = new StringBuilder();

            if (string.IsNullOrEmpty(schema))
                builder.AppendFormat("DROP INDEX IF EXISTS {0}_{1}_idx",
                                     table, column);
            else
                builder.AppendFormat("DROP INDEX IF EXISTS {0}{1}_{2}_idx",
                                     this.QuoteSchema(schema), table, column);

            builder.Append(this.MultipleQueriesSeparator);

            return builder.ToString();
        }

        /// <summary>
        /// Gets a value indicating whether it supports spatial metadata.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if it supports spatial metadata; otherwise, <c>false</c>.
        /// </value>
        public bool SupportsSpatialMetadata(MetadataClass metadataClass)
        {
            return true;
        }

        #endregion ISpatialDialect Members

        // TODO: Use ISessionFactory.ConnectionProvider.Driver.MultipleQueriesSeparator
        public string MultipleQueriesSeparator
        {
            get { return ";"; }
        }
    }
}