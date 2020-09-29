using System.Collections.Generic;
using System.Threading.Tasks;
using CMS.Modules.Utils.Abstractions.Interface.Dapper;
using Dapper;
using Foundation.DataAccess;
using Foundation.DataAccess.Interface;

namespace CMS.Modules.Utils.DataAccess.SqlServer
{
    public class CodeGenerateDapperDapper : DefaultDataAccess<ModalBase>, ICodeGenerateDapper
    {
        public CodeGenerateDapperDapper(IPageSqlHelper pageSqlHelper) : base(pageSqlHelper)
        {
            
        }
        
        public Task<IEnumerable<string>> GetAllTableName()
        {
            string sql = $"select name from sysobjects where xtype='U'";
            return Connection().QueryAsync<string>(sql);
        }

        public Task<IEnumerable<dynamic>> GetTableFields(string tableName)
        {
            string sql = $@"SELECT col.name AS columnName,
                                     t.name AS dataType,
                                     col.max_length AS dataLength,
                                     col.is_nullable AS isNullable,
                                     ep.value AS description
                            FROM sys.objects obj
                            INNER JOIN sys.columns col
                                ON obj.object_id=col.object_id
                            LEFT JOIN sys.types t
                                ON t.user_type_id=col.user_type_id
                            LEFT JOIN sys.extended_properties ep
                                ON ep.major_id=obj.object_id
                                    AND ep.minor_id=col.column_id
                            WHERE obj.name= @TableName ";
            return Connection().QueryAsync<dynamic>(sql, new {TableName = tableName});
        }
    }
}