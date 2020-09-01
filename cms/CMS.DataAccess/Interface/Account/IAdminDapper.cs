using System.Threading.Tasks;
using Foundation.DataAccess.Interface;
using Foundation.Modal;
using Model.Account;

namespace DataAccess.Interface.Account
{
    public interface IAdminDapper : IDefaultDataAccess<Administrator>
    {
        Task<Administrator> GetByAccountName(string accountName);
    }
}