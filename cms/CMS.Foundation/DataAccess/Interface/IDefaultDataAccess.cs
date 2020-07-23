using System.Collections.Generic;
using System.Threading.Tasks;
using Foundation.Modal;
using Foundation.Modal.RequestModal;
using Foundation.Modal.Result;

namespace Foundation.DataAccess.Interface
{
    public interface IDefaultDataAccess<TModal> where TModal : class, new()
    {
        Task<IEnumerable<TModal>> GetAll();
        
        Task<TModal> GetById(int id);

        Task<TModal> GetByNum(string num);

        Task<IEnumerable<TModal>> GetByNum(string[] num);

        Task<bool> Delete(string num);

        Task<bool> Delete(string[] num);

        Task<bool> Delete(TModal t);

        Task<bool> Delete(List<TModal> t);

        Task<int> Add(TModal t);

        Task<int> Add(List<TModal> t);

        Task<bool> Update(TModal t);

        Task<bool> Update(List<TModal> t);
        
        Task<PageResponse> Page(IPageRequest req);
    }
}