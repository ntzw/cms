namespace CMS.React.Component
{
    public class TextAreaAttribute : BaseComponent
    {
        public TextAreaAttribute(string title) : base(title)
        {
        }

        public int MaxLength { get; set; } = 500;

        public int Rows { get; set; } = 4;
    }
}