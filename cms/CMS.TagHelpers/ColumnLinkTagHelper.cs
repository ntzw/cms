using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Service.CMS;

namespace CMS.TagHelpers
{
    public class ColumnLinkTagHelper : TagHelper
    {
        public string Num { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "a";

            var column = await ColumnService.Interface.GetByNum(Num);
            if (column != null)
            {
                output.Attributes.SetAttribute("href", $"/list/{column.Num}");
                output.Content.SetContent(column.Name);
            }
        }
    }
}