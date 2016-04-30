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

namespace NHibernate.Spatial.Dialect
{
    /// <summary>
    /// MySQL spatial dialect that supports the changes introduced in MySQL 5.7
    /// </summary>

    public class MySQL57SpatialDialect: MySQLSpatialDialect
    {
        protected new static readonly IType geometryType = new CustomType(typeof(MySQL57GeometryType), null);

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
    }
}
