/**
 * author：created by zw on 2020-07-08 15:54:53
 * email：ntzw.geek@gmail.com
 */

using System.Threading.Tasks;
using Dapper;
using DataAccess.Interface.CMS;
using Model.CMS;

namespace DataAccess.SqlServer.CMS
{
    public class ModelTableDapper : DefaultDataAccess<ModelTable>, IModelTableDapper
    {
        public Task<int> CreateTable(string tableName)
        {
            return Connection().ExecuteAsync(CreateTableSql(tableName));
        }

        public Task<int> CreateCategoryTable(string tableName)
        {
	        return Connection().ExecuteAsync(CreateCategoryTableSql(tableName));
        }

        /// <summary>
        /// 获取创建模型实体分类表SQL语句
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        private string CreateCategoryTableSql(string tableName)
        {
            return $@"CREATE TABLE [dbo].[{tableName}](
								[Id] [int] IDENTITY(1,1) NOT NULL,
								[Num] [nvarchar](50) NULL,
								[CreateDate] [datetime] NULL,
								[UpdateDate] [datetime] NULL,
								[CreateAccountNum] [nvarchar](50) NULL,
								[UpdateAccountNum] [nvarchar](50) NULL,
								[IsDel] [bit] NULL,
								[Status] [int] NULL,
								[SiteNum] [nvarchar](50) NOT NULL,
								[ColumnNum] [nvarchar](50) NOT NULL,
								[Name] [nvarchar](200) NOT NULL,
								[ParentNum] [nvarchar](50) NULL,
							 CONSTRAINT [PK_{tableName}] PRIMARY KEY CLUSTERED 
							(
								[Id] ASC
							)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
							) ON [PRIMARY]";
        }

        /// <summary>
        /// 获取创建模型实体表SQL语句
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        private string CreateTableSql(string tableName)
        {
            return $@"CREATE TABLE [dbo].[{tableName}](
							[Id] [int] IDENTITY(1,1) NOT NULL,
							[Num] [nvarchar](50) NULL,
							[CreateDate] [datetime] NULL,
							[UpdateDate] [datetime] NULL,
							[CreateAccountNum] [nvarchar](50) NULL,
							[UpdateAccountNum] [nvarchar](50) NULL,
							[IsDel] [bit] NULL,
							[Status] [int] NULL,
							[SiteNum] [nvarchar](50) NULL,
							[ColumnNum] [nvarchar](50) NULL,
							[CategoryNum] [nvarchar](50) NULL,
							[SeoTitle] [nvarchar](500) NULL,
							[SeoKeyword] [nvarchar](2000) NULL,
							[SeoDesc] [nvarchar](2000) NULL,
							[ClickCount] [int] NULL,
							[IsTop] [bit] NULL,
						 CONSTRAINT [PK_{tableName}] PRIMARY KEY CLUSTERED 
						(
							[Id] ASC
						)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
						) ON [PRIMARY]";
        }

        public Task<ModelTable> GetByTableName(string tableName)
        {
            string sql = $"SELECT * FROM {GetTableName()} WHERE TableName = @TableName ";
            return Connection().QueryFirstOrDefaultAsync<ModelTable>(sql, new {TableName = tableName});
        }
    }
}