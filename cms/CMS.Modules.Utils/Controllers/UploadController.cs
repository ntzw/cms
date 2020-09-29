using System.IO;
using System.Threading.Tasks;
using Extension;
using Foundation.Modal.Result;
using Foundation.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CMS.Modules.Utils.Controllers
{
    [Route("Api/Utils/[controller]/[action]")]
    public class UploadController : ControllerBase
    {
        private readonly UploadImageUtil _uploadImageUtil = new UploadImageUtil();
        private readonly UploadFileUtil _uploadFileUtil = new UploadFileUtil();

        public Task<HandleResult> Image(IFormFile file, IFormCollection form)
        {
            return _uploadImageUtil.Upload(file, form);
        }

        public Task<HandleResult> File(IFormFile file, IFormCollection form)
        {
            return _uploadFileUtil.Upload(file, form);
        }

        public async Task<HandleResult> BraftEditor(IFormFile file, IFormCollection form)
        {
            if (file.FileName.IsEmpty()) return HandleResult.Error("无效数据");

            string ext = Path.GetExtension(file.FileName);

            return ext.IsImage() ? await _uploadImageUtil.Upload(file, form) : await _uploadFileUtil.Upload(file, form);
        }
    }
}