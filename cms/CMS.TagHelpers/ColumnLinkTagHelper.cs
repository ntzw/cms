using System.Threading.Tasks;
using Extension;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Service.CMS;

namespace CMS.TagHelpers
{
    public class ColumnLinkTagHelper : TagHelper
    {
        public string Num { get; set; }

        public string Title { get; set; }

        public string Target { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "a";

            var column = await ColumnService.Interface.GetByNum(Num);
            if (column != null)
            {
                if (Target.IsNotEmpty())
                    output.Attributes.SetAttribute("target", Target);

                output.Attributes.SetAttribute("href", $"/list/{column.Num}");
                output.Attributes.SetAttribute("title", Title.IsEmpty() ? column.SeoTitle : Title);
                output.Content.SetContent(column.Name);
            }
        }
    }
}