/**
 * author：created by zw on 2020-07-08 15:54:55
 * email：ntzw.geek@gmail.com
 */

using System.Threading.Tasks;
using CMS.Modules.Content.Abstractions.Model;
using Foundation.DataAccess.Interface;

namespace CMS.Modules.Content.Abstractions.Interface.Dapper
{
    public interface IModelTableDapper : IDefaultDataAccess<ModelTable>
    {
        Task<int> CreateTable(string tableName);

        Task<int> CreateCategoryTable(string tableName);

        Task<ModelTable> GetByTableName(string tableName);
    }
}