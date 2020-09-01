namespace Model.CMS.Content
{
    public class ContentSeo
    {
        public ContentSeo(){}

        public ContentSeo(Column column)
        {
            this.Title = column.SeoTitle;
            this.Keyword = column.SeoKeyword;
            this.Desc = column.SeoDesc;
        }
        
        public string Title { get; set; }
        
        public string Keyword { get; set; }
        
        public string Desc { get; set; }
    }
}