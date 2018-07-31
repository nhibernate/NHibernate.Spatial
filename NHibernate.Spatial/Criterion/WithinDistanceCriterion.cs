// Copyright 2007 - Ricardo Stuven (rstuven@gmail.com)
//
// This file is part of NHibernate.Spatial.
// NHibernate.Spatial is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// MsSqlSpatial is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.

// You should have received a copy of the GNU Lesser General Public License
// along with MsSqlSpatial; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

using System;
using System.Collections;
using NHibernate.Criterion;
using NHibernate.Engine;
using NHibernate.SqlCommand;

namespace NHibernate.Spatial.Criterion
{
	[Serializable]
	public class WithinDistanceCriterion : AbstractCriterion
	{
		public override TypedValue[] GetTypedValues(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			throw new Exception("The method or operation is not implemented.");
		}

        public override IProjection[] GetProjections()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override SqlString ToSqlString(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override string ToString()
		{
			throw new Exception("The method or operation is not implemented.");
		}
	}
}
