using System.Collections.Generic;
using System.Threading.Tasks;
using CMS.Modules.Content.Abstractions.Model;
using Foundation.Modal;
using Foundation.Modal.Result;

namespace CMS.Modules.Content.Abstractions.Interface.Service
{
    public interface IColumnFieldService : IDefaultService<ColumnField>
    {
        Task<IEnumerable<ColumnField>> GetByModelFieldNum(string columnNum, List<string> modelFieldNum);

        Task<IEnumerable<ColumnField>> GetByColumnNum(string columnNum);

        Task<HandleResult> Clear(string[] columnNums);
    }
}