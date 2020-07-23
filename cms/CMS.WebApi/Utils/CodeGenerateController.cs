using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Extension;
using Foundation.Modal;
using Foundation.Modal.Result;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Service.Utils;

namespace WebApi.Utils
{
    [Route("api/Utils/[controller]/[action]")]
    public class CodeGenerateController : Controller
    {
        public async Task<HandleResult> GetAllTable()
        {
            return new HandleResult
            {
                IsSuccess = true,
                Data = await CodeGenerateService.Interface.GetAllTableName()
            };
        }

        public async Task<HandleResult> GetTableFields([FromBody]JObject info)
        {
            return new HandleResult
            {
                IsSuccess = true,
                Data = await CodeGenerateService.Interface.GetTableFields(info["name"].ToStr())
            };
        }
    }
}