using System;
using System.Collections.Concurrent;
using System.Data.SqlClient;
using CMS.Enums;
using Foundation.Application;
using MySql.Data.MySqlClient;

namespace Foundation.DataAccess.Connections
{
    public class MainConnection : DbConnection
    {
        private static MainConnection _interface;

        private readonly ConcurrentDictionary<string, string> _connectionString =
            new ConcurrentDictionary<string, string>();

        private MainConnection()
        {
        }

        public static MainConnection Interface => _interface ??= new MainConnection();

        protected override string GetDbConnectionBuilderString()
        {
            var sqlConnectName = ConfigHelper.GetAppSetting("currentConnectionStringName");

            if (!_connectionString.ContainsKey(sqlConnectName))
            {
                var sqlConnectionString = ConfigHelper.GetAppSetting("connectionStrings:" + sqlConnectName);
                switch (GetDataProvider())
                {
                    case DataProvider.SqlServer:
                        _connectionString[sqlConnectName] = new SqlConnectionStringBuilder(sqlConnectionString)
                        {
                            Pooling = true
                        }.ConnectionString;
                        break;
                    case DataProvider.MySql:
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
    }
}