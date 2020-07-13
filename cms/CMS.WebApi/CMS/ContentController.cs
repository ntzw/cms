using System.Threading.Tasks;
using Extension;
using Foundation.Modal;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Service.CMS;

namespace WebApi.CMS
{
    [Route("Api/CMS/[controller]/[action]")]
    public class ContentController : ControllerBase
    {
        public async Task<HandleResult> GetEdit([FromBody] JObject form)
        {
            string itemNum = form["itemNum"].ToStr();
            string columnNum = form["columnNum"].ToStr();
            
            if (itemNum.IsNotEmpty() && columnNum.IsEmpty())
            {
                //todo 根据内容编号，获取栏目编号
            }
            
            if (itemNum.IsEmpty() && columnNum.IsEmpty()) return HandleResult.Error("参数有误");
            
            
            return new HandleResult
            {
                IsSuccess = true,
                Data = new
                {
                    editValue = new {}, //todo 需要根据内容编号获取内容数据
                    fields = await ColumnFieldService.Interface.GetByColumnNum(columnNum)
                }
            };
        }
    }
}