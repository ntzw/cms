using CMS.Enums;

namespace Foundation.Modal
{
    public interface IQuerySql
    {
        string ToWhereString();
        
        /// <summary>
        /// 字段名称
        /// </summary>
        string FieldName { get; }
        
        /// <summary>
        /// 参数名称
        /// </summary>
        string ParamName { get; }
        
        /// <summary>
        /// 符号
        /// </summary>
        QuerySymbol Symbol { get; }
        
        
    }
}