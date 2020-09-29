/**
 * author：created by zw on 2020-06-20 14:28:20
 * email：ntzw.geek@gmail.com
 */

using System.Collections.Generic;
using System.Threading.Tasks;
using CMS.Modules.Content.Abstractions.Model;
using Foundation.DataAccess.Interface;

namespace CMS.Modules.Content.Abstractions.Interface.Dapper
{
    public interface IColumnDapper : IDefaultDataAccess<Column>
    {
        Task<IEnumerable<Column>> GetByParentNum(string parentNum);
        Task<IEnumerable<dynamic>> GetCascaderDataByParentNum(string parentNum);
        Task<IEnumerable<Column>> GetBySiteNum(string siteNum);
        
    }
}