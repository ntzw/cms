namespace CMS.React.Component
{
    public class PasswordAttribute : BaseComponent
    {
        public PasswordAttribute(string title) : base(title)
        {
        }

        public int MaxLength { get; set; } = 20;
    }
}