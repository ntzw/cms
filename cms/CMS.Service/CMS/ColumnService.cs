/**
 * author：created by zw on 2020-06-20 14:28:19
 * email：ntzw.geek@gmail.com
 */

using DataAccess.Interface.CMS;
using DataAccess.SqlServer.CMS;
using Model.CMS;

namespace Service.CMS
{
    public class ColumnService : DefaultService<Column, IColumnDapper>
    {
        private IColumnDapper _dapper;

        protected override IColumnDapper GetDapper() => _dapper ??= new ColumnDapper();

        private ColumnService()
        {
        }

        private static ColumnService _interface;
        public static ColumnService Interface => _interface ??= new ColumnService();
    }
}