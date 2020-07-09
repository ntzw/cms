using System;
using CMS.Enums;

namespace CMS.React.Component
{
    public class InputAttribute : BaseComponent
    {
        public InputAttribute(string title) : base(title)
        {
        }

        public InputAttribute(string title, RegularType[] regularTypes) : base(title, regularTypes)
        {
            
        }

        public int MaxLength { get; set; } = 500;

        /// <summary>
        /// 带标签的 input，设置前置标签
        /// </summary>
        public string AddonBefore { get; set; }

        /// <summary>
        /// 带标签的 input，设置后置标签
        /// </summary>
        public string AddonAfter { get; set; }
    }
}