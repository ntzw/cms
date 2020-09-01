using System;
using System.Collections.Concurrent;
using System.Data;
using System.Data.SqlClient;
using Foundation.Application;
using Helper;
using MySql.Data.MySqlClient;

namespace Foundation.DataAccess.Connections
{
    public class DynamicConnection : DbConnection
    {
        private readonly string _dbBaseName;

        private DynamicConnection(string dbBaseName)
        {
            _dbBaseName = dbBaseName;
        }

        readonly ConcurrentDictionary<string, string> _connectionString = new ConcurrentDictionary<string, string>();

        protected override string GetDbConnectionBuilderString()
        {
            string sqlConnectName = ConfigHelper.GetAppSetting("currentConnectionStringName");

            if (!_connectionString.ContainsKey(sqlConnectName))
            {
                string sqlConnectionString = ConfigHelper.GetAppSetting("_connectionStrings:" + sqlConnectName);
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