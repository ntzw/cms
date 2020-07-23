using System;
using System.Text;
using System.Threading.Tasks;
using Extension;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;

namespace Foundation.Modal
{
    public class ResultBaseExecutor : IActionResultExecutor<ResultBase>
    {
        private static readonly string DefaultContentType = new MediaTypeHeaderValue("application/json")
        {
            Encoding = Encoding.UTF8
        }.ToString();

        private readonly IHttpResponseStreamWriterFactory _httpResponseStreamWriterFactory;

        public ResultBaseExecutor(IHttpResponseStreamWriterFactory httpResponseStreamWriterFactory)
        {
            _httpResponseStreamWriterFactory = httpResponseStreamWriterFactory;
        }

        public async Task ExecuteAsync(ActionContext context, ResultBase result)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            var response = context.HttpContext.Response;
            ResolveContentTypeAndEncoding(
                null,
                response.ContentType,
                DefaultContentType,
                out var resolvedContentType,
                out var resolvedContentTypeEncoding);
            response.ContentType = resolvedContentType;
            var jsonSettings = new JsonSerializerSettings
            {
                ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
            };

            var content = result.ToJson(jsonSettings);
            response.ContentLength = resolvedContentTypeEncoding.GetByteCount(content);
            await using var textWriter =
                _httpResponseStreamWriterFactory.CreateWriter(response.Body, resolvedContentTypeEncoding);
            await textWriter.WriteAsync(content);
            await textWriter.FlushAsync();
        }

        public static void ResolveContentTypeAndEncoding(
            string actionResultContentType,
            string httpResponseContentType,
            string defaultContentType,
            out string resolvedContentType,
            out Encoding resolvedContentTypeEncoding)
        {
           

            var defaultContentTypeEncoding = MediaType.GetEncoding(defaultContentType);
           

            // 1. User sets the ContentType property on the action result
            if (actionResultContentType != null)
            {
                resolvedContentType = actionResultContentType;
                var actionResultEncoding = MediaType.GetEncoding(actionResultContentType);
                resolvedContentTypeEncoding = actionResultEncoding ?? defaultContentTypeEncoding;
                return;
            }

            // 2. User sets the ContentType property on the http response directly
            if (!string.IsNullOrEmpty(httpResponseContentType))
            {
                var mediaTypeEncoding = MediaType.GetEncoding(httpResponseContentType);
                if (mediaTypeEncoding != null)
                {
                    resolvedContentType = httpResponseContentType;
                    resolvedContentTypeEncoding = mediaTypeEncoding;
                }
                else
                {
                    resolvedContentType = httpResponseContentType;
                    resolvedContentTypeEncoding = defaultContentTypeEncoding;
                }

                return;
            }

            // 3. Fall-back to the default content type
            resolvedContentType = defaultContentType;
            resolvedContentTypeEncoding = defaultContentTypeEncoding;
        }
    }
}