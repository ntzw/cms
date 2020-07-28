using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Extension;

namespace Helper
{
    public class ContentEditSqlHelper
    {
        private readonly string _tableName;
        readonly List<string> _editField = new List<string>();
        readonly ExpandoObject _editValue = new ExpandoObject();

        public ContentEditSqlHelper(string tableName)
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