using System.Collections.Generic;
using System.Threading.Tasks;
using Foundation.DataAccess.Interface;
using Foundation.Modal;
using Foundation.Modal.RequestModal;
using Model.Account;

namespace Service
{
    public abstract class DefaultService<TModal, TDapper>
        where TModal : class, new()
        where TDapper : IDefaultDataAccess<TModal>
    {
        protected abstract TDapper GetDapper();

        public Task<PageResponse> Page(IPageRequest req)
        {
            return GetDapper().Page(req);
        }
        
        public Task<TModal> GetById(int id)
        {
            return GetDapper().GetById(id);
        }

        public Task<TModal> GetByNum(string num)
        {
            return GetDapper().GetByNum(num);
        }

        public Task<IEnumerable<TModal>> GetByNum(string[] num)
        {
            return GetDapper().GetByNum(num);
        }

        public async Task<HandleResult> Add(TModal modal)
        {
            var id = await GetDapper().Add(modal);
            if (id > 0)
            {
               var property = modal.GetType().GetProperty("Id");
               if (property != null)
                   property.SetValue(modal, id);
            }

            return HandleResult.Success();
        }

        public async Task<HandleResult> Add(List<TModal> modal)
        {
            var count = await GetDapper().Add(modal);
            return count > 0 ? HandleResult.Success() : HandleResult.Error("");
        }

        public async Task<HandleResult> Update(TModal modal)
        {
            return new HandleResult
            {
                IsSuccess = await GetDapper().Update(modal)
            };
        }

        public async Task<HandleResult> Update(List<TModal> modal)
        {
            return new HandleResult
            {
                IsSuccess = await GetDapper().Update(modal)
            };
        }

        public async Task<HandleResult> Delete(TModal modal)
        {
            return new HandleResult
            {
                IsSuccess = await GetDapper().Delete(modal)
            };
        }

        public async Task<HandleResult> Delete(List<TModal> modals)
        {
            return new HandleResult
            {
                IsSuccess = await GetDapper().Delete(modals)
            };
        }

        public Task<IEnumerable<TModal>> GetAll()
        {
            return GetDapper().GetAll();
        }
    }
}