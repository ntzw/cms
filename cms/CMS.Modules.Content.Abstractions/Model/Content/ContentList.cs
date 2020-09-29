using System.Collections.Generic;

namespace CMS.Modules.Content.Abstractions.Model.Content
{
    public class ContentList
    {
        public Column Column { get; set; }

        public Site Site { get; set; }

        public ModelTable ModelTable { get; set; }

        public IEnumerable<dynamic> Data { get; set; }

        public PageConfig PageConfig { get; set; }
    }

    public class PageConfig
    {
        public int Current { get; set; }

        public int Size { get; set; }

        public long Total { get; set; }

        public long Begin => (Current - 1) * Size + 1;

        public long End => Current * Size;
    }
}