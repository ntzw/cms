using System;

namespace CMS.React
{
    public class BaseComponent : Attribute
    {
        public BaseComponent(string title)
        {
            this.Title = title;
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
    }
}