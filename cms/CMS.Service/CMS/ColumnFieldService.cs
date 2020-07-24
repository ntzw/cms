/**
 * author：created by zw on 2020-07-10 13:51:33
 * email：ntzw.geek@gmail.com
 */

using System.Collections.Generic;
using System.Threading.Tasks;
using DataAccess.Interface.CMS;
using DataAccess.SqlServer.CMS;
using Foundation.Modal.Result;
using Model.CMS;

namespace Service.CMS
{
    public class ColumnFieldService : DefaultService<ColumnField, IColumnFieldDapper>
    {
        private ColumnFieldDapper _dapper;

        protected override IColumnFieldDapper GetDapper() => _dapper ??= new ColumnFieldDapper();

        private ColumnFieldService()
        {
        }

        private static ColumnFieldService _interface;
        public static ColumnFieldService Interface => _interface ??= new ColumnFieldService();

        public Task<IEnumerable<ColumnField>> GetByModelFieldNum(string columnNum, List<string> modelFieldNum)
        {
            return GetDapper().GetByModelFieldNum(columnNum, modelFieldNum);
        }

        public Task<IEnumerable<ColumnField>> GetByColumnNum(string columnNum)
        {
            return GetDapper().GetByColumnNum(columnNum);
        }

        public async Task<HandleResult> Clear(string[] columnNums)
        {
            var count = await GetDapper().Clear(columnNums);
            return count > 0 ? HandleResult.Success() : HandleResult.Error("");
        }
    }
}