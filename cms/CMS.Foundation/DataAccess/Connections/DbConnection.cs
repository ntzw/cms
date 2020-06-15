using System.Data;
using System.Data.SqlClient;

namespace Foundation.DataAccess.Connections
{
    public abstract class DbConnection
    {
        protected abstract SqlConnectionStringBuilder GetDbConnectionBuilder();
        
        public IDbConnection GetConnection()
        {
            return new SqlConnection(GetDbConnectionBuilder().ConnectionString);
        }
    }
}