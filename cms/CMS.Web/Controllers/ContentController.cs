using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CMS.Modules.Content.Abstractions.Interface.Service;
using CMS.Modules.Content.Abstractions.Model;
using CMS.Modules.Content.Abstractions.Model.Content;
using Extension;
using Foundation.Application;
using Foundation.Modal;
using Foundation.Modal.RequestModal;
using Foundation.Modal.Result;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    public class ContentController : Controller
    {
        private readonly ISiteService _siteService;
        private readonly IContentService _contentService;
        private readonly IColumnService _columnService;
        private readonly IModelTableService _modelTableService;
        private readonly IColumnFieldService _columnFieldService;

        public ContentController(
            ISiteService siteService,
            IContentService contentService,
            IColumnService columnService,
            IModelTableService modelTableService,
            IColumnFieldService columnFieldService)
        {
            _siteService = siteService;
            _contentService = contentService;
            _columnService = columnService;
            _modelTableService = modelTableService;
            _columnFieldService = columnFieldService;
        }

        public IActionResult Index()
        {
            var site = SessionHelper.Get<Site>("CurrentSite");
            if (site == null) return Error404();

            string folderName = site.IsMobileSite ? site.MobileSiteFolder : site.SiteFolder;
            return View($"~/Views/Content/{folderName}/Index.cshtml");
        }

        [Route("list/{num}/{current:int?}")]
        [HttpGet]
        public async Task<IActionResult> List(string num, int current = 1)
        {
            if (num.IsEmpty()) return Error404();

            var site = _siteService.GetCurrentSite();
            if (site == null) return Error404();

            var column = await _columnService.GetByNum(num);
            if (column == null || column.SiteNum != site.Num) return Error404();

            var model = column.ModelNum.IsEmpty() ? null : await _modelTableService.GetByNum(column.ModelNum);
            if (column.IsSingle)
                return await RenderInfo(site, column, model);
            
            string folderName = site.IsMobileSite ? site.MobileSiteFolder : site.SiteFolder;
            string templatePath = $"Views/Content/{folderName}/{column.ListTemplatePath}";
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
                var rep = await _contentService.Page(model.SqlTableName, req);
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

        [Route("info/{columnNum}/{num}")]
        public async Task<IActionResult> Info(string columnNum, string num)
        {
            var cm = await _columnService.GetModelByNum(columnNum);
            if (cm == null) return Error404();

            var item = await _contentService.GetByItem(cm?.ModelTable.SqlTableName, num);
            if (item == null) return Error404();

            var site = await _siteService.GetByNum(cm?.Column.SiteNum);
            if (site == null) return Error404();

            return await RenderInfo(site, cm?.Column, cm?.ModelTable, item);
        }

        [NonAction]
        private async Task<IActionResult> RenderInfo(Site site, Column column, ModelTable model)
        {
            ContentData item = null;
            if (model != null)
                item = await _contentService.GetFirstByColumnNum(model.SqlTableName, column.Num);

            return await RenderInfo(site, column, model, item);
        }

        [NonAction]
        private async Task<IActionResult> RenderInfo(Site site, Column column, ModelTable model, ContentData item)
        {
            string folderName = site.IsMobileSite ? site.MobileSiteFolder : site.SiteFolder;
            string templatePath = $"Views/Content/{folderName}/{column.InfoTemplatePath}";
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
                info.NextData = await _contentService.GetNext(model.SqlTableName, column.Num, id);
                info.PrevData = await _contentService.GetPrev(model.SqlTableName, column.Num, id);

                seo.Title = item.SeoTitle;
                seo.Keyword = item.SeoKeyword;
                seo.Desc = item.SeoDesc;
                await _contentService.UpdateClickCount(model.SqlTableName, id, clickCount + 1);
            }

            ViewData.Model = info;
            ViewBag.Seo = seo;
            ViewBag.Site = site;
            return View($"~/{templatePath}");
        }

        [NonAction]
        private IActionResult Error404()
        {
            return View("~/Views/Error/404.cshtml");
        }

        public async Task<HandleResult> Submit(IFormCollection form)
        {
            string columnNum = form["columnNum"];
            if (columnNum.IsEmpty()) return HandleResult.Error();

            var columnModel = await _columnService.GetModelByNum(columnNum);
            if (columnModel?.ModelTable == null) return HandleResult.Error();

            var fields = await _columnFieldService.GetByColumnNum(columnNum);
            var columnFields = fields as ColumnField[] ?? fields.ToArray();

            var edit = ContentData.CreateEmptyInstance(columnModel.Column.SiteNum, columnModel.Column.Num);

            foreach (var field in columnFields)
            {
                if (form.ContainsKey(field.Name))
                {
                    edit[field.Name] = form[field.Name];
                }
            }

            var tableSqlHelper = new DynamicTableSqlHelper(columnModel.ModelTable.SqlTableName);
            tableSqlHelper.SetData(columnFields, edit);

            return await _contentService.Add(tableSqlHelper);
        }
    }
}