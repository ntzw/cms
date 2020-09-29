/**
 * author：created by zw on 2020-07-08 15:54:52
 * email：ntzw.geek@gmail.com
 */

using System.ComponentModel;
using CMS.Enums;
using Dapper.Contrib.Extensions;
using Foundation.DataAccess;

namespace CMS.Modules.Content.Abstractions.Model
{
	[Table("CMS_ModelField")]
	public class ModelField : ModalBase 
	{
		
		[Description("")]
		public string ModelNum { get; set; }
		
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