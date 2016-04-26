using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHibernate.Spatial.Dialect
{
    public class MySQL57SpatialDialect: MySQLSpatialDialect
    {

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
