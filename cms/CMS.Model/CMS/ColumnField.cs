/**
 * author：created by zw on 2020-07-10 13:51:31
 * email：ntzw.geek@gmail.com
 */

using System.ComponentModel;
using CMS.Enums;
using Dapper.Contrib.Extensions;

namespace Model.CMS
{
	[Table("CMS_ColumnField")]
	public class ColumnField : ModalBase 
	{
		
		[Description("")]
		public string ColumnNum { get; set; }
		
		/// <summary>
		/// 模型字段编号
		/// </summary>
		public string ModelFieldNum { get; set; }
		
		[Description("")]
		public string Name { get; set; }
		
		[Description("")]
		public string Explain { get; set; }
		
		[Description("")]
		public ReactFormItemType OptionType { get; set; }
		
		[Description("")]
		public int DataType { get; set; }
		
		/// <summary>
		/// 配置JSON
		/// </summary>
		public string Options { get; set; }
	}
}