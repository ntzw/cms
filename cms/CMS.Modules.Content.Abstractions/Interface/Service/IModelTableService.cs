using System.Collections.Generic;
using System.Threading.Tasks;
using CMS.Modules.Content.Abstractions.Model;
using CMS.React.Model;
using Foundation.Modal;

namespace CMS.Modules.Content.Abstractions.Interface.Service
{
    public interface IModelTableService: IDefaultService<ModelTable>
    {
        void CreateTable(ModelTable info);

        Task<ModelTable> GetByTableName(string tableName);

        Task<List<SelectDataType>> GetSelectData();
    }
}