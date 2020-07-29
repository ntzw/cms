using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;
using Foundation.Modal.Result;
using Model.CMS;

namespace DataAccess.Interface.CMS
{
    public interface ICategoryDapper
    {
        Task<IEnumerable<Category>> GetByColumnNum(string columnNum, string tableName);
        Task<Category> GetById(string tableName, int id);
        Task<Category> GetByNum(string tableName, string num);
        Task<int> Execute(string sql, ExpandoObject value);
        Task<int> Delete(string tableName, string[] ids);
    }
}