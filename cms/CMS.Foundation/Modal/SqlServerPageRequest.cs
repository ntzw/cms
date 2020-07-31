using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using CMS.Enums;
using Extension;
using Foundation.Modal.RequestModal;
using Newtonsoft.Json.Linq;

namespace Foundation.Modal
{
    public class SqlServerPageRequest : IPageRequest
    {
        public SqlServerPageRequest()
        {
            Sort = new SqlServerSort().Add("Id", "DESC");
            Queries = new List<IQuery>();
        }

        public SqlServerPageRequest(JObject form, Dictionary<string, string> defaultSort = null, string[] ignore = null)
        {
            Queries = new List<IQuery>();
            Sort = new SqlServerSort();
            if (form.ContainsKey("params") && form["params"] is JObject param)
            {
                foreach (var keyValue in param)
                {
                    switch (keyValue.Key.ToLower())
                    {
                        case "current":
                            this.Current = keyValue.Value.ToInt();
                            break;
                        case "pagesize":
                            this.Size = keyValue.Value.ToInt();
                            break;
                        default:
                            if (keyValue.Key.IsSqlField() && (ignore == null || !ignore.Contains(keyValue.Key)))
                                Queries.Add(new DefaultQuery(GetValue(keyValue.Value),
                                    new DefaultQuerySql(keyValue.Key, GetQuerySymbol(form, keyValue.Key))));
                            break;
                    }
                }
            }

            if (defaultSort != null)
            {
                foreach (var keyValue in defaultSort)
                {
                    Sort.Add(keyValue.Key, keyValue.Value);
                }
            }

            if (form.ContainsKey("sort") && form["sort"] is JObject sort && sort.HasValues)
            {
                foreach (var jToken in sort)
                {
                    if (jToken.Key.IsSqlField())
                        Sort.Add(jToken.Key, jToken.Value.ToStr());
                }
            }
            else if (defaultSort == null)
            {
                Sort.Add("Id", "DESC");
            }
        }

        private QuerySymbol GetQuerySymbol(JObject form, string key)
        {
            if (!form.ContainsKey("query")) return QuerySymbol.Equal;
            if (!(form["query"] is JObject querySymbol)) return QuerySymbol.Equal;
            if (!querySymbol.ContainsKey(key)) return QuerySymbol.Equal;

            int symbolIndex = querySymbol[key].ToInt();
            if (!Enum.TryParse(symbolIndex.ToString(), out QuerySymbol symbol)) return QuerySymbol.Equal;
            return symbol;
        }

        public int Current { get; set; } = 1;

        public int Size { get; set; } = 10;

        public List<IQuery> Queries { get; set; }

        public ISort Sort { get; set; }

        public long Begin => (Current - 1) * Size + 1;

        public long End => Current * Size;

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

        private object GetValue(JToken token)
        {
            switch (token.Type)
            {
                case JTokenType.Array:
                    return token.Select(item => item.ToStr());
                case JTokenType.Boolean:
                    return token.ToBoolean() ? "1" : "0";
                default:
                    return token.ToStr();
            }
        }
    }
}