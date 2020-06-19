using System.Collections.Generic;
using System.Threading.Tasks;
using Foundation.DataAccess.Interface;
using Model.Account;

namespace DataAccess.Interface.Utils
{
    public interface ICodeGenerate: IDefaultDataAccess<Administrator>
    {
        Task<IEnumerable<string>> GetAllTableName();
        Task<IEnumerable<dynamic>> GetTableFields(string tableName);
    }
}