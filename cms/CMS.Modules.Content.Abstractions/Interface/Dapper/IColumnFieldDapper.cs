/**
 * author：created by zw on 2020-07-10 13:51:34
 * email：ntzw.geek@gmail.com
 */

using System.Collections.Generic;
using System.Threading.Tasks;
using CMS.Modules.Content.Abstractions.Model;
using Foundation.DataAccess.Interface;

namespace CMS.Modules.Content.Abstractions.Interface.Dapper
{
    public interface IColumnFieldDapper : IDefaultDataAccess<ColumnField>
    {
        Task<IEnumerable<ColumnField>> GetByModelFieldNum(string columnNum, List<string> modelFieldNum);
        Task<IEnumerable<ColumnField>> GetByColumnNum(string columnNum);
        Task<int> Clear(string[] columnNums);
    }
}