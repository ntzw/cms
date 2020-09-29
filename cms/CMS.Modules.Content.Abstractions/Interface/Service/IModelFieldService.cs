using System.Threading.Tasks;
using CMS.Modules.Content.Abstractions.Model;
using Foundation.Modal;
using Foundation.Modal.Result;

namespace CMS.Modules.Content.Abstractions.Interface.Service
{
    public interface IModelFieldService: IDefaultService<ModelField>
    {
        Task<ModelField> GetByName(string fieldName, string modelNum);

        Task<HandleResult> CreateField(ModelField info);
        
        
    }
}