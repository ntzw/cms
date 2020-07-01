using System;

namespace CMS.React.Component
{
    public class InputAttribute : BaseComponent
    {
        public InputAttribute(string title) : base(title)
        {
            
        }

        public int MaxLength { get; set; } = 500;
    }
}