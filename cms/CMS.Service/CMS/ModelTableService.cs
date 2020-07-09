/**
 * author：created by zw on 2020-07-08 15:54:54
 * email：ntzw.geek@gmail.com
 */

using System.Threading.Tasks;
using DataAccess.Interface.CMS;
using DataAccess.SqlServer.CMS;
using Model.CMS;

namespace Service.CMS
{
    public class ModelTableService : DefaultService<ModelTable, IModelTableDapper>
    {
        private ModelTableDapper _dapper;

        protected override IModelTableDapper GetDapper() => _dapper ??= new ModelTableDapper();

        private ModelTableService()
        {
        }

        private static ModelTableService _interface;
        public static ModelTableService Interface => _interface ??= new ModelTableService();

        public void CreateTable(ModelTable info)
        {
            Task.Run(async () =>
            {
                string tableName = $"CMS_U_{info.TableName}";
                await GetDapper().CreateTable(tableName);
            });
        }

        public Task<ModelTable> GetByTableName(string tableName)
        {
            return GetDapper().GetByTableName(tableName);
        }
    }
}