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

using NHibernate.Spatial.Type;
using NHibernate.Type;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.SqlCommand;

namespace NHibernate.Spatial.Dialect
{
	/// <summary>
	/// MySQL spatial dialect that supports the changes introduced in MySQL 5.7
	/// </summary>

	public class MySQL57SpatialDialect: MySQLSpatialDialect
	{
		protected new static readonly IType geometryType = new CustomType(typeof(MySQL57GeometryType), null);

	    public MySQL57SpatialDialect():base()
	    {
	    }

        protected override void RegisterFunctions()
		{
			base.RegisterFunctions();

			// Fixes error when using AsText() with MySQL 5.7 or newer
			RegisterSpatialFunction("AsText", NHibernateUtil.String);

			// Fixes error when using GeometryType() with MySQL 5.7 or newer
			RegisterSpatialFunction("GeometryType", NHibernateUtil.String);
		}

		public override IGeometryUserType CreateGeometryUserType()
		{
			return new MySQL57GeometryType();
		}

		public override IType GeometryType
		{
			get { return geometryType; }
		}


		public override SqlString GetSpatialAnalysisString(object geometry, SpatialAnalysis analysis,
			object extraArgument)
		{
			switch (analysis)
			{
				case SpatialAnalysis.Buffer:
					if (!(extraArgument is Parameter || new SqlString(SqlCommand.Parameter.Placeholder).Equals(extraArgument)))
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
	}
}
