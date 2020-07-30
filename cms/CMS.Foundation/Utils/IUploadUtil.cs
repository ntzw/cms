using System.Threading.Tasks;
using Foundation.Modal.Result;
using Microsoft.AspNetCore.Http;

namespace Foundation.Utils
{
    public interface IUploadUtil
    {
        Task<HandleResult> Upload(IFormFile file, IFormCollection form);
    }
}