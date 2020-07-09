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
            string sql = $@"CREATE TABLE [dbo].[{tableName}](
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
							 CONSTRAINT [PK_{tableName}] PRIMARY KEY CLUSTERED 
							(
								[Id] ASC
							)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
							) ON [PRIMARY]";
            return Connection().ExecuteAsync(sql);
        }

        public Task<ModelTable> GetByTableName(string tableName)
        {
            string sql = $"SELECT * FROM {GetTableName()} WHERE TableName = @TableName ";
            return Connection().QueryFirstOrDefaultAsync<ModelTable>(sql, new {TableName = tableName});
        }
    }
}