using System;
using System.Collections.Concurrent;
using System.Data.SqlClient;
using Foundation.Application;
using MySql.Data.MySqlClient;

namespace Foundation.DataAccess.Connections
{
    public class DynamicConnection : DbConnection
    {
        private static readonly ConcurrentDictionary<string, DynamicConnection> SqlConnectionStringBuilders =
            new ConcurrentDictionary<string, DynamicConnection>();

        private readonly ConcurrentDictionary<string, string> _connectionString =
            new ConcurrentDictionary<string, string>();

        private readonly string _dbBaseName;

        private DynamicConnection(string dbBaseName)
        {
            _dbBaseName = dbBaseName;
        }

        protected override string GetDbConnectionBuilderString()
        {
            var sqlConnectName = ConfigHelper.GetAppSetting("currentConnectionStringName");

            if (!_connectionString.ContainsKey(sqlConnectName))
            {
                var sqlConnectionString = ConfigHelper.GetAppSetting("_connectionStrings:" + sqlConnectName);
                switch (sqlConnectName.ToLower())
                {
                    case "sqlserver":
                        _connectionString[sqlConnectName] = new SqlConnectionStringBuilder(sqlConnectionString)
                        {
                            Pooling = true
                        }.ConnectionString;
                        break;
                    case "mysql":
                        _connectionString[sqlConnectName] = new MySqlConnectionStringBuilder(sqlConnectionString)
                        {
                            Pooling = true
                        }.ConnectionString;
                        break;
                    default:
                        throw new Exception("只支持 sqlserver 、 mysql");
                }
            }

            return _connectionString[sqlConnectName];
        }

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