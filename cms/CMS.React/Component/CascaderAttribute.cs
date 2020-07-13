namespace CMS.React.Component
{
    public class CascaderAttribute: BaseComponent
    {
        public CascaderAttribute(string title) : base(title)
        {
        }

        /// <summary>
        /// 次级菜单的展开方式，可选 'click' 和 'hover'
        /// </summary>
        public string ExpandTrigger { get; set; } = "click";
        
        /// <summary>
        /// 当此项为 true 时，点选每级菜单选项值都会发生变化
        /// </summary>
        public bool ChangeOnSelect { get; set; }
    }
}