namespace CMS.Modules.Content.Abstractions.Model.Content
{
    public class ContentInfo
    {
        public Column Column { get; set; }

        public Site Site { get; set; }

        public ModelTable ModelTable { get; set; }

        public ContentData Data { get; set; }
        
        public ContentData NextData { get; set; }
        
        public ContentData PrevData { get; set; }
    }
}