using System;
using CMS.Enums;

namespace Foundation.Modal.RequestModal
{
    public class DefaultQuery : IQuery
    {
        public DefaultQuery(object value, IQuerySql querySql)
        {
            switch (querySql.Symbol)
            {
                case QuerySymbol.Like:
                    Value = $"%{value}%";
                    break;
                default:
                    Value = value;
                    break;
            }
            
           
            QuerySql = querySql;
        }

        public object Value { get; }

        public IQuerySql QuerySql { get; }
    }
}