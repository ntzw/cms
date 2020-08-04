using System.Collections.Generic;
using System.Threading.Tasks;
using Foundation.Modal;
using Foundation.Modal.RequestModal;
using Foundation.Modal.Result;

namespace DataAccess.Interface.CMS
{
    public interface IContentDapper
    {
        Task<dynamic> GetByItem(string tableName, string itemNum);

        Task<int> Execute(string sql, object data);

        Task<PageResponse> Page(string tableName, IPageRequest req);

        Task<dynamic> GetFirstByColumnNum(string tableName, string columnNum);

        Task<int> UpdateClickCount(string tableName, int id, int count);

        Task<dynamic> GetNext(string tableName, string columnNum, int id);

        Task<dynamic> GetPrev(string tableName, string columnNum, int id);

        Task<IEnumerable<dynamic>> GetByConditions(string tableName, ISelectRequest request);
    }
}