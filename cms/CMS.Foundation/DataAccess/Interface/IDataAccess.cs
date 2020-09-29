using System.Data;

namespace Foundation.DataAccess.Interface
{
    public interface IDataAccess
    {
        IDbConnection Connection();
    }
}