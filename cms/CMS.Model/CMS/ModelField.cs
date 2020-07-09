/**
 * author：created by zw on 2020-07-08 15:54:52
 * email：ntzw.geek@gmail.com
 */

using System.ComponentModel;
using CMS.Enums;
using Dapper.Contrib.Extensions;

namespace Model.CMS
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
		
	}
}