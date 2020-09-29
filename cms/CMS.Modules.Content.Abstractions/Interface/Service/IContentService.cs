using System.Collections.Generic;
using System.Threading.Tasks;
using CMS.Modules.Content.Abstractions.Model;
using CMS.Modules.Content.Abstractions.Model.Content;
using Foundation.Modal;
using Foundation.Modal.RequestModal;
using Foundation.Modal.Result;
using Newtonsoft.Json.Linq;

namespace CMS.Modules.Content.Abstractions.Interface.Service
{
    public interface IContentService
    {
        Task<dynamic> GetByNum(string tableName, string num);

        Task<dynamic> GetByNumAndColumn(string columnNum, string num);

        Task<HandleResult> Edit(JObject form, string accountNum);

        Task<ContentData> GetByItem(string tableName, string num);

        Task<HandleResult> Update(Dictionary<string, object> form, string tableName);
        
        Task<HandleResult> Update(DynamicTableSqlHelper form, int id);

        Task<HandleResult> Add(DynamicTableSqlHelper form);

        Task<PageResponse> Page(JObject form, bool isRecycle = false);

        Task<PageResponse> Page(string tableName, IPageRequest req);

        Task<ContentData> GetFirstByColumnNum(string tableName, string columnNum);

        Task<ContentData> GetFirstByColumnNum(string columnNum);

        Task<ContentData> GetFirstByColumnNum(string columnNum, ISelectRequest request);

        Task<HandleResult> UpdateClickCount(string tableName, int id, int count);

        Task<ContentData> GetNext(string tableName, string columnNum, int id);
        
        Task<ContentData> GetPrev(string tableName, string columnNum, int id);

        Task<IEnumerable<dynamic>> GetByConditions(ISelectRequest request);

        Task<IEnumerable<dynamic>> GetByConditions(string tableName, ISelectRequest request);

        Task<IEnumerable<dynamic>> GetByColumnNum(string columnNum);

        Task<List<Dictionary<string, object>>> GetDictionaryDataByColumnNum(string columnNum);
        
        IDictionary<string, object> DynamicToDictionary(dynamic item);

        Task<HandleResult> GetCascaderData(string columnNum, string labelFieldName, string currentFieldName);

        Task<HandleResult> UpdateIsTop(string columnNum, string num, bool isTop);

        Task<HandleResult> Delete(string tableName, List<int> ids);

        Task<HandleResult> MoveRecycle(string tableName, List<int> ids);

        Task<HandleResult> Removed(string tableName, List<int> ids);
    }
}