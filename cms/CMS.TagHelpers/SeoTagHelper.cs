using Extension;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Service.CMS;

namespace CMS.TagHelpers
{
    public class SeoTagHelper : TagHelper
    {
        public string Title { get; set; }

        public string Keywords { get; set; }

        public string Description { get; set; }


        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = null;

            var site = SiteService.Interface.GetCurrentSite();
            
            output.Content.AppendHtml(new HtmlString($"<title>{(Title.IsEmpty() ? site?.SeoTitle : Title)}</title>"));
            output.Content.AppendHtml(
                new HtmlString(
                    $"<meta name=\"keywords\" content=\"{(Keywords.IsEmpty() ? site?.SeoKeywords : Keywords)}\"/>"));
            output.Content.AppendHtml(
                new HtmlString(
                    $"<meta name=\"description\" content=\"{(Description.IsEmpty() ? site?.SeoDescription : Description)}\"/>"));
        }
    }
}