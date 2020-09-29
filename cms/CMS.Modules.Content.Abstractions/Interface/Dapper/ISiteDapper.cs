/**
 * author：created by zw on 2020-07-08 15:54:55
 * email：ntzw.geek@gmail.com
 */

using System.Threading.Tasks;
using CMS.Modules.Content.Abstractions.Model;
using Foundation.DataAccess.Interface;

namespace CMS.Modules.Content.Abstractions.Interface.Dapper
{
    public interface ISiteDapper : IDefaultDataAccess<Site>
    {
        Task<int> RemoveOtherDefault(int excludeId);
        Task<Site> GetByHost(string host);
        Task<Site> GetByDefault();
    }
}