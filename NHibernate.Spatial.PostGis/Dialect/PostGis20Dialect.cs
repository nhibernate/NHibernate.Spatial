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

using NHibernate.SqlCommand;
using NHibernate.Type;
using System;

namespace NHibernate.Spatial.Dialect
{
    /// <summary>
    /// 
    /// </summary>
    public class PostGis20Dialect : PostGisDialect
    {
        protected override void RegisterSpatialFunction(string name, IType returnedType, int allowedArgsCount)
        {
            RegisterSpatialFunction(name, SpatialDialect.IsoPrefix + name, returnedType, allowedArgsCount);
        }

        protected override void RegisterSpatialFunction(string name, IType returnedType)
        {
            RegisterSpatialFunction(name, SpatialDialect.IsoPrefix + name, returnedType);
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
                case SpatialAggregate.Collect:
                    aggregateFunction = SpatialDialect.IsoPrefix + "Collect";
                    break;
                case SpatialAggregate.ConvexHull:
                    aggregateFunction = SpatialDialect.IsoPrefix + "ConvexHull";
                    break;
                case SpatialAggregate.Envelope:
                    aggregateFunction = SpatialDialect.IsoPrefix + "Extent";
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

        /// <summary>
        /// Gets the spatial create string.
        /// </summary>
        /// <param name="schema">The schema.</param>
        /// <returns></returns>
        public override string GetSpatialCreateString(string schema)
        {
            return GetSpatialCreateString(schema, SpatialDialect.IsoPrefix);
        }
    }
}
