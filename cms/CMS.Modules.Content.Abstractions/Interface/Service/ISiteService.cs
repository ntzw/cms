using System.Threading.Tasks;
using CMS.Modules.Content.Abstractions.Model;
using Foundation.Modal;
using Foundation.Modal.Result;

namespace CMS.Modules.Content.Abstractions.Interface.Service
{
    public interface ISiteService: IDefaultService<Site>
    {
        Site GetCurrentSite();

        Task<HandleResult> RemoveOtherDefault(int excludeId);

        Task<HandleResult> GetSelectData();

        Task<Site> GetByHost(string host);

        Task<Site> GetByDefault();
    }
}