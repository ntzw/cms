/**
 * author：created by zw on 2020-07-10 13:51:34
 * email：ntzw.geek@gmail.com
 */

using System.Collections.Generic;
using System.Threading.Tasks;
using Foundation.DataAccess.Interface;
using Model.CMS;

namespace DataAccess.Interface.CMS
{
    public interface IColumnFieldDapper : IDefaultDataAccess<ColumnField>
    {
        Task<IEnumerable<ColumnField>> GetByModelFieldNum(string columnNum, List<string> modelFieldNum);
        Task<IEnumerable<ColumnField>> GetByColumnNum(string columnNum);
        Task<int> Clear(string[] columnNums);
    }
}