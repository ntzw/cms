using System.Threading.Tasks;
using CMS.Modules.Utils.Abstractions.Interface.Service;
using Extension;
using Foundation.Modal.Result;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace CMS.Modules.Utils.Controllers
{
    [Route("api/Utils/[controller]/[action]")]
    public class CodeGenerateController : ControllerBase
    {
        private readonly ICodeGenerateService _service;

        public CodeGenerateController(ICodeGenerateService service)
        {
            _service = service;
        }
        
        public async Task<HandleResult> GetAllTable()
        {
            return new HandleResult
            {
                IsSuccess = true,
                Data = await _service.GetAllTableName()
            };
        }

        public async Task<HandleResult> GetTableFields([FromBody]JObject info)
        {
            return new HandleResult
            {
                IsSuccess = true,
                Data = await _service.GetTableFields(info["name"].ToStr())
            };
        }
    }
}