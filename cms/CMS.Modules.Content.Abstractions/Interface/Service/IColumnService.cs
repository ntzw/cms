using System.Collections.Generic;
using System.Threading.Tasks;
using CMS.Modules.Content.Abstractions.Model;
using CMS.React.Model;
using Foundation.Modal;
using Foundation.Modal.Result;

namespace CMS.Modules.Content.Abstractions.Interface.Service
{
    public interface IColumnService : IDefaultService<Column>
    {
        Task<List<CascaderDataType>> GetCascaderData(string siteNum);

        Task<PageResponse> GetTreeTableData(string siteNum);

        Task<IEnumerable<Column>> GetBySite(string siteNum);

        Task<IEnumerable<Column>> GetByParent(string parentNum);
        Task<ColumnModel> GetModelByNum(string columnNum);
    }
}