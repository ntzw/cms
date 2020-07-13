namespace CMS.React.Component
{
    public class SelectAttribute : BaseComponent
    {
        public SelectAttribute(string title) : base(title)
        {
        }

        /// <summary>
        /// 设置 Select 的模式为多选或标签	multiple | tags
        /// </summary>
        public string Mode { get; set; }
        
        /// <summary>
        /// 使单选模式可搜索
        /// </summary>
        public bool ShowSearch { get; set; }
    }
}