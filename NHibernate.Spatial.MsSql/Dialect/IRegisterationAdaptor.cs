using NHibernate.Dialect.Function;
using NHibernate.Type;

namespace NHibernate.Spatial.Dialect
{
    internal interface IRegisterationAdaptor 
    {
      void RegisterFunction(string name, ISQLFunction function);

      string QuoteForTableName(string table);

      string QuoteForColumnName(string column);

      string QuoteForSchemaName(string schema);

      string Quote(string name);

      string QuoteSchema(string schema);
    }
}