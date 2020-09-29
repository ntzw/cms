using System.Collections.Generic;
using System.Threading.Tasks;
using CMS.Modules.Content.Abstractions.Model;
using CMS.React.Model;
using Foundation.Modal.Result;
using Newtonsoft.Json.Linq;

namespace CMS.Modules.Content.Abstractions.Interface.Service
{
    public interface ICategoryService
    {
        Task<PageResponse> GetTreeTableData(string columnNum);

        Task<Category> GetById(string columnNum, int id);

        Task<List<CascaderDataType>> GetCascaderData(string columnNum);

        Task<HandleResult> Edit(JObject form, string accountNum);

        Task<HandleResult> Delete(string columnNum, string ids);

        Task<IEnumerable<Category>> GetByColumnNum(string tableName, string columnNum);
    }
}