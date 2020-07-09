using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;
using CMS.Enums;
using CMS.React.Component;
using Extension;

namespace CMS.React
{
    public static class ReactForm
    {
        private static readonly Type ComponentType = typeof(BaseComponent);

        #region 实体转前端字段数据

        /// <summary>
        /// 实体转前端字段数据
        /// </summary>
        /// <param name="isUpdate"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<IDictionary<string, object>> ToFormFields<T>(bool isUpdate)
        {
            Type obj = typeof(T);
            List<IDictionary<string, object>> fieldData = new List<IDictionary<string, object>>();

            foreach (var propertyInfo in obj.GetProperties())
            {
                var attr = propertyInfo.GetCustomAttribute(ComponentType) as BaseComponent;
                if (attr == null) continue;
                if (attr.UpdateShow == false && isUpdate) continue;

                var field = new ExpandoObject();
                List<Rule> rules = new List<Rule>();

                attr.Disabled = attr.Disabled || (attr.UpdateDisabled && isUpdate);
                
                field.TryAdd("label", attr.Title);
                field.TryAdd("name", ToNameLower(propertyInfo.Name));
                field.TryAdd("dataAction", attr.DataAction);
                field.TryAdd("Split", attr.Split);

                if (attr is InputAttribute input)
                    SetInput(field, input);
                else if (attr is PasswordAttribute password)
                    SetPassword(field, password);
                else if (attr is SelectAttribute select)
                    SetSelect(field, select);
                else if (attr is TextAreaAttribute textarea)
                    SetTextArea(field, textarea);
                else if (attr is CascaderAttribute cascader)
                    SetCascader(field, cascader);
                else if (attr is SwitchAttribute switchAttribute)
                    SetSwitch(field, switchAttribute);

                if (attr.Required)
                {
                    rules.Add(new Rule
                    {
                        Required = true,
                        Message = $"{attr.Title} 为必填项，请认真填写"
                    });
                }

                rules.AddRange(GetRules(attr));
                field.TryAdd("rules", rules);
                fieldData.Add(field);
            }

            return fieldData;
        }

        /// <summary>
        /// 获取字段需要验证信息
        /// </summary>
        /// <param name="attr"></param>
        /// <returns></returns>
        private static List<Rule> GetRules(BaseComponent attr)
        {
            List<Rule> rules = new List<Rule>();
            if (attr.RegularTypes == null || attr.RegularTypes.Length <= 0) return rules;

            foreach (var regularType in attr.RegularTypes)
            {
                switch (regularType)
                {
                    case RegularType.SqlField:
                        rules.Add(new Rule
                        {
                            Pattern = RegularExtension.SqlField.ToString(),
                            Message = $"请填写正确的 {attr.Title}"
                        });
                        break;
                }
            }

            return rules;
        }

        private static void SetSwitch(ExpandoObject field, SwitchAttribute switchAttribute)
        {
            field.TryAdd("type", ReactFormItemType.Switch);
            field.TryAdd("valuePropName", "checked");
            field.TryAdd("switch", new
            {
                switchAttribute.CheckedChildren,
                switchAttribute.UnCheckedChildren,
                switchAttribute.Disabled,
            });
        }

        private static void SetCascader(ExpandoObject field, CascaderAttribute cascader)
        {
            field.TryAdd("type", ReactFormItemType.Cascader);
            field.TryAdd("cascader", new
            {
                cascader.ExpandTrigger,
                cascader.ChangeOnSelect,
                cascader.Disabled,
            });
        }

        private static void SetTextArea(ExpandoObject field, TextAreaAttribute textarea)
        {
            field.TryAdd("type", ReactFormItemType.TextArea);
            field.TryAdd("textarea", new
            {
                textarea.Placeholder,
                textarea.Rows,
                textarea.MaxLength,
                textarea.Disabled,
            });
        }

        private static void SetSelect(ExpandoObject field, SelectAttribute @select)
        {
            field.TryAdd("type", ReactFormItemType.Select);
            field.TryAdd("dataAction", select.DataAction);

            var selectOptions = new ExpandoObject();
            selectOptions.TryAdd("Mode", select.Mode);
            selectOptions.TryAdd("Placeholder", select.Placeholder);
            selectOptions.TryAdd("Disabled", select.Disabled);

            if (select.AllowClear != null)
                selectOptions.TryAdd("AllowClear", select.AllowClear);

            if (select.ShowSearch != null)
                selectOptions.TryAdd("ShowSearch", select.ShowSearch);

            field.TryAdd("select", selectOptions);
        }

        private static void SetPassword(ExpandoObject field, PasswordAttribute password)
        {
            field.TryAdd("type", ReactFormItemType.Password);
            field.TryAdd("password", new
            {
                password.MaxLength,
                password.Placeholder,
                password.Disabled,
            });
        }

        private static void SetInput(ExpandoObject field, InputAttribute input)
        {
            field.TryAdd("type", ReactFormItemType.Input);
            field.TryAdd("input", new
            {
                input.MaxLength,
                input.Placeholder,
                input.AddonAfter,
                input.AddonBefore,
                input.Disabled,
            });
        }

        private static string ToNameLower(string name)
        {
            if (name.IsEmpty()) return "";

            var first = name.Substring(0, 1).ToLower();
            var el = name.Length > 1 ? name.Substring(1, name.Length - 1) : "";
            return first + el;
        }

        #endregion

        #region 设置值

        /// <summary>
        /// 设置模型值，将新数据实体里的数据设置到旧实体里
        /// </summary>
        /// <param name="oldModel">旧数据实体</param>
        /// <param name="newModel">新数据实体</param>
        /// <param name="isUpdate">是否更新</param>
        /// <typeparam name="T"></typeparam>
        public static void SetEditModelValue<T>(T oldModel, T newModel, bool isUpdate = false)
        {
            foreach (var propertyInfo in typeof(T).GetProperties())
            {
                var attr = propertyInfo.GetCustomAttribute(typeof(BaseComponent)) as BaseComponent;
                if (attr == null || (isUpdate && (!attr.UpdateShow || attr.UpdateDisabled))) continue;

                object newValue = propertyInfo.GetValue(newModel);
                propertyInfo.SetValue(oldModel, newValue);
            }
        }

        #endregion
    }

    public class Rule
    {
        /// <summary>
        /// 必填
        /// </summary>
        public bool Required { get; set; }

        /// <summary>
        /// 提示信息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 正则
        /// </summary>
        public string Pattern { get; set; }
    }
}