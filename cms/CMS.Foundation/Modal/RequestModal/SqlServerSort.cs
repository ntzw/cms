using System;
using System.Collections.Generic;
using CMS.Enums;
using Extension;

namespace Foundation.Modal.RequestModal
{
    public class SqlServerSort : ISort
    {
        private readonly List<string> _field = new List<string>();
        private readonly List<SortOrder> _order = new List<SortOrder>();

        public ISort Add(string field, SortOrder order = SortOrder.DESC)
        {
            if (!field.IsNotEmpty()) return this;

            _field.Add(field);
            _order.Add(order);

            return this;
        }

        public void Delete(string field)
        {
            int index = _field.FindIndex(temp => string.Equals(temp, field, StringComparison.OrdinalIgnoreCase));
            if (index > -1)
            {
                _field.RemoveAt(index);
                _order.RemoveAt(index);
            }
        }

        public string ToSql(string prefix = "")
        {
            List<string> temp = new List<string>();
            for (int i = 0; i < _field.Count && i < _order.Count; i++)
            {
                temp.Add($"{(prefix.IsNotEmpty() ? $"[{prefix}]." : "")}[{_field[i]}] {_order[i].ToString()}");
            }

            return _field.Count > 0 ? $"ORDER BY {string.Join(",", temp)}" : "";
        }
    }
}