using System.Threading.Tasks;
using CMS.Modules.Content.Abstractions.Interface.Service;
using Extension;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace CMS.TagHelpers
{
    public class ColumnLinkTagHelper : TagHelper
    {
        private readonly IColumnService _columnService;

        public ColumnLinkTagHelper(IColumnService columnService)
        {
            _columnService = columnService;
        }

        /// <summary>
        /// 栏目编号
        /// </summary>
        public string Num { get; set; }

        public string Title { get; set; }

        public string Target { get; set; }

        /// <summary>
        /// 需要跳转到的栏目编号（默认栏目编号）
        /// </summary>
        public string UrlNum { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "a";

            var column = await _columnService.GetByNum(Num);
            if (column != null)
            {
                if (Target.IsNotEmpty())
                    output.Attributes.SetAttribute("target", Target);

                output.Attributes.SetAttribute("href", $"/list/{(UrlNum.IsEmpty() ? column.Num : UrlNum)}");
                output.Attributes.SetAttribute("title", Title.IsEmpty() ? column.SeoTitle : Title);

                context.Items["Name"] = column.Name;

                TagHelperContent content = await output.GetChildContentAsync();
                if (content.IsEmptyOrWhiteSpace)
                    output.Content.SetContent(column.Name);
            }
        }
    }

    public class ColumnNameTagHelper : TagHelper
    {
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "";
            if (context.Items.ContainsKey("Name"))
                output.Content.SetContent(context.Items["Name"].ToStr());
        }
    }
}