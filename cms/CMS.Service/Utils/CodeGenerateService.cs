using System.Collections.Generic;
using System.Threading.Tasks;
using DataAccess.Interface.Utils;
using DataAccess.SqlServer.Utils;

namespace Service.Utils
{
    public class CodeGenerateService
    {
        private CodeGenerateService()
        {
        }

        private static CodeGenerateService _interface;
        public static CodeGenerateService Interface => _interface ??= new CodeGenerateService();

        private readonly ICodeGenerate _dapper = new CodeGenerateDapper();
        
        public Task<IEnumerable<string>> GetAllTableName()
        {
            return _dapper.GetAllTableName();
        }

        public Task<IEnumerable<dynamic>> GetTableFields(string tableName)
        {
            return _dapper.GetTableFields(tableName);
        }
    }
}