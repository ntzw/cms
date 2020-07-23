/**
 * author：created by zw on 2020-07-08 15:54:55
 * email：ntzw.geek@gmail.com
 */

using System.Threading.Tasks;
using CMS.Enums;
using Foundation.DataAccess.Interface;
using Model.CMS;

namespace DataAccess.Interface.CMS
{
    public interface IModelFieldDapper : IDefaultDataAccess<ModelField>
    {
        Task<int> CreateField(string fieldName, string tableName, ReactFormItemType fieldType);
        Task<ModelField> GetByName(string fieldName, string modelNum);
    }
}