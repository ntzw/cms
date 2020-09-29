using System.Collections.Generic;
using System.Threading.Tasks;

namespace CMS.Modules.Utils.Abstractions.Interface.Service
{
    public interface ICodeGenerateService
    {
        Task<IEnumerable<string>> GetAllTableName();

        Task<IEnumerable<dynamic>> GetTableFields(string tableName);
    }
}