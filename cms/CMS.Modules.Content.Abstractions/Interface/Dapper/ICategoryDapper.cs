using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;
using CMS.Modules.Content.Abstractions.Model;
using Foundation.DataAccess.Interface;

namespace CMS.Modules.Content.Abstractions.Interface.Dapper
{
    public interface ICategoryDapper: IDataAccess
    {
        Task<IEnumerable<Category>> GetByColumnNum(string columnNum, string tableName);
        Task<Category> GetById(string tableName, int id);
        Task<Category> GetByNum(string tableName, string num);
        Task<int> Execute(string sql, ExpandoObject value);
        Task<int> Delete(string tableName, string[] ids);
    }
}