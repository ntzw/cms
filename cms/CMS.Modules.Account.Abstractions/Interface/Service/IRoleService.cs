using System.Collections.Generic;
using System.Threading.Tasks;
using CMS.Modules.Account.Abstractions.Model;
using CMS.React.Model;
using Foundation.Modal;
using Foundation.Modal.Result;

namespace CMS.Modules.Account.Abstractions.Interface.Service
{
    public interface IRoleService : IDefaultService<Role>
    {
        Task<PageResponse> TreeTableData();

        Task<List<CascaderDataType>> GetCascaderData();
    }
}