using System;
using CMS.Enums;

namespace Foundation.Modal.RequestModal
{
    public class DefaultQuerySql : IQuerySql
    {
        private readonly string _fieldName;
        private readonly string _paramName;
        private readonly QuerySymbol _symbol;

        public DefaultQuerySql(string fieldName, QuerySymbol symbol = QuerySymbol.Equal) :
            this(fieldName, fieldName, symbol)
        {
        }

        public DefaultQuerySql(string fieldName, string paramName, QuerySymbol symbol = QuerySymbol.Equal)
        {
            _fieldName = fieldName;
            _paramName = paramName;
            _symbol = symbol;
        }

        public string ToWhereString()
        {
            return $"[{_fieldName}] {GetSymbol()} @{_paramName}";
        }

        public string FieldName => _fieldName;

        public string ParamName => _paramName;

        public QuerySymbol Symbol => _symbol;

        private string GetSymbol()
        {
            switch (_symbol)
            {
                case QuerySymbol.NotEqual:
                    return "<>";
                case QuerySymbol.Great:
                    return ">";
                case QuerySymbol.GreatEqual:
                    return ">=";
                case QuerySymbol.Little:
                    return "<";
                case QuerySymbol.LittleEqual:
                    return "<=";
                case QuerySymbol.Like:
                    return "like";
                default:
                    return "=";
            }
        }
    }
}