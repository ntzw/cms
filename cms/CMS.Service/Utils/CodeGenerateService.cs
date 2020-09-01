using System.Collections.Generic;
using System.Threading.Tasks;
using DataAccess;
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

        private readonly ICodeGenerateDapper _dapper = DataAccessFactory.GetInstance<ICodeGenerateDapper>();
        
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