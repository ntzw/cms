using CMS.Modules.Content.Abstractions.Interface.Service;
using Extension;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace CMS.TagHelpers
{
    public class SeoTagHelper : TagHelper
    {
        private readonly ISiteService _siteService;

        public SeoTagHelper(ISiteService siteService)
        {
            _siteService = siteService;
        }

        public string Title { get; set; }

        public string Keywords { get; set; }

        public string Description { get; set; }


        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = null;

            var site = _siteService.GetCurrentSite();

            output.Content.AppendHtml(new HtmlString($"<title>{(Title.IsEmpty() ? site?.SeoTitle : Title)}</title>"));
            output.Content.AppendLine();
            output.Content.AppendHtml(
                new HtmlString(
                    $"<meta name=\"keywords\" content=\"{(Keywords.IsEmpty() ? site?.SeoKeyword : Keywords)}\"/>"));
            output.Content.AppendLine();
            output.Content.AppendHtml(
                new HtmlString(
                    $"<meta name=\"description\" content=\"{(Description.IsEmpty() ? site?.SeoDesc : Description)}\"/>"));
            output.Content.AppendLine();
        }
    }
}