using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CMS.React;
using Extension;
using Foundation.Modal;
using Microsoft.AspNetCore.Mvc;
using Model.CMS;
using Newtonsoft.Json.Linq;
using Service.CMS;

namespace WebApi.CMS
{
    [Route("Api/CMS/[controller]/[action]")]
    public class ModelController : ControllerBase
    {
        #region 模型表

        public async Task<PageResponse> Page([FromBody] JObject form)
        {
            var req = new SqlServerPageRequest(form);
            return await ModelTableService.Interface.Page(req);
        }

        public async Task<HandleResult> FormFields([FromBody] JObject form)
        {
            int id = form["id"].ToInt();
            var data = id > 0 ? await ModelTableService.Interface.GetById(id) : new ModelTable();
            if (data == null) return HandleResult.Error("无效数据");

            return new HandleResult
            {
                IsSuccess = true,
                Data = new
                {
                    EditData = data,
                    Field = ReactForm.ToFormFields<ModelTable>(data.Id > 0)
                }
            };
        }

        public async Task<HandleResult> Edit([FromBody] ModelTable model)
        {
            var info = model.Id > 0 ? await ModelTableService.Interface.GetById(model.Id) : new ModelTable();
            if (info == null) return HandleResult.Error("无效数据");

            bool isUpdate = info.Id > 0;

            if (!isUpdate)
            {
                var verify = await ModelTableService.Interface.GetByTableName(model.TableName);
                if (verify != null && verify.Id != info.Id) return HandleResult.Error("表名已存在");
            }

            info.Init();
            ReactForm.SetEditModelValue(info, model, isUpdate);

            var res = isUpdate
                ? await ModelTableService.Interface.Update(info)
                : await ModelTableService.Interface.Add(info);

            if (res.IsSuccess && !isUpdate)
                ModelTableService.Interface.CreateTable(info);

            return res;
        }

        public async Task<HandleResult> Delete([FromBody] JArray form)
        {
            if (form == null || form.Count <= 0) return HandleResult.Error("请选择要删除的数据");

            //todo 模型删除需要删除对应的实体表，不允许轻易删除，需要判断有没有栏目再使用该模型

            var deleteModels = form.Select(temp => new ModelTable {Id = temp.ToInt()}).ToList();
            return await ModelTableService.Interface.Delete(deleteModels);
        }

        public async Task<HandleResult> SelectData([FromBody] JObject form)
        {
            return new HandleResult
            {
                IsSuccess = true,
                Data = await ModelTableService.Interface.GetSelectData()
            };
        }

        #endregion

        public async Task<PageResponse> FieldPage([FromBody] JObject form)
        {
            var req = new SqlServerPageRequest(form);
            if (!req.ContainsQueryField("ModelNum")) return PageResponse.Error("请选择模型");

            return await ModelFieldService.Interface.Page(req);
        }

        public async Task<HandleResult> FieldEdit([FromBody] ModelField model)
        {
            if (model.Id <= 0)
            {
                if (model.ModelNum.IsEmpty()) return HandleResult.Error("请选择模型");
                if (model.Name.IsEmpty()) return HandleResult.Error("请填写名称");
            }

            var info = model.Id > 0 ? await ModelFieldService.Interface.GetById(model.Id) : new ModelField();
            if (info == null) return HandleResult.Error("无效数据");

            info.Init();
            if (info.Id <= 0)
            {
                info.ModelNum = model.ModelNum;
                info.Name = model.Name;
                info.OptionType = model.OptionType;
            }
            
            info.Explain = model.Explain;
            info.Options = model.Options;

            var res = info.Id > 0
                ? await ModelFieldService.Interface.Update(info)
                : await ModelFieldService.Interface.Add(info);

            //todo 需要添加字段

            return res;
        }

        public async Task<HandleResult> GetFieldEditValue([FromBody] JObject form)
        {
            int id = form["id"].ToInt();
            if (id <= 0) return HandleResult.Error("无效参数");

            var info = await ModelFieldService.Interface.GetById(id);
            if (info == null) return HandleResult.Error("无效参数");

            return new HandleResult
            {
                IsSuccess = true,
                Data = info,
            };
        }
    }
}