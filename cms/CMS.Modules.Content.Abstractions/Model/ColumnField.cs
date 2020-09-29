/**
 * author：created by zw on 2020-07-10 13:51:31
 * email：ntzw.geek@gmail.com
 */

using System.ComponentModel;
using CMS.Enums;
using Dapper.Contrib.Extensions;
using Foundation.DataAccess;

namespace CMS.Modules.Content.Abstractions.Model
{
    [Table("CMS_ColumnField")]
    public class ColumnField : ModalBase
    {
        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }

        [Description("")] public string ColumnNum { get; set; }

        /// <summary>
        /// 模型字段编号
        /// </summary>
        public string ModelFieldNum { get; set; }

        [Description("")] public string Name { get; set; }

        [Description("")] public string Explain { get; set; }

        [Description("")] public ReactFormItemType OptionType { get; set; }

        [Description("")] public int DataType { get; set; }

        /// <summary>
        /// 配置JSON
        /// </summary>
        public string Options { get; set; }
    }
}