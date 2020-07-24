namespace Foundation.Modal
{
    public interface ISort
    {
        string ToSql(string prefix = "");
        
        ISort Add(string field, string order);

        void Delete(string field);
    }
}