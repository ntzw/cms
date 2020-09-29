using System.Collections.Generic;
using System.Threading.Tasks;
using CMS.Modules.Utils.Abstractions.Interface.Dapper;
using CMS.Modules.Utils.Abstractions.Interface.Service;

namespace CMS.Modules.Utils.Service
{
    public class CodeGenerateService : ICodeGenerateService
    {
        private readonly ICodeGenerateDapper _dapper;

        public CodeGenerateService(ICodeGenerateDapper dapper)
        {
            _dapper = dapper;
        }

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