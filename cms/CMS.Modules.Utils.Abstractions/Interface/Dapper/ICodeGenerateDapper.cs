using System.Collections.Generic;
using System.Threading.Tasks;
using Foundation.DataAccess;
using Foundation.DataAccess.Interface;

namespace CMS.Modules.Utils.Abstractions.Interface.Dapper
{
    public interface ICodeGenerateDapper: IDefaultDataAccess<ModalBase>
    {
        Task<IEnumerable<string>> GetAllTableName();
        Task<IEnumerable<dynamic>> GetTableFields(string tableName);
    }
}