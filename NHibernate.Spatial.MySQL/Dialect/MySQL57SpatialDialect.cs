﻿// Copyright 2016 - Andreas Ravnestad (andreas.ravnestad@gmail.com)
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

using System;
using NHibernate.Spatial.Type;
using NHibernate.SqlCommand;
using NHibernate.Type;

namespace NHibernate.Spatial.Dialect
{
    /// <summary>
    /// MySQL spatial dialect that supports the changes introduced in MySQL 5.7
    /// </summary>
    public class MySQL57SpatialDialect : MySQLSpatialDialect
    {
        protected new static readonly IType geometryType = new CustomType(typeof(MySQL57GeometryType), null);

        public override IType GeometryType => geometryType;

        public override IGeometryUserType CreateGeometryUserType()
        {
            return new MySQL57GeometryType();
        }

        /// <summary>
        /// Gets the spatial aggregate string.
        /// </summary>
        /// <param name="geometry">The geometry.</param>
        /// <param name="aggregate">The aggregate.</param>
        /// <returns></returns>
        public override SqlString GetSpatialAggregateString(object geometry, SpatialAggregate aggregate)
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

        public override SqlString GetSpatialAnalysisString(object geometry, SpatialAnalysis analysis, object extraArgument)
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

        protected override void RegisterFunctions()
        {
            base.RegisterFunctions();

            // Fixes error when using AsText() with MySQL 5.7 or newer
            RegisterSpatialFunction("AsText", NHibernateUtil.String);

            // Fixes error when using GeometryType() with MySQL 5.7 or newer
            RegisterSpatialFunction("GeometryType", NHibernateUtil.String);
        }
    }
}
