using System.Threading.Tasks;
using CMS.Modules.Account.Abstractions.Model;
using Foundation.DataAccess.Interface;

namespace CMS.Modules.Account.Abstractions.Interface.Dapper
{
    public interface IAdminDapper : IDefaultDataAccess<Administrator>
    {
        Task<Administrator> GetByAccountName(string accountName);
    }
}