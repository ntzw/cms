namespace CMS.React.Component
{
    public class SwitchAttribute: BaseComponent
    {
        public SwitchAttribute(string title) : base(title)
        {
        }

        /// <summary>
        /// 选中时的内容
        /// </summary>
        public string CheckedChildren { get; set; } = "是";

        /// <summary>
        /// 非选中时的内容
        /// </summary>
        public string UnCheckedChildren { get; set; } = "否";
    }
}