/**
 * author：created by zw on 2020-06-20 14:28:18
 * email：ntzw.geek@gmail.com
 */

using DataAccess.Interface.Account;
using Model.Account;

namespace DataAccess.MySql.Account
{
    public class PermissionsDapper : DefaultDataAccess<Permissions>, IPermissionsDapper
    {
    }
}