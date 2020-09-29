/**
 * author：created by zw on 2020-07-10 13:51:33
 * email：ntzw.geek@gmail.com
 */

using System.Collections.Generic;
using System.Threading.Tasks;
using CMS.Modules.Content.Abstractions.Interface.Dapper;
using CMS.Modules.Content.Abstractions.Interface.Service;
using CMS.Modules.Content.Abstractions.Model;
using Foundation.DataAccess;
using Foundation.Modal.Result;

namespace CMS.Modules.Content.Service
{
    public class ColumnFieldService : DefaultService<ColumnField, IColumnFieldDapper>, IColumnFieldService
    {
        private readonly IColumnFieldDapper _dapper;

        public ColumnFieldService(IColumnFieldDapper dapper)
        {
            _dapper = dapper;
        }

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

        protected override IColumnFieldDapper GetDapper()
        {
            return _dapper;
        }
    }
}