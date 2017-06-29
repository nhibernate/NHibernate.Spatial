// Copyright 2008 - Ricardo Stuven (rstuven@gmail.com)
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
    internal class MsSql2012FunctionRegistration : MsSql2008FunctionRegistration
    {
        public MsSql2012FunctionRegistration(IRegisterationAdaptor adaptor, string sqlTypeName, string geometryColumnsViewName, IType geometryType)
            : base(adaptor, sqlTypeName, geometryColumnsViewName, geometryType)
        { }

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
    }
}
