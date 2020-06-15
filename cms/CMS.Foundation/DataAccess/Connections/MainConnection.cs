using System.Data;
using System.Data.SqlClient;
using Helper;

namespace Foundation.DataAccess.Connections
{
    public class MainConnection: DbConnection
    {
        private MainConnection()
        {
            
        }

        private static MainConnection _interface;
        public static MainConnection Interface => _interface ??= new MainConnection();
        

        private SqlConnectionStringBuilder _builder;
        protected override SqlConnectionStringBuilder GetDbConnectionBuilder()
        {
            if (_builder != null) return _builder;
            string sqlConnectName = ConfigHelper.GetAppSetting("currentConnectionStringName");
            string sqlConnectionString = ConfigHelper.GetAppSetting("connectionStrings:" + sqlConnectName);
            _builder = new SqlConnectionStringBuilder(sqlConnectionString)
            {
                Pooling = true
            };

            return _builder;
        }
    }
}