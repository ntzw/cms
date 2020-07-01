using System.Collections.Generic;
using Extension;

namespace Foundation.Modal.RequestModal
{
    public class SqlServerSort : ISort
    {
        private readonly List<string> _field = new List<string>();
        private readonly List<string> _order = new List<string>();

        public ISort Add(string field, string order)
        {
            if (!field.IsNotEmpty() || !order.IsNotEmpty()) return this;

            _field.Add(field);
            _order.Add(order);

            return this;
        }

        public string ToSql(string prefix = "")
        {
            List<string> temp = new List<string>();
            for (int i = 0; i < _field.Count && i < _order.Count; i++)
            {
                temp.Add($"{(prefix.IsNotEmpty() ? $"[{prefix}]." : "")}[{_field[i]}] {(_order[i].ToLower() == "desc" ? "desc" : "asc")}");
            }

            return $"ORDER BY {string.Join(",", temp)}";
        }
    }
}