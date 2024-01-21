﻿// Copyright 2008 - Ricardo Stuven (rstuven@gmail.com)
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
using NHibernate.Spatial.Metadata;
using NHibernate.Spatial.Type;
using NHibernate.SqlCommand;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Spatial.Dialect
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class MsSql2012SpatialDialect : MsSql2012Dialect, ISpatialDialect, IRegisterationAdaptor
    {
        private readonly ISpatialDialect worker;

        /// <summary>
        /// Initializes a new instance of the <see cref="MsSql2012SpatialDialect"/> class.
        /// </summary>
        protected MsSql2012SpatialDialect(string sqlTypeName, string geometryColumnsViewName, IType geometryType)
        {
            SpatialDialect.LastInstantiated = this;
            worker = new MsSql2012FunctionRegistration(this, sqlTypeName, geometryColumnsViewName, geometryType);
        }

        #region ISpatialDialect Members

        public virtual IType GeometryType => worker.GeometryType;

        public abstract IGeometryUserType CreateGeometryUserType();

        public SqlString GetSpatialTransformString(object geometry, int srid)
        {
            return worker.GetSpatialTransformString(geometry, srid);
        }

        public SqlString GetSpatialAggregateString(object geometry, SpatialAggregate aggregate)
        {
            return worker.GetSpatialAggregateString(geometry, aggregate);
        }

        public SqlString GetSpatialAnalysisString(object geometry, SpatialAnalysis analysis, object extraArgument)
        {
            return worker.GetSpatialAnalysisString(geometry, analysis, extraArgument);
        }

        public SqlString GetSpatialValidationString(object geometry, SpatialValidation validation, bool criterion)
        {
            return worker.GetSpatialValidationString(geometry, validation, criterion);
        }

        public SqlString GetSpatialRelateString(object geometry, object anotherGeometry, object pattern, bool isStringPattern, bool criterion)
        {
            return worker.GetSpatialRelateString(geometry, anotherGeometry, pattern, isStringPattern, criterion);
        }

        public SqlString GetSpatialRelationString(object geometry, SpatialRelation relation, object anotherGeometry, bool criterion)
        {
            return worker.GetSpatialRelationString(geometry, relation, anotherGeometry, criterion);
        }

        public SqlString GetSpatialRelationString(object geometry, SpatialRelation relation, object anotherGeometry, object parameter, bool criterion)
        {
            return worker.GetSpatialRelationString(geometry, relation, anotherGeometry, parameter, criterion);
        }

        public SqlString GetSpatialFilterString(string tableAlias, string geometryColumnName, string primaryKeyColumnName, string tableName, Parameter parameter)
        {
            return worker.GetSpatialFilterString(tableAlias, geometryColumnName, primaryKeyColumnName, tableName, parameter);
        }

        public string GetSpatialCreateString(string schema)
        {
            return worker.GetSpatialCreateString(schema);
        }

        public string GetSpatialCreateString(string schema, string table, string column, int srid, string subtype, int dimension, bool isNullable)
        {
            return worker.GetSpatialCreateString(schema, table, column, srid, subtype, dimension, isNullable);
        }

        public string GetSpatialDropString(string schema)
        {
            return worker.GetSpatialDropString(schema);
        }

        public string GetSpatialDropString(string schema, string table, string column)
        {
            return worker.GetSpatialDropString(schema, table, column);
        }

        public bool SupportsSpatialMetadata(MetadataClass metadataClass)
        {
            return worker.SupportsSpatialMetadata(metadataClass);
        }

        // TODO: Use ISessionFactory.ConnectionProvider.Driver.MultipleQueriesSeparator
        public string MultipleQueriesSeparator => ";";

        #endregion ISpatialDialect Members

        #region IRegisterationAdaptor Members

        public new void RegisterFunction(string name, NHibernate.Dialect.Function.ISQLFunction function)
        {
            base.RegisterFunction(name, function);
        }

        public new string Quote(string name)
        {
            return base.Quote(name);
        }

        /// <summary>
        /// Quotes the schema.
        /// </summary>
        /// <param name="schema">The schema.</param>
        /// <returns></returns>
        public string QuoteSchema(string schema)
        {
            if (string.IsNullOrEmpty(schema))
            {
                return null;
            }
            return QuoteForSchemaName(schema) + StringHelper.Dot;
        }

        #endregion IRegisterationAdaptor Members
    }
}
