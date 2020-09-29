using Foundation.Modal.RequestModal;

namespace Foundation.DataAccess
{
    public class MySqlPageHelper : PageDapperHelper
    {
        public override string GetPageCountSql(string whereSql, string tableName)
        {
            return $"SELECT COUNT(Id) FROM {tableName} WHERE 1=1 {whereSql}";
        }

        public override string GetPageDataSql(IPageRequest req, string whereSql, string tableName)
        {
            return
                $"SELECT * FROM {tableName} WHERE 1=1 {whereSql} {req.Sort.ToSql()} LIMIT {req.Begin - 1},{req.Size}";
        }
    }
}