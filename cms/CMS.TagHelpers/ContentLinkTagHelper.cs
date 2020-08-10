using System.Threading.Tasks;
using Extension;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace CMS.TagHelpers
{
    public class ContentLinkTagHelper : TagHelper
    {
        public string ColumnNum { get; set; }

        public string ItemNum { get; set; }

        public string Title { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "a";

            output.Attributes.SetAttribute("href", $"/info/{ColumnNum}/{ItemNum}");
            output.Attributes.SetAttribute("title", Title);
        }
    }
}