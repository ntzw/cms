/**
 * author：created by zw on 2020-06-20 14:28:17
 * email：ntzw.geek@gmail.com
 */

using System.ComponentModel;
using Dapper.Contrib.Extensions;

namespace Model.CMS
{
	[Table("CMS_Column")]
	public class Column : ModalBase 
	{
		
		[Description("")]
		public string Name { get; set; }
		
		[Description("")]
		public string ParentNum { get; set; }
		
		[Description("")]
		public string SiteNum { get; set; }
		
	}
}