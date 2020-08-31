using System;
using System.Collections.Generic;
using Extension;

namespace Model.CMS.Content
{
    public class ContentData
    {
        private readonly Dictionary<string, object> _dicValue =
            new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        public ContentData(dynamic item)
        {
            if (item is IDictionary<string, object> itemValue)
            {
                foreach (var o in itemValue)
                {
                    switch (o.Key.ToLower())
                    {
                        case "id":
                            this.Id = o.Value.ToInt();
                            break;
                        case "num":
                            this.Num = o.Value.ToString();
                            break;
                        case "createdate":
                            this.CreateDate = o.Value.ToDateTime();
                            break;
                        case "updatedate":
                            this.UpdateDate = o.Value.ToDateTime();
                            break;
                        case "createaccountnum":
                            this.CreateAccountNum = o.Value.ToStr();
                            break;
                        case "updateaccountnum":
                            this.UpdateAccountNum = o.Value.ToStr();
                            break;
                        case "isdel":
                            this.IsDel = o.Value.ToBoolean();
                            break;
                        case "status":
                            this.Status = o.Value.ToInt();
                            break;
                        case "sitenum":
                            this.SiteNum = o.Value.ToStr();
                            break;
                        case "columnnum":
                            this.ColumnNum = o.Value.ToStr();
                            break;
                        case "categorynum":
                            this.CategoryNum = o.Value.ToStr();
                            break;
                        case "seotitle":
                            this.SeoTitle = o.Value.ToStr();
                            break;
                        case "seokeyword":
                            this.SeoKeyword = o.Value.ToStr();
                            break;
                        case "seodesc":
                            this.SeoDesc = o.Value.ToStr();
                            break;
                        case "clickcount":
                            this.ClickCount = o.Value.ToInt();
                            break;
                        case "istop":
                            this.IsTop = o.Value.ToBoolean();
                            break;
                        default:
                            _dicValue.Add(o.Key, o.Value);
                            break;
                    }
                }
            }
        }

        public int Id { get; set; }

        public string Num { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public string CreateAccountNum { get; set; }

        public string UpdateAccountNum { get; set; }

        public bool IsDel { get; set; }

        public int Status { get; set; }

        public string SiteNum { get; set; }

        public string ColumnNum { get; set; }

        public string CategoryNum { get; set; }

        public string SeoTitle { get; set; }

        public string SeoKeyword { get; set; }

        public string SeoDesc { get; set; }

        public int ClickCount { get; set; }

        public bool IsTop { get; set; }

        /// <summary>
        /// 其他属性
        /// </summary>
        /// <param name="key"></param>
        public object this[string key]
        {
            get
            {
                if (_dicValue.ContainsKey(key))
                    return _dicValue[key];

                return "";
            }
        }
    }
}