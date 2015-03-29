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

using NHibernate.Dialect;
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

	/// <summary>
	/// 
	/// </summary>
	public abstract class MsSql2012SpatialDialect : MsSql2008SpatialDialect, ISpatialDialect, IRegisterationAdaptor
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="MsSql2008SpatialDialect"/> class.
		/// </summary>
		protected MsSql2012SpatialDialect(string sqlTypeName, string geometryColumnsViewName, IType geometryType)
			: base(sqlTypeName, geometryColumnsViewName, geometryType)
		{
			SpatialDialect.LastInstantiated = this;
			worker = new MsSql2012FunctionRegistration(this, sqlTypeName, geometryColumnsViewName, geometryType);
		}
	}
}