using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using CMS.Enums;
using Extension;
using Helper;
using Model.CMS;
using Newtonsoft.Json.Linq;

namespace Service.CMS
{
    public class DynamicTableSqlHelper
    {
        private readonly string _tableName;
        readonly List<string> _editField = new List<string>();
        readonly ExpandoObject _editValue = new ExpandoObject();

        public DynamicTableSqlHelper(string tableName)
        {
            _tableName = tableName;
        }

        /// <summary>
        /// 添加字段和数据
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="value"></param>
        public void AddFieldAndValue(string fileName, object value)
        {
            string tempFieldName = fileName.ToFieldNameLower();
            if (!_editField.Contains(tempFieldName))
            {
                _editField.Add(tempFieldName);
                _editValue.TryAdd(tempFieldName, value);
            }
        }

        /// <summary>
        /// 添加栏目表单数据
        /// </summary>
        /// <param name="fields"></param>
        /// <param name="form"></param>
        public void SetJObjectFormData(IEnumerable<ColumnField> fields, JObject form)
        {
            foreach (var columnField in fields)
            {
                var formFieldName = columnField.Name.ToFieldNameLower();
                var value = form.ContainsKey(formFieldName) ? form[formFieldName] : "";
                
                switch (columnField.OptionType)
                {
                    case ReactFormItemType.Input:
                    case ReactFormItemType.Editor:
                    case ReactFormItemType.TextArea:
                    case ReactFormItemType.Select:
                    case ReactFormItemType.Upload:
                    case ReactFormItemType.Tags:
                    case ReactFormItemType.Cascader:
                        AddFieldAndValue(columnField.Name, value.ToStr());
                        break;
                    case ReactFormItemType.Password:
                        AddFieldAndValue(columnField.Name, Md5Helper.GetMD5_32(value.ToStr()));
                        break;
                    case ReactFormItemType.Switch:
                        break;
                    case ReactFormItemType.Radio:
                        break;
                    case ReactFormItemType.CheckBox:
                        break;
                    case ReactFormItemType.DataPicker:
                        break;
                    case ReactFormItemType.RangePicker:
                        break;
                    case ReactFormItemType.Region:
                        break;
                    default:
                        throw new Exception("无处理分类逻辑");
                }
            }
        }

        public string GetUpdateSql(int id)
        {
            _editValue.TryAdd("Id", id);
            return $@"UPDATE [{_tableName}] 
                      SET {string.Join(",", _editField.Select(temp => $"[{temp}] = @{temp}"))} 
                      WHERE Id = @Id";
        }

        public string GetAddSql()
        {
            return $@"INSERT INTO [{_tableName}] ([{string.Join("],[", _editField)}]) 
                      VALUES ({string.Join(",", _editField.Select(temp => $"@{temp}"))}) ";
        }

        public ExpandoObject GetValue()
        {
            return _editValue;
        }
    }
}