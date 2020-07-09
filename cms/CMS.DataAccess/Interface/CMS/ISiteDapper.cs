/**
 * author：created by zw on 2020-07-08 15:54:55
 * email：ntzw.geek@gmail.com
 */

using System.Threading.Tasks;
using Foundation.DataAccess.Interface;
using Model.CMS;

namespace DataAccess.Interface.CMS
{
    public interface ISiteDapper : IDefaultDataAccess<Site>
    {
        Task<int> RemoveOtherDefault(int excludeId);
    }
}