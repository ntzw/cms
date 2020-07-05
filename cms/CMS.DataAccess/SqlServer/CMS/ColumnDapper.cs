/**
 * author：created by zw on 2020-06-20 14:28:18
 * email：ntzw.geek@gmail.com
 */

using System.Threading.Tasks;
using DataAccess.Interface.CMS;
using Foundation.Modal;
using Foundation.Modal.RequestModal;
using Model.CMS;

namespace DataAccess.SqlServer.CMS
{
    public class ColumnDapper : DefaultDataAccess<Column>, IColumnDapper
    {
    }
}