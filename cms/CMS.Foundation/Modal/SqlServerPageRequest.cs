using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
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

        public SqlServerPageRequest(JObject form)
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
                            Queries.Add(new DefaultQuery(GetValue(keyValue.Value), new DefaultQuerySql(keyValue.Key)));
                            break;
                    }
                }
            }

            if (form.ContainsKey("sort") && form["sort"] is JObject sort && sort.HasValues)
            {
                foreach (var jToken in sort)
                {
                    Sort.Add(jToken.Key, jToken.Value.ToStr());
                }
            }
            else
            {
                Sort.Add("Id", "DESC");
            }
        }

        public long Current { get; set; } = 1;

        public long Size { get; set; } = 10;

        public List<IQuery> Queries { get; set; }

        public ISort Sort { get; set; }

        public long Begin => (Current - 1) * Size + 1;

        public long End => Current * Size;

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