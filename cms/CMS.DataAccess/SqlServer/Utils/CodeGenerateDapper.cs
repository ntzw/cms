using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using DataAccess.Interface.Utils;
using Model.Account;

namespace DataAccess.SqlServer.Utils
{
    public class CodeGenerateDapper : DefaultDataAccess<Administrator>, ICodeGenerate
    {
        public Task<IEnumerable<string>> GetAllTableName()
        {
            string sql = $"select name from sysobjects where xtype='U'";
            return Connection().QueryAsync<string>(sql);
        }

        public Task<IEnumerable<dynamic>> GetTableFields(string tableName)
        {
            string sql = $@"SELECT col.name AS ColumnName,
                                     t.name AS DataType,
                                     col.max_length AS DataLength,
                                     col.is_nullable AS IsNullable,
                                     ep.value AS Description
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