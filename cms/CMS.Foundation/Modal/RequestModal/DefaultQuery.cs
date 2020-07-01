namespace Foundation.Modal.RequestModal
{
    public class DefaultQuery : IQuery
    {
        public DefaultQuery(object value, IQuerySql querySql)
        {
            Value = value;
            QuerySql = querySql;
        }

        public object Value { get; }

        public IQuerySql QuerySql { get; }
    }
}