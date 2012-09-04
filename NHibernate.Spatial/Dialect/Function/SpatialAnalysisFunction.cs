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

using System.Collections;
using NHibernate.Engine;
using NHibernate.SqlCommand;

namespace NHibernate.Spatial.Dialect.Function
{
	/// <summary>
	/// 
	/// </summary>
	public class SpatialAnalysisFunction : SpatialStandardSafeFunction
	{
		private readonly ISpatialDialect spatialDialect;
		private readonly SpatialAnalysis analysis;

		/// <summary>
		/// Initializes a new instance of the <see cref="SpatialAnalysisFunction"/> class.
		/// </summary>
		/// <param name="spatialDialect">The spatial dialect.</param>
		/// <param name="analysis">The analysis.</param>
		public SpatialAnalysisFunction(ISpatialDialect spatialDialect, SpatialAnalysis analysis)
			: base(analysis.ToString(),
			  (analysis == SpatialAnalysis.Distance ?
					NHibernateUtil.Double :
					spatialDialect.GeometryType)
			  )
		{
			this.spatialDialect = spatialDialect;
			this.analysis = analysis;
			if (this.analysis == SpatialAnalysis.ConvexHull)
			{
				this.allowedArgsCount = 1;
			}
			else
			{
				this.allowedArgsCount = 2;
			}
		}

		/// <summary>
		/// Render the function call as SQL.
		/// </summary>
		/// <param name="args">List of arguments</param>
		/// <param name="factory"></param>
		/// <returns>SQL fragment for the function.</returns>
		public override SqlString Render(IList args, ISessionFactoryImplementor factory)
		{
			ValidateArgsCount(args);
			if (this.analysis == SpatialAnalysis.ConvexHull)
			{
				return this.spatialDialect.GetSpatialAnalysisString(args[0], this.analysis, null);
			}
			return this.spatialDialect.GetSpatialAnalysisString(args[0], this.analysis, args[1]);
		}

	}
}
