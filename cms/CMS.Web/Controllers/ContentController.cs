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
            if (site == null) return Error404();

            return View($"~/Views/Content/{site.SiteFolder}/Index.cshtml");
        }

        [Route("list/{num}/{current:int?}")]
        [HttpGet]
        public async Task<IActionResult> List(string num, int current = 1)
        {
            if (num.IsEmpty()) return Error404();

            var site = SiteService.Interface.GetCurrentSite();
            if (site == null) return Error404();

            var column = await ColumnService.Interface.GetByNum(num);
            if (column == null || column.SiteNum != site.Num) return Error404();

            var model = column.ModelNum.IsEmpty() ? null : await ModelTableService.Interface.GetByNum(column.ModelNum);
            if (column.IsSingle)
                return await Info(site, column, model);

            string templatePath = $"Views/Content/{site.SiteFolder}/{column.ListTemplatePath}";
            if (!System.IO.File.Exists(Path.GetFullPath(templatePath))) return Error404();

            var req = new SqlServerPageRequest
            {
                Current = current,
                Size = 15,
                Queries = new List<IQuery>
                {
                    new DefaultQuery(column.Num, new DefaultQuerySql("columnNum")),
                    new DefaultQuery(false, new DefaultQuerySql("IsDel"))
                }
            };

            var seo = new ContentSeo(column);
            if (model != null)
            {
                var rep = await ContentService.Interface.Page(model.SqlTableName, req);
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
            }

            ViewBag.Seo = seo;
            ViewBag.Site = site;
            return View($"~/{templatePath}");
        }

        [NonAction]
        private async Task<IActionResult> Info(Site site, Column column, ModelTable model)
        {
            return await Info(site, column, model,
                model != null
                    ? await ContentService.Interface.GetFirstByColumnNum(model.SqlTableName, column.Num)
                    : null);
        }

        [NonAction]
        private async Task<IActionResult> Info(Site site, Column column, ModelTable model, ContentData item)
        {
            string templatePath = $"Views/Content/{site.SiteFolder}/{column.InfoTemplatePath}";
            if (!System.IO.File.Exists(Path.GetFullPath(templatePath))) return Error404();

            var seo = new ContentSeo(column);
            var info = new ContentInfo();
            if (item != null)
            {
                int clickCount = item.ClickCount;
                int id = item.Id;

                info.Column = column;
                info.Site = site;
                info.ModelTable = model;
                info.Data = item;
                info.NextData = await ContentService.Interface.GetNext(model.SqlTableName, column.Num, id);
                info.PrevData = await ContentService.Interface.GetPrev(model.SqlTableName, column.Num, id);

                seo.Title = item.SeoTitle;
                seo.Keyword = item.SeoKeyword;
                seo.Desc = item.SeoDesc;
                await ContentService.Interface.UpdateClickCount(model.SqlTableName, id, clickCount + 1);
            }

            ViewData.Model = info;
            ViewBag.Seo = seo;
            ViewBag.Site = site;
            return View($"~/{templatePath}");
        }

        [Route("info/{columnNum}/{num}")]
        public async Task<IActionResult> Info(string columnNum, string num)
        {
            var cm = await ColumnService.Interface.GetModelByNum(columnNum);
            if (cm == null) return Error404();

            var item = await ContentService.Interface.GetByNum(cm?.ModelTable.SqlTableName, num);
            if (item == null) return Error404();

            var site = await SiteService.Interface.GetByNum(cm?.Column.SiteNum);
            if (site == null) return Error404();

            return await Info(site, cm?.Column, cm?.ModelTable, item);
        }

        [NonAction]
        private IActionResult Error404()
        {
            return View("~/Views/Error/404.cshtml");
        }
    }
}