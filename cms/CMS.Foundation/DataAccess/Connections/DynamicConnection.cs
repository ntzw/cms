using System.Collections.Concurrent;
using System.Data;
using System.Data.SqlClient;
using Foundation.Application;
using Helper;

namespace Foundation.DataAccess.Connections
{
    public class DynamicConnection : DbConnection
    {
        private readonly string _dbBaseName;

        private DynamicConnection(string dbBaseName)
        {
            _dbBaseName = dbBaseName;
        }

        private SqlConnectionStringBuilder _builder;

        protected override SqlConnectionStringBuilder GetDbConnectionBuilder()
        {
            if (_builder != null) return _builder;

            string sqlConnectName = ConfigHelper.GetAppSetting("currentConnectionStringName");
            string sqlConnectionString = ConfigHelper.GetAppSetting("connectionStrings:" + sqlConnectName);
            var mainBuilder = new SqlConnectionStringBuilder(sqlConnectionString)
            {
                Pooling = true
            };

            sqlConnectionString =
                $"Server={mainBuilder.DataSource};User Id={mainBuilder.UserID};Pwd={mainBuilder.Password};DataBase={_dbBaseName};Persist Security Info=True;";
            _builder = new SqlConnectionStringBuilder(sqlConnectionString)
            {
                Pooling = true
            };

            return _builder;
        }

        private static readonly ConcurrentDictionary<string, DynamicConnection> SqlConnectionStringBuilders =
            new ConcurrentDictionary<string, DynamicConnection>();

        public static DynamicConnection Interface(string dbBaseName = "")
        {
            if (string.IsNullOrEmpty(dbBaseName))
                dbBaseName = SessionHelper.Get("DynamicConnection");
            
            if (!SqlConnectionStringBuilders.ContainsKey(dbBaseName))
                SqlConnectionStringBuilders[dbBaseName] = new DynamicConnection(dbBaseName);

            return SqlConnectionStringBuilders[dbBaseName];
        }
    }
}