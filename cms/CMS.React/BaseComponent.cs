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
    }
}