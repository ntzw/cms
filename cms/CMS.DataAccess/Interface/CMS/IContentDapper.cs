using System.Collections.Generic;
using System.Threading.Tasks;
using Foundation.DataAccess.Interface;
using Foundation.Modal.RequestModal;
using Foundation.Modal.Result;

namespace DataAccess.Interface.CMS
{
    public interface IContentDapper : IDataAccess
    {
        Task<dynamic> GetByItem(string tableName, string itemNum);

        Task<int> Execute(string sql, object data);

        Task<PageResponse> Page(string tableName, IPageRequest req);

        Task<int> UpdateClickCount(string tableName, int id, int count);

        Task<dynamic> GetNext(string tableName, string columnNum, int id);

        Task<dynamic> GetPrev(string tableName, string columnNum, int id);

        Task<IEnumerable<dynamic>> GetByConditions(string tableName, ISelectRequest request);

        Task<int> UpdateIsTop(string tableName, string num, bool isTop);

        Task<int> Delete(string tableName, List<int> ids);

        Task<int> Recycle(string tableName, List<int> ids, bool isDel);
    }
}