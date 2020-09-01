using System;
using System.Data;
using System.Data.SqlClient;
using CMS.Enums;
using Foundation.Application;
using MySql.Data.MySqlClient;

namespace Foundation.DataAccess.Connections
{
    public abstract class DbConnection
    {
        protected abstract string GetDbConnectionBuilderString();

        public DataProvider GetDataProvider()
        {
            string sqlConnectName = ConfigHelper.GetAppSetting("currentConnectionStringName");

            switch (sqlConnectName.ToLower())
            {
                case "sqlserver":
                    return DataProvider.SqlServer;
                case "mysql":
                    return DataProvider.MySql;
                default:
                    throw new Exception("只支持 sqlserver 、 mysql");
            }
        }

        public IDbConnection GetConnection()
        {
            switch (GetDataProvider())
            {
                case DataProvider.SqlServer:
                    return new SqlConnection(GetDbConnectionBuilderString());
                case DataProvider.MySql:
                    return new MySqlConnection(GetDbConnectionBuilderString());
                default:
                    throw new Exception("只支持 sqlserver 、 mysql");
            }
        }
    }
}