namespace Foundation.Modal
{
    public interface IQuery
    {
        /// <summary>
        /// 查询数据
        /// </summary>
        object Value { get; }
        
        /// <summary>
        /// 查询条件SQL
        /// </summary>
        IQuerySql QuerySql { get; }
    }
}