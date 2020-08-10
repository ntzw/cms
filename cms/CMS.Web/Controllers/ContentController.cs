using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Extension;
using Foundation.Application;
using Foundation.Modal;
using Foundation.Modal.RequestModal;
using Microsoft.AspNetCore.Mvc;
using Model.CMS;
using Model.CMS.Content;
using Service.CMS;

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

            var site = SiteService.Interface.GetCurrentSite();
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
            
            ViewBag.Site = site;

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
                NextData = await ContentService.Interface.GetNext(model.SqlTableName, column.Num, id),
                PrevData = await ContentService.Interface.GetPrev(model.SqlTableName, column.Num, id)
            };

            ViewBag.Seo = new ContentSeo
            {
                Title = item.SeoTitle,
                Keyword = item.SeoKeyword,
                Desc = item.SeoDesc
            };

            ViewBag.Site = site;

            await ContentService.Interface.UpdateClickCount(model.SqlTableName, id, clickCount + 1);
            return View($"~/{templatePath}");
        }

        [Route("info/{columnNum}/{num}")]
        public async Task<IActionResult> Info(string columnNum, string num)
        {
            var cm = await ColumnService.Interface.GetModelByNum(columnNum);
            if (cm == null) return NotFound();

            var item = await ContentService.Interface.GetByNum(cm?.ModelTable.SqlTableName, num);
            if (item == null) return NotFound();

            var site = await SiteService.Interface.GetByNum(cm?.Column.SiteNum);
            if (site == null) return NotFound();

            return await Info(site, cm?.Column, cm?.ModelTable, item);
        }

        public IActionResult NotFound()
        {
            return View("~/Views/Error/404.cshtml");
        }
    }
}