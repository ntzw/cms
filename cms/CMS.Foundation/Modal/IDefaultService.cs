using System.Collections.Generic;
using System.Threading.Tasks;
using Foundation.Modal.RequestModal;
using Foundation.Modal.Result;

namespace Foundation.Modal
{
    public interface IDefaultService<TModal>
        where TModal : class, new()
    {
        Task<PageResponse> Page(IPageRequest req);

        Task<TModal> GetById(int id);

        Task<TModal> GetByNum(string num);

        Task<IEnumerable<TModal>> GetByNum(string[] num);

        Task<HandleResult> Add(TModal modal);

        Task<HandleResult> Add(List<TModal> modal);

        Task<HandleResult> Update(TModal modal);

        Task<HandleResult> Update(List<TModal> modal);

        Task<HandleResult> Delete(TModal modal);

        Task<HandleResult> Delete(List<TModal> modals);

        Task<IEnumerable<TModal>> GetAll();
    }
}