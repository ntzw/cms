using System;
using System.Collections.Concurrent;
using System.Data;
using System.Data.SqlClient;
using CMS.Enums;
using Foundation.Application;
using Helper;
using MySql.Data.MySqlClient;

namespace Foundation.DataAccess.Connections
{
    public class MainConnection : DbConnection
    {
        private MainConnection()
        {
        }

        private static MainConnection _interface;
        public static MainConnection Interface => _interface ??= new MainConnection();

        readonly ConcurrentDictionary<string, string> _connectionString = new ConcurrentDictionary<string, string>();

        protected override string GetDbConnectionBuilderString()
        {
            string sqlConnectName = ConfigHelper.GetAppSetting("currentConnectionStringName");

            if (!_connectionString.ContainsKey(sqlConnectName))
            {
                string sqlConnectionString = ConfigHelper.GetAppSetting("connectionStrings:" + sqlConnectName);
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