/**
 * author：created by zw on 2020-07-08 15:54:54
 * email：ntzw.geek@gmail.com
 */

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CMS.Modules.Content.Abstractions.Interface.Dapper;
using CMS.Modules.Content.Abstractions.Interface.Service;
using CMS.Modules.Content.Abstractions.Model;
using CMS.React.Model;
using Foundation.DataAccess;

namespace CMS.Modules.Content.Service
{
    public class ModelTableService : DefaultService<ModelTable, IModelTableDapper>, IModelTableService
    {
        private readonly IModelTableDapper _dapper;

        public ModelTableService(IModelTableDapper dapper)
        {
            _dapper = dapper;
        }

        public void CreateTable(ModelTable info)
        {
            Task.Run(async () =>
            {
                var tableName = $"CMS_U_{info.TableName}";
                await GetDapper().CreateTable(tableName);
                await GetDapper().CreateCategoryTable($"{tableName}_Category");
            });
        }

        public Task<ModelTable> GetByTableName(string tableName)
        {
            return GetDapper().GetByTableName(tableName);
        }

        public async Task<List<SelectDataType>> GetSelectData()
        {
            var all = await GetDapper().GetAll();
            return all.Select(temp => new SelectDataType
            {
                Label = temp.Explain,
                Value = temp.Num
            }).ToList();
        }

        protected override IModelTableDapper GetDapper()
        {
            return _dapper;
        }
    }
}