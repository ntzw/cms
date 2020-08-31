using System.Threading.Tasks;
using Extension;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Service.CMS;

namespace CMS.TagHelpers
{
    public class ColumnLinkTagHelper : TagHelper
    {
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

            var column = await ColumnService.Interface.GetByNum(Num);
            if (column != null)
            {
                if (Target.IsNotEmpty())
                    output.Attributes.SetAttribute("target", Target);

                output.Attributes.SetAttribute("href", $"/list/{(UrlNum.IsEmpty() ? column.Num : UrlNum)}");
                output.Attributes.SetAttribute("title", Title.IsEmpty() ? column.SeoTitle : Title);
                output.Content.SetContent(column.Name);
            }
        }
    }
}