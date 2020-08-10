using System;
using CMS.Enums;

namespace CMS.React
{
    public class BaseComponent : Attribute
    {
        public BaseComponent(string title)
        {
            this.Title = title;
        }

        public BaseComponent(string title, RegularType[] regularTypes)
        {
            this.Title = title;
            this.RegularTypes = regularTypes;
        }

        public string Title { get; set; }

        public bool Required { get; set; }

        public string Placeholder { get; set; }

        /// <summary>
        /// 数据请求地址，请支持那些可以远程获取数据的组件，如Select
        /// </summary>
        public string DataAction { get; set; }

        /// <summary>
        /// 更新时显示
        /// </summary>
        public bool UpdateShow { get; set; } = true;

        /// <summary>
        /// 分隔符
        /// </summary>
        public string Split { get; set; } = ",";

        /// <summary>
        /// 正则类型
        /// </summary>
        public RegularType[] RegularTypes { get; set; }

        /// <summary>
        /// 是否禁用
        /// </summary>
        public bool Disabled { get; set; }

        /// <summary>
        /// 更新时禁用
        /// </summary>
        public bool UpdateDisabled { get; set; }

        /// <summary>
        /// 是否支持清除
        /// </summary>
        public bool AllowClear { get; set; }

        /// <summary>
        /// 提示信息
        /// </summary>
        public string Extra { get; set; }
    }
}