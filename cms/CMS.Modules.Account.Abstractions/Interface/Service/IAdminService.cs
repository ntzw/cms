using System.Threading.Tasks;
using CMS.Modules.Account.Abstractions.Model;
using Foundation.Modal;

namespace CMS.Modules.Account.Abstractions.Interface.Service
{
    public interface IAdminService : IDefaultService<Administrator>
    {
        public string PasswordToMd5(string password);

        public Task<Administrator> GetByAccountName(string accountName);

        Task<Administrator> GetByLoginAdmin();
    }
}