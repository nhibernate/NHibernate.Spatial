using System;
using System.Globalization;
using System.Text;
using NHibernate.Dialect;
using NHibernate.Spatial.Dialect.Function;
using NHibernate.Spatial.Metadata;
using NHibernate.Spatial.Type;
using NHibernate.SqlCommand;
using NHibernate.Type;

namespace NHibernate.Spatial.Dialect
{
    public class SpatiaLiteDialect : SQLiteDialect, ISpatialDialect
    {
        private static readonly IType GeometryTypeInstance = new CustomType(typeof(SpatiaLiteGeometryType), null);

        public SpatiaLiteDialect()
        {
            SpatialDialect.LastInstantiated = this;

            RegisterBasicFunctions();
            RegisterFunctions();
        }

        public IType GeometryType => GeometryTypeInstance;

        public string MultipleQueriesSeparator => ";";

        public IGeometryUserType CreateGeometryUserType()
        {
            return new SpatiaLiteGeometryType();
        }

        public SqlString GetSpatialTransformString(object geometry, int srid)
        {
            return new SqlStringBuilder()
                .Add(SpatialDialect.IsoPrefix)
                .Add("Transform(")
                .AddObject(geometry)
                .Add(",")
                .Add(srid.ToString(NumberFormatInfo.InvariantInfo))
                .Add(")")
                .ToSqlString();
        }

        public SqlString GetSpatialAggregateString(object geometry, SpatialAggregate aggregate)
        {
            string aggregateFunction;
            switch (aggregate)
            {
                case SpatialAggregate.Collect:
                    aggregateFunction = "Collect";
                    break;
                case SpatialAggregate.ConvexHull:
                    return new SqlStringBuilder()
                        .Add(SpatialDialect.IsoPrefix)
                        .Add("Collect(")
                        .Add(SpatialDialect.IsoPrefix)
                        .Add(aggregate.ToString())
                        .Add("(")
                        .AddObject(geometry)
                        .Add("))")
                        .ToSqlString();
                case SpatialAggregate.Envelope:
                    aggregateFunction = "Extent";
                    break;
                case SpatialAggregate.Intersection:
                    throw new NotSupportedException("SpatialAggregate Intersection"); // TODO Check whether collect works here too
                case SpatialAggregate.Union:
                    aggregateFunction = "GUnion";
                    break;
                default:
                    throw new ArgumentException("Invalid spatial aggregate argument");
            }
            return new SqlStringBuilder()
                .Add(aggregateFunction)
                .Add("(")
                .AddObject(geometry)
                .Add(")")
                .ToSqlString();
        }

        public SqlString GetSpatialAnalysisString(object geometry,
                                                  SpatialAnalysis analysis,
                                                  object extraArgument)
        {
            switch (analysis)
            {
                case SpatialAnalysis.Buffer:
                    if (!(extraArgument is Parameter || new SqlString(Parameter.Placeholder).Equals(extraArgument)))
                    {
                        extraArgument = Convert.ToString(extraArgument, NumberFormatInfo.InvariantInfo);
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
                        .Add(",")
                        .AddObject(extraArgument)
                        .Add(")")
                        .ToSqlString();
                default:
                    throw new ArgumentException("Invalid spatial analysis argument");
            }
        }

        public SqlString GetSpatialValidationString(object geometry,
                                                    SpatialValidation validation,
                                                    bool criterion)
        {
            return new SqlStringBuilder()
                .Add(SpatialDialect.IsoPrefix)
                .Add(validation.ToString())
                .Add("(")
                .AddObject(geometry)
                .Add(")")
                .Add(" = 1")
                .ToSqlString();
        }

        public SqlString GetSpatialRelateString(object geometry,
                                                object anotherGeometry,
                                                object pattern,
                                                bool isStringPattern,
                                                bool criterion)
        {
            var relateString = new SqlStringBuilder()
                .Add(SpatialDialect.IsoPrefix)
                .Add("Relate(")
                .AddObject(geometry)
                .Add(",")
                .AddObject(anotherGeometry)
                .Add(",");

            if (isStringPattern)
            {
                relateString.Add("'");
            }

            relateString.Add(pattern.ToString());

            if (isStringPattern)
            {
                relateString.Add("'");
            }

            relateString.Add(")");

            return relateString.ToSqlString();
        }

        public SqlString GetSpatialRelationString(object geometry,
                                                  SpatialRelation relation,
                                                  object anotherGeometry,
                                                  bool criterion)
        {
            switch (relation)
            {
                case SpatialRelation.Covers:
                    string[] patterns =
                    {
                        "T*****FF*",
                        "*T****FF*",
                        "***T**FF*",
                        "****T*FF*"
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
                case SpatialRelation.EqualsExact:
                    throw new ArgumentOutOfRangeException(nameof(relation), relation, "Unsupported spatial relation");
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

        public SqlString GetSpatialRelationString(object geometry, SpatialRelation relation, object anotherGeometry, object parameter, bool criterion)
        {
            switch (relation)
            {
                case SpatialRelation.IsWithinDistance:
                    return new SqlStringBuilder()
                        .Add("PtDistWithin")
                        .Add("(")
                        .AddObject(geometry)
                        .Add(", ")
                        .AddObject(anotherGeometry)
                        .Add(", ")
                        .Add(parameter.ToString())
                        .Add(")")
                        .ToSqlString();
                case SpatialRelation.Relate:
                    return new SqlStringBuilder()
                        .Add(SpatialDialect.IsoPrefix)
                        .Add(relation.ToString())
                        .Add("(")
                        .AddObject(geometry)
                        .Add(", ")
                        .AddObject(anotherGeometry)
                        .Add(", '")
                        .Add(parameter.ToString())
                        .Add("')")
                        .ToSqlString();
                default:
                    throw new ArgumentOutOfRangeException(nameof(relation), relation, "Unsupported spatial relation");
            }
        }

        public SqlString GetSpatialFilterString(string tableAlias,
                                                string geometryColumnName,
                                                string primaryKeyColumnName,
                                                string tableName,
                                                Parameter parameter)
        {
            return new SqlStringBuilder(30)
                .Add("(MbrIntersects(")
                .Add(tableAlias)
                .Add(".")
                .Add(geometryColumnName)
                .Add(", ")
                .Add(parameter)
                .Add("))")
                .ToSqlString();
        }

        public SqlString GetSpatialFilterString(string tableAlias,
                                                string geometryColumnName,
                                                string primaryKeyColumnName,
                                                string tableName)
        {
            return new SqlStringBuilder(30)
                .Add("(MbrIntersects(")
                .Add(tableAlias)
                .Add(".")
                .Add(geometryColumnName)
                .Add(", ")
                .AddParameter()
                .Add("))")
                .ToSqlString();
        }

        /// <summary>
        /// Gets the spatial create string.
        /// </summary>
        /// <param name="schema">The schema.</param>
        /// <returns></returns>
        public string GetSpatialCreateString(string schema)
        {
            return string.Empty;
        }

        /// <summary>
        /// Gets the spatial create string.
        /// </summary>
        /// <param name="schema"></param>
        /// <param name="table"></param>
        /// <param name="column"></param>
        /// <param name="srid"></param>
        /// <param name="subtype"></param>
        /// <param name="dimension"></param>
        /// <param name="isNullable"></param>
        /// <returns></returns>
        public string GetSpatialCreateString(string schema,
                                             string table,
                                             string column,
                                             int srid,
                                             string subtype,
                                             int dimension,
                                             bool isNullable)
        {
            var builder = new StringBuilder();

            builder.AppendFormat("ALTER TABLE {0} DROP COLUMN {1};"
                , QuoteForTableName(table)
                , QuoteForColumnName(column)
            );

            builder.AppendFormat("SELECT AddGeometryColumn('{0}','{1}',{2},'{3}','{4}', {5});",
                table, column, srid, subtype, ToXyzm(dimension), isNullable ? 0 : 1);

            builder.AppendFormat("SELECT CreateSpatialIndex('{0}','{1}');", table, column);

            return builder.ToString();
        }

        /// <summary>
        /// Gets the spatial drop string.
        /// </summary>
        /// <param name="schema">The schema.</param>
        /// <returns></returns>
        public string GetSpatialDropString(string schema)
        {
            return string.Empty;
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

            builder.AppendFormat("SELECT DisableSpatialIndex('{0}','{1}');",
                table, column);

            builder.AppendFormat("SELECT DiscardGeometryColumn('{0}','{1}');",
                table, column);

            builder.AppendFormat("DELETE FROM geometry_columns where f_table_name = '{0}' AND f_geometry_column = '{1}';",
                table, column);

            return builder.ToString();
        }

        public bool SupportsSpatialMetadata(MetadataClass metadataClass)
        {
            return true;
        }

        private static string ToXyzm(int dimension)
        {
            switch (dimension)
            {
                case 3:
                    return "XYZ";
                case 4:
                    return "XYZM";
            }
            return "XY";
        }

        private static int ToGeometryType(string subtype)
        {
            switch (subtype)
            {
                case "POINT":
                    return 1;
                case "LINESTRING":
                    return 2;
                case "POLYGON":
                    return 3;
                case "MULTIPOINT":
                    return 4;
                case "MULTILINESTRING":
                    return 5;
                case "MULTIPOLYGON":
                    return 6;
                case "GEOMETRYCOLLECTION":
                    return 7;
                default:
                    throw new Exception("Should never reach here");
            }
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
            RegisterSpatialFunction("Length", "GLength", NHibernateUtil.Double);
            RegisterSpatialFunction("X", NHibernateUtil.Double);
            RegisterSpatialFunction("Y", NHibernateUtil.Double);

            RegisterSpatialFunction("SRID", NHibernateUtil.Int32);
            RegisterSpatialFunction("Dimension", NHibernateUtil.Int32);
            RegisterSpatialFunction("NumGeometries", NHibernateUtil.Int32);
            RegisterSpatialFunction("NumInteriorRings", "NumInteriorRing", NHibernateUtil.Int32);
            RegisterSpatialFunction("NumPoints", NHibernateUtil.Int32);

            RegisterSpatialFunction("Relate", NHibernateUtil.Boolean, 3);
        }

        private void RegisterSpatialFunction(string standardName, string dialectName, IType returnedType, int allowedArgsCount)
        {
            RegisterFunction(SpatialDialect.HqlPrefix + standardName, new SpatialStandardSafeFunction(dialectName, returnedType, allowedArgsCount));
        }

        private void RegisterSpatialFunction(string standardName, string dialectName, IType returnedType)
        {
            RegisterSpatialFunction(standardName, dialectName, returnedType, 1);
        }

        private void RegisterSpatialFunction(string name, IType returnedType, int allowedArgsCount)
        {
            RegisterSpatialFunction(name, name, returnedType, allowedArgsCount);
        }

        private void RegisterSpatialFunction(string name, IType returnedType)
        {
            RegisterSpatialFunction(name, name, returnedType);
        }

        private void RegisterSpatialFunction(string name, int allowedArgsCount)
        {
            RegisterSpatialFunction(name, GeometryType, allowedArgsCount);
        }

        private void RegisterSpatialFunction(string name)
        {
            RegisterSpatialFunction(name, GeometryType);
        }

        private void RegisterSpatialFunction(SpatialRelation relation)
        {
            RegisterFunction(SpatialDialect.HqlPrefix + relation, new SpatialRelationFunction(this, relation));
        }

        private void RegisterSpatialFunction(SpatialValidation validation)
        {
            RegisterFunction(SpatialDialect.HqlPrefix + validation, new SpatialValidationFunction(this, validation));
        }

        private void RegisterSpatialFunction(SpatialAnalysis analysis)
        {
            RegisterFunction(SpatialDialect.HqlPrefix + analysis, new SpatialAnalysisFunction(this, analysis));
        }

        #endregion
    }
}
