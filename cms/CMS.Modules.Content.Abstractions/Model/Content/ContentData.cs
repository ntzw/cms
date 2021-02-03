using System;
using System.Collections.Generic;
using Extension;
using Helper;
using Newtonsoft.Json.Linq;

namespace CMS.Modules.Content.Abstractions.Model.Content
{
    public class ContentData
    {
        private readonly Dictionary<string, object> _dicValue =
            new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        public ContentData(JObject form)
        {
            foreach (var keyValue in form)
            {
                switch (keyValue.Key.ToLower())
                    {
                        case "id":
                            this.Id = keyValue.Value.ToInt();
                            break;
                        case "num":
                            this.Num = keyValue.Value.ToString();
                            break;
                        case "createdate":
                            this.CreateDate = keyValue.Value.ToDateTime();
                            break;
                        case "updatedate":
                            this.UpdateDate = keyValue.Value.ToDateTime();
                            break;
                        case "createaccountnum":
                            this.CreateAccountNum = keyValue.Value.ToStr();
                            break;
                        case "updateaccountnum":
                            this.UpdateAccountNum = keyValue.Value.ToStr();
                            break;
                        case "isdel":
                            this.IsDel = keyValue.Value.ToBoolean();
                            break;
                        case "status":
                            this.Status = keyValue.Value.ToInt();
                            break;
                        case "sitenum":
                            this.SiteNum = keyValue.Value.ToStr();
                            break;
                        case "columnnum":
                            this.ColumnNum = keyValue.Value.ToStr();
                            break;
                        case "categorynum":
                            this.CategoryNum = keyValue.Value.ToStr();
                            break;
                        case "seotitle":
                            this.SeoTitle = keyValue.Value.ToStr();
                            break;
                        case "seokeyword":
                            this.SeoKeyword = keyValue.Value.ToStr();
                            break;
                        case "seodesc":
                            this.SeoDesc = keyValue.Value.ToStr();
                            break;
                        case "clickcount":
                            this.ClickCount = keyValue.Value.ToInt();
                            break;
                        case "istop":
                            this.IsTop = keyValue.Value.ToBoolean();
                            break;
                        default:
                            _dicValue.Add(keyValue.Key, keyValue.Value);
                            break;
                    }
            }
        }
        
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

        public ContentData()
        {
        }

        /// <summary>
        /// 创建空实例
        /// </summary>
        /// <returns></returns>
        public static ContentData CreateEmptyInstance(string siteNum, string columnNum, string accountNum = "")
        {
            return new ContentData
            {
                Num = RandomHelper.CreateNum(),
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now,
                CreateAccountNum = accountNum,
                UpdateAccountNum = accountNum,
                SiteNum = siteNum,
                ColumnNum = columnNum,
            };
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
            set
            {
                if (_dicValue.ContainsKey(key))
                    _dicValue[key] = value;
                else
                    _dicValue.Add(key, value);
            }
        }

        /// <summary>
        /// 是否存在Key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(string key)
        {
            return _dicValue.ContainsKey(key);
        }

        public Dictionary<string, object> ToDictionary()
        {
           var data = new Dictionary<string, object>(_dicValue);
           
           data.Add("Id", Id);
           data.Add("Num", Num);
           data.Add("CreateDate",CreateDate);
           data.Add("UpdateDate",UpdateDate);
           data.Add("CreateAccountNum",CreateAccountNum);
           data.Add("UpdateAccountNum",UpdateAccountNum);
           data.Add("IsDel",IsDel);
           data.Add("Status",Status);
           data.Add("SiteNum",SiteNum);
           data.Add("ColumnNum",ColumnNum);
           data.Add("CategoryNum",CategoryNum);
           data.Add("SeoTitle",SeoTitle);
           data.Add("SeoKeyword",SeoKeyword);
           data.Add("SeoDesc",SeoDesc);
           data.Add("ClickCount",ClickCount);
           data.Add("IsTop",IsTop);

           return data;
        }
        
    }
}