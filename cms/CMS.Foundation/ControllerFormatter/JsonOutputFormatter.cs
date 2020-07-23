using System;
using System.Text;
using System.Threading.Tasks;
using Extension;
using Foundation.Modal;
using Foundation.Modal.Result;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;

namespace Foundation.ControllerFormatter
{
    public class JsonOutputFormatter : TextOutputFormatter
    {
        public JsonOutputFormatter()
        {
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("application/json"));

            SupportedEncodings.Add(Encoding.UTF8);
            SupportedEncodings.Add(Encoding.Unicode);
        }



        private readonly Type _resultType = typeof(HandleResult);

        protected override bool CanWriteType(Type type)
        {
            if (type.FullName != null && (type.FullName.Contains("PageResponse") || 
                                          type.FullName.Contains("ResultHelper") ||
                                          (_resultType != null && _resultType.FullName != null && type.FullName.Contains(_resultType.FullName))))
            {
                return base.CanWriteType(type);
            }

            return false;
        }

        public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
        {
            var response = context.HttpContext.Response;
            var obj = context.Object;

            var jsonSettings = new JsonSerializerSettings
            {
                ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
            };

            await response.WriteAsync(obj == null ? new HandleResult().ToJson(jsonSettings) : obj.ToJson(jsonSettings));
        }
    }
}