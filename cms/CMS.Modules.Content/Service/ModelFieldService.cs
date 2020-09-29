/**
 * author：created by zw on 2020-07-08 15:54:54
 * email：ntzw.geek@gmail.com
 */

using System.Threading.Tasks;
using CMS.Enums;
using CMS.Modules.Content.Abstractions.Interface.Dapper;
using CMS.Modules.Content.Abstractions.Interface.Service;
using CMS.Modules.Content.Abstractions.Model;
using Foundation.DataAccess;
using Foundation.Modal.Result;

namespace CMS.Modules.Content.Service
{
    public class ModelFieldService : DefaultService<ModelField, IModelFieldDapper>, IModelFieldService
    {
        private readonly IModelFieldDapper _dapper;
        private readonly IModelTableService _modelTableService;

        public ModelFieldService(IModelFieldDapper dapper, IModelTableService modelTableService)
        {
            _dapper = dapper;
            _modelTableService = modelTableService;
        }

        public Task<ModelField> GetByName(string fieldName, string modelNum)
        {
            return GetDapper().GetByName(fieldName, modelNum);
        }

        public async Task<HandleResult> CreateField(ModelField info)
        {
            var model = await _modelTableService.GetByNum(info.ModelNum);
            if (model == null) return HandleResult.Error("模型不存在");

            var tableName = $"CMS_U_{model.TableName}";

            var count = 0;
            switch (info.OptionType)
            {
                case ReactFormItemType.RangePicker:
                    count += await GetDapper().CreateField($"{info.Name}Start", tableName, info.OptionType);
                    count += await GetDapper().CreateField($"{info.Name}End", tableName, info.OptionType);
                    break;
                default:
                    count = await GetDapper().CreateField(info.Name, tableName, info.OptionType);
                    break;
            }

            return new HandleResult
            {
                IsSuccess = count > 0
            };
        }

        protected override IModelFieldDapper GetDapper()
        {
            return _dapper;
        }
    }
}