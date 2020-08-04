using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace CMS.TagHelpers
{
    public class PagesTagHelper : TagHelper
    {
        public int Current { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        public long Total { get; set; } = 0;

        public int ShowCount { get; set; } = 4;

        public string PrevText { get; set; } = "上一页";

        public string NextText { get; set; } = "下一页";

        public string FirstText { get; set; } = "首页";

        public string LastText { get; set; } = "尾页";

        public string UrlTemplate { get; set; } = "/{page}.html";

        private long TotalPage => Math.Max((Total + PageSize - 1) / PageSize, 1);

        private List<long> PageList
        {
            get
            {
                List<long> pagerList = new List<long>();
                var currint = ShowCount / 2;
                for (var i = 0; i <= ShowCount; i++)
                {
                    //一共最多显示10个页码，前面5个，后面5个
                    if ((Current + i - currint) >= 1 && (Current + i - currint) <= TotalPage)
                    {
                        if (currint == i)
                        {
                            pagerList.Add(Current);
                        }
                        else
                        {
                            pagerList.Add(Current + i - currint);
                        }
                    }
                }

                return pagerList;
            }
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            output.Attributes.Add("class", "pages");
            if (Total > 0)
            {
                var ul = new TagBuilder("ul");
                if (Current > 1)
                {
                    ul.InnerHtml.AppendHtml($"<li><a href=\"{GetPageUrl(1)}\">{FirstText}</a></li>");
                    ul.InnerHtml.AppendHtml($"<li><a href=\"{GetPageUrl(Current - 1)}\">{PrevText}</a></li>");
                }

                foreach (var page in PageList)
                {
                    if (page == Current)
                    {
                        ul.InnerHtml.AppendHtml($"<li class=\"thisclass\">{page}</li>");
                    }
                    else
                    {
                        ul.InnerHtml.AppendHtml($"<li><a href=\"{GetPageUrl(page)}\">{page}</a></li>");
                    }
                }

                if (Current < TotalPage && TotalPage > 5)
                {
                    ul.InnerHtml.AppendHtml($"<li><a href=\"{GetPageUrl(Current + 1)}\">{NextText}</a></li>");
                    ul.InnerHtml.AppendHtml($"<li><a href=\"{GetPageUrl(TotalPage)}\">{LastText}</a></li>");
                }

                output.Content.SetHtmlContent(ul);
            }
        }

        private string GetPageUrl(long page)
        {
            return UrlTemplate.Replace("{p}", page.ToString());
        }
    }
}