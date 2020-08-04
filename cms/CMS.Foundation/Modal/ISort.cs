using System;
using CMS.Enums;

namespace Foundation.Modal
{
    public interface ISort
    {
        string ToSql(string prefix = "");
        
        ISort Add(string field, SortOrder order = SortOrder.DESC);

        void Delete(string field);
    }
}