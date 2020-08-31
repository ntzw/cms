namespace Model.CMS.Content
{
    public class ContentInfo
    {
        public Column Column { get; set; }

        public Site Site { get; set; }

        public ModelTable ModelTable { get; set; }

        public ContentData Data { get; set; }
        
        public dynamic NextData { get; set; }
        
        public dynamic PrevData { get; set; }
    }
}