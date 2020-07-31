using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Extension;
using Foundation.Modal;
using Foundation.Modal.RequestModal;
using Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Model.CMS;
using Model.CMS.Content;
using Service.CMS;
using Web.Models;

namespace Web.Controllers
{
    public class ContentController : Controller
    {
        public ContentController()
        {
        }

        public IActionResult Index()
        {
            var site = SessionHelper.Get<Site>("CurrentSite");
            if (site == null) return NotFound();

            return View($"~/Views/Content/{site.SiteFolder}/Index.cshtml");
        }

        [Route("list/{num}/{current:int?}")]
        [HttpGet]
        public async Task<IActionResult> List(string num, int current = 1)
        {
            if (num.IsEmpty()) return NotFound();

            var site = SessionHelper.Get<Site>("CurrentSite");
            if (site == null) return NotFound();

            var column = await ColumnService.Interface.GetByNum(num);
            if (column == null || column.SiteNum != site.Num) return NotFound();

            var model = await ModelTableService.Interface.GetByNum(column.ModelNum);
            if (model == null) return NotFound();

            if (column.IsSingle)
                return await Info(site, column, model);

            string templatePath = $"Views/Content/{site.SiteFolder}/{column.ListTemplatePath}";
            if (!System.IO.File.Exists(Path.GetFullPath(templatePath))) return NotFound();

            var req = new SqlServerPageRequest
            {
                Current = current,
                Size = 15,
                Queries = new List<IQuery>
                {
                    new DefaultQuery(column.Num, new DefaultQuerySql("columnNum"))
                }
            };

            var rep = await ContentService.Interface.Page(model.SqlTableName, req);

            ViewBag.Seo = new ContentSeo
            {
            };
            ViewData.Model = new ContentList
            {
                Column = column,
                ModelTable = model,
                Site = site,
                Data = rep.Data,
                PageConfig = new PageConfig
                {
                    Current = req.Current,
                    Size = req.Size,
                    Total = rep.Total,
                }
            };

            return View($"~/{templatePath}");
        }

        [NonAction]
        private async Task<IActionResult> Info(Site site, Column column, ModelTable model)
        {
            var item = await ContentService.Interface.GetFirstByColumnNum(model.SqlTableName, column.Num);
            return await Info(site, column, model, item);
        }

        [NonAction]
        private async Task<IActionResult> Info(Site site, Column column, ModelTable model, dynamic item)
        {
            string templatePath = $"Views/Content/{site.SiteFolder}/{column.InfoTemplatePath}";
            if (!System.IO.File.Exists(Path.GetFullPath(templatePath))) return NotFound();

            IDictionary<string, object> dataItem = item;
            int clickCount = dataItem["ClickCount"].ToInt();
            int id = dataItem["Id"].ToInt();

            ViewData.Model = new ContentInfo
            {
                Column = column,
                Site = site,
                ModelTable = model,
                Data = item,
                NextData = await ContentService.Interface.GetNext(model.SqlTableName, id),
                PrevData = await ContentService.Interface.GetPrev(model.SqlTableName, id)
            };

            ViewBag.Seo = new ContentSeo
            {
                Title = item.SeoTitle,
                Keyword = item.SeoKeyword,
                Desc = item.SeoDesc
            };


            await ContentService.Interface.UpdateClickCount(model.SqlTableName, id, clickCount + 1);
            return View($"~/{templatePath}");
        }

        [Route("info/{modelNum}/{num}")]
        public async Task<IActionResult> Info(string modelNum, string num)
        {
            var model = await ModelTableService.Interface.GetByNum(modelNum);
            if (model == null) return NotFound();

            var item = await ContentService.Interface.GetByNum(model.SqlTableName, num);
            if (item == null) return NotFound();

            string columnNum = item.ColumnNum;
            if (columnNum.IsEmpty()) return NotFound();

            var column = await ColumnService.Interface.GetByNum(columnNum);
            if (column == null) return NotFound();

            var site = await SiteService.Interface.GetByNum(column.SiteNum);
            if (site == null) return NotFound();

            return await Info(site, column, model, item);
        }

        public IActionResult NotFound()
        {
            return View("~/Views/Error/404.cshtml");
        }
    }
}