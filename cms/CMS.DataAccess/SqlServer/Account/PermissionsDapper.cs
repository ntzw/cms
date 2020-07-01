/**
 * author：created by zw on 2020-06-20 14:28:18
 * email：ntzw.geek@gmail.com
 */

using System.Threading.Tasks;
using DataAccess.Interface.Account;
using Foundation.Modal;
using Foundation.Modal.RequestModal;
using Model.Account;

namespace DataAccess.SqlServer.Account
{
    public class PermissionsDapper : DefaultDataAccess<Permissions>, IPermissionsDapper
    {
        public Task<PageResponse> Page(IPageRequest req)
        {
            throw new System.NotImplementedException();
        }
    }
}