using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;
using Extension;
using Foundation.DataAccess.Connections;
using Foundation.DataAccess.Interface;
using Foundation.Modal;
using Foundation.Modal.RequestModal;
using Foundation.Modal.Result;

namespace Foundation.DataAccess
{
    public class DefaultDataAccess<T> : IDataAccess
        where T : class, new()
    {
        private readonly IPageSqlHelper _pageSqlHelper;

        public DefaultDataAccess(IPageSqlHelper pageSqlHelper)
        {
            _pageSqlHelper = pageSqlHelper;
        }

        public virtual IDbConnection Connection()
        {
            return MainConnection.Interface.GetConnection();
        }

        public Task<IEnumerable<T>> GetAll()
        {
            return Connection().GetAllAsync<T>();
        }

        public Task<T> GetById(int id)
        {
            return Connection().GetAsync<T>(id);
        }

        public Task<T> GetByNum(string num)
        {
            var sql = $"SELECT * FROM {GetTableName()} WHERE Num = @Num";
            return Connection().QueryFirstAsync<T>(sql, new {Num = num});
        }

        public Task<IEnumerable<T>> GetByNum(string[] num)
        {
            var sql = $"SELECT * FROM {GetTableName()} WHERE Num IN @Num";
            return Connection().QueryAsync<T>(sql, new {Num = num});
        }

        protected static string GetTableName()
        {
            return GetTableName<T>();
        }

        protected static string GetTableName<TT>() where TT : class, new()
        {
            return new TT().GetAttribute<TableAttribute>()?.Name;
        }

        #region 增删改

        public async Task<bool> Delete(string num)
        {
            var sql = $"DELETE FROM {GetTableName()} WHERE Num = @Num";
            var count = await Connection().ExecuteAsync(sql, new {Num = num});
            return count > 0;
        }

        public async Task<bool> Delete(string[] num)
        {
            var sql = $"DELETE FROM {GetTableName()} WHERE Num IN @Num";
            var count = await Connection().ExecuteAsync(sql, new {Num = num});
            return count > 0;
        }

        public Task<bool> Delete(T t)
        {
            return Connection().DeleteAsync(t);
        }

        public Task<bool> Delete(List<T> t)
        {
            return Connection().DeleteAsync(t);
        }

        public Task<int> Add(T t)
        {
            return Connection().InsertAsync(t);
        }

        public Task<int> Add(List<T> t)
        {
            return Connection().InsertAsync(t);
        }

        public Task<bool> Update(T t)
        {
            return Connection().UpdateAsync(t);
        }

        public Task<bool> Update(List<T> t)
        {
            return Connection().UpdateAsync(t);
        }

        #endregion

        #region 分页查询相关

        public virtual async Task<PageResponse> Page(IPageRequest req)
        {
            var tableName = GetTableName();
            var whereSql =
                _pageSqlHelper.OutDefaultParams(req.Queries, out var whereParams,
                    GetPageSetParamsAction());
            var dataSql = _pageSqlHelper.GetPageDataSql(req, whereSql, tableName);
            var countSql = _pageSqlHelper.GetPageCountSql(whereSql, tableName);

            return new PageResponse(await Connection().QueryAsync<dynamic>(dataSql, whereParams),
                await Connection().QueryFirstAsync<long>(countSql, whereParams));
        }

        /// <summary>
        ///     获取分页查询参数回调
        /// </summary>
        /// <returns></returns>
        protected virtual Action<IQuery, List<string>, IDictionary<string, object>> GetPageSetParamsAction()
        {
            return null;
        }

        #endregion
    }
}