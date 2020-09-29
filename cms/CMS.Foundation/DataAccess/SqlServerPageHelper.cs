using Foundation.Modal.RequestModal;

namespace Foundation.DataAccess
{
    public class SqlServerPageHelper : PageDapperHelper
    {
        public override string GetPageCountSql(string whereSql, string tableName)
        {
            return $"SELECT COUNT(Id) FROM {tableName} WHERE 1=1 {whereSql}";
        }

        public override string GetPageDataSql(IPageRequest req, string whereSql, string tableName)
        {
            return
                $"SELECT * FROM (SELECT *,ROW_NUMBER() OVER({req.Sort.ToSql()}) AS RowNum FROM [{tableName}] WHERE 1=1 {whereSql}) AS temp WHERE RowNum BETWEEN {req.Begin} AND {req.End} ";
        }
    }
}