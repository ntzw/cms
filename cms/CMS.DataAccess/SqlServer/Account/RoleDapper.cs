/**
 * author：created by zw on 2020-06-20 14:28:18
 * email：ntzw.geek@gmail.com
 */

using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using DataAccess.Interface.Account;
using Foundation.Modal;
using Foundation.Modal.RequestModal;
using Model.Account;

namespace DataAccess.SqlServer.Account
{
    public class RoleDapper : DefaultDataAccess<Role>, IRoleDapper
    {
        
    }
}