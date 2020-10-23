/**
 * author：created by zw on 2020-07-08 15:54:53
 * email：ntzw.geek@gmail.com
 */

using System.Threading.Tasks;
using CMS.Modules.Content.Abstractions.Interface.Dapper;
using CMS.Modules.Content.Abstractions.Model;
using Dapper;
using Foundation.DataAccess;
using Foundation.DataAccess.Interface;

namespace CMS.Modules.Content.DataAccess.MySql
{
    public class ModelTableDapper : DefaultDataAccess<ModelTable>, IModelTableDapper
    {
        public ModelTableDapper(IPageSqlHelper pageSqlHelper) : base(pageSqlHelper)
        {
        }

        public Task<int> CreateTable(string tableName)
        {
            return Connection().ExecuteAsync(CreateTableSql(tableName));
        }

        public Task<int> CreateCategoryTable(string tableName)
        {
            return Connection().ExecuteAsync(CreateCategoryTableSql(tableName));
        }

        public Task<ModelTable> GetByTableName(string tableName)
        {
            var sql = $"SELECT * FROM {GetTableName()} WHERE TableName = @TableName ";
            return Connection().QueryFirstOrDefaultAsync<ModelTable>(sql, new {TableName = tableName});
        }

        /// <summary>
        ///     获取创建模型实体分类表SQL语句
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        private string CreateCategoryTableSql(string tableName)
        {
            return $@"CREATE TABLE `{tableName}`  (
                          `Id` int NOT NULL AUTO_INCREMENT,
                          `Num` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL,
                          `CreateDate` timestamp(0) NULL DEFAULT NULL,
                          `UpdateDate` timestamp(0) NULL DEFAULT NULL,
                          `CreateAccountNum` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL,
                          `UpdateAccountNum` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL,
                          `IsDel` tinyint(1) NULL DEFAULT NULL,
                          `Status` int NULL DEFAULT NULL,
                          `SiteNum` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL,
                          `ColumnNum` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL,
                          `Name` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL,
                          `ParentNum` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL,
                          PRIMARY KEY (`Id`) USING BTREE
                        ) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = Dynamic;";
        }

        /// <summary>
        ///     获取创建模型实体表SQL语句
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        private string CreateTableSql(string tableName)
        {
            return $@"CREATE TABLE `{tableName}`  (
					  `Id` int NOT NULL AUTO_INCREMENT,
					  `Num` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL,
					  `CreateDate` timestamp(0) NULL DEFAULT NULL,
					  `UpdateDate` timestamp(0) NULL DEFAULT NULL,
					  `CreateAccountNum` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL,
					  `UpdateAccountNum` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL,
					  `IsDel` tinyint(1) NULL DEFAULT NULL,
					  `Status` int NULL DEFAULT NULL,
                      `SiteNum`          varchar(50)  CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL,
                      `ColumnNum`        varchar(50)  CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL,
                      `CategoryNum`      varchar(50)  CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL,
                      `SeoTitle`         varchar(500)  CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL,
                      `SeoKeyword`       text         null,
                      `SeoDesc`          text         null,
                      `ClickCount`       int          null,
					  PRIMARY KEY (`Id`) USING BTREE
					) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = Dynamic;";
        }
    }
}