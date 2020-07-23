/**
 * author：created by zw on 2020-07-08 15:54:54
 * email：ntzw.geek@gmail.com
 */

using System.Threading.Tasks;
using CMS.Enums;
using DataAccess.Interface.CMS;
using DataAccess.SqlServer.CMS;
using Extension;
using Foundation.Modal;
using Foundation.Modal.Result;
using Model.CMS;

namespace Service.CMS
{
    public class ModelFieldService : DefaultService<ModelField, IModelFieldDapper>
    {
        private ModelFieldDapper _dapper;

        protected override IModelFieldDapper GetDapper() => _dapper ??= new ModelFieldDapper();

        private ModelFieldService()
        {
        }

        private static ModelFieldService _interface;
        public static ModelFieldService Interface => _interface ??= new ModelFieldService();

        public Task<ModelField> GetByName(string fieldName, string modelNum)
        {
            return GetDapper().GetByName(fieldName, modelNum);
        }

        public async Task<HandleResult> CreateField(ModelField info)
        {
            var model = await ModelTableService.Interface.GetByNum(info.ModelNum);
            if (model == null) return HandleResult.Error("模型不存在");

            string tableName = $"CMS_U_{model.TableName}";

            int count = 0;
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
    }
}