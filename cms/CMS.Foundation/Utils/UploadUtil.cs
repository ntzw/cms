using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Extension;
using Foundation.Modal.Result;
using Helper;
using Microsoft.AspNetCore.Http;

namespace Foundation.Utils
{
    public abstract class UploadUtil
    {
        /// <summary>
        /// 是否在白名单
        /// </summary>
        /// <param name="ext"></param>
        /// <returns></returns>
        protected abstract bool ExistWhite(string ext);

        /// <summary>
        /// 获取新文件名
        /// </summary>
        /// <param name="oldFileName"></param>
        /// <param name="ext"></param>
        /// <returns></returns>
        protected virtual string GetNewFileName(string oldFileName, string ext)
        {
            return Path.GetFileNameWithoutExtension(oldFileName) + "_" + RandomHelper.CreateNum() + ext;
        }

        /// <summary>
        /// 获取保存的文件夹路径
        /// </summary>
        /// <returns></returns>
        protected virtual string GetSaveWebFolderPath()
        {
            return $"/upload/{DateTime.Now:yyyyMMdd}/";
        }

        /// <summary>
        /// 是否重命名
        /// </summary>
        /// <param name="savePath">保存地址</param>
        /// <returns></returns>
        protected virtual bool IsRename(string savePath)
        {
            if (ConfigHelper.GetAppSetting("uploadconfig:rename").ToBoolean()) return true;
            if (!IsCover()) return File.Exists(savePath);

            return false;
        }

        /// <summary>
        /// 是否允许覆盖
        /// </summary>
        /// <returns></returns>
        protected virtual bool IsCover()
        {
            return ConfigHelper.GetAppSetting("uploadconfig:iscover").ToBoolean();
        }

        /// <summary>
        /// 获取文件保存地址
        /// </summary>
        /// <param name="saveFolderPath"></param>
        /// <param name="oldFileName"></param>
        /// <param name="fileExt"></param>
        /// <returns></returns>
        protected virtual string GetSavePath(string saveFolderPath, string oldFileName, string fileExt)
        {
            string oldSavePath = Path.Combine(saveFolderPath, oldFileName);
            if (!IsRename(oldSavePath)) return oldSavePath;

            return Path.Combine(saveFolderPath, GetNewFileName(oldFileName, fileExt));
        }

        /// <summary>
        /// 上传成功之后的处理,返回显示图片地址
        /// </summary>
        /// <param name="savePath"></param>
        /// <param name="form">上传时的附加参数</param>
        /// <returns></returns>
        protected virtual HandleResult UploadSuccessAfter(string savePath, IFormCollection form)
        {
            return new HandleResult
            {
                IsSuccess = true,
                Data = GetSaveWebFolderPath() + Path.GetFileName(savePath)
            };
        }

        /// <summary>
        /// 文件后缀是否在黑名单中
        /// </summary>
        /// <param name="ext"></param>
        /// <returns></returns>
        private bool ExistBack(string ext)
        {
            string backlist = ConfigHelper.GetAppSetting("backlist:upload");
            if (backlist.IsEmpty()) return false;

            List<string> back = backlist.Split(new[] {";"}, StringSplitOptions.RemoveEmptyEntries).ToList();
            return back.IndexOf(ext.ToLower().TrimStart('.')) > -1;
        }

        public async Task<HandleResult> Upload(IFormFile file, IFormCollection form)
        {
            string folderPath = Path.GetFullPath($"wwwroot{GetSaveWebFolderPath()}");
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            string fileName = file.FileName;
            string fileExt = Path.GetExtension(fileName);

            if (ExistBack(fileExt)) return HandleResult.Error("非法文件,无法上传");
            if (!ExistWhite(fileExt)) return HandleResult.Error("文件后缀不在可信范围内");

            string savePath = GetSavePath(folderPath, fileName, fileExt);
            await using (var stream = File.Create(savePath))
            {
                await file.CopyToAsync(stream);
            }

            return UploadSuccessAfter(savePath, form);
        }
    }
}