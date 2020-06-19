using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;

namespace Foundation.ControllerFormatter
{
    public class JsonInputFormatter: TextInputFormatter
    {
        public JsonInputFormatter()
        {
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("application/json"));
            SupportedEncodings.Add(Encoding.UTF8);
            SupportedEncodings.Add(Encoding.Unicode);
        }

        public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context, Encoding encoding)
        {
            using var reader = new StreamReader(context.HttpContext.Request.Body, encoding);
            string json = "";
            try
            {
                json = await reader.ReadLineAsync();
                var data = JsonConvert.DeserializeObject(json, context.ModelType);

                return await InputFormatterResult.SuccessAsync(data);
            }
            catch (Exception ex)
            {
                return await InputFormatterResult.FailureAsync();
            }
        }
    }
}