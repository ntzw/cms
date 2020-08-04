using System;
using System.Collections.Generic;
using Extension;
using Foundation.Modal.RequestModal;

namespace Foundation.Modal
{
    public class SqlServerSelectRequest : ISelectRequest
    {
        public int Top { get; set; }
        public List<IQuery> Queries { get; set; }
        public ISort Sort { get; set; }

        public bool ContainsQueryField(string field)
        {
            if (field.IsEmpty()) return false;
            return GetQueryField(field) != null;
        }

        public IQuery GetQueryField(string filed)
        {
            if (filed.IsEmpty()) return null;
            return Queries.Find(temp =>
                string.Equals(temp.QuerySql.FieldName, filed, StringComparison.OrdinalIgnoreCase));
        }
    }
}