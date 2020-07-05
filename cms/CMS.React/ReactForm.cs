using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using CMS.Enums;
using CMS.React.Component;
using Extension;

namespace CMS.React
{
    public static class ReactForm
    {
        private static readonly Type ComponentType = typeof(BaseComponent);

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

                field.TryAdd("label", attr.Title);
                field.TryAdd("name", ToNameLower(propertyInfo.Name));

                if (attr is InputAttribute input)
                    SetInput(field, input);
                else if (attr is PasswordAttribute password)
                    SetPassword(field, password);
                else if (attr is SelectAttribute select)
                    SetSelect(field, select);

                if (attr.Required)
                {
                    rules.Add(new Rule
                    {
                        Required = true,
                        Message = $"{attr.Title} 为必填项，请认真填写"
                    });
                }

                field.TryAdd("rules", rules);
                fieldData.Add(field);
            }

            return fieldData;
        }

        private static void SetSelect(ExpandoObject field, SelectAttribute @select)
        {
            field.TryAdd("type", ReactFormItemType.Select);
            field.TryAdd("dataAction", select.DataAction);
            field.TryAdd("select", new
            {
                select.Mode,
                select.AllowClear,
                select.ShowSearch,
                select.Placeholder
            });
        }

        private static void SetPassword(ExpandoObject field, PasswordAttribute password)
        {
            field.TryAdd("type", ReactFormItemType.Password);
            field.TryAdd("password", new
            {
                password.MaxLength,
                password.Placeholder,
            });
        }

        private static void SetInput(ExpandoObject field, InputAttribute input)
        {
            field.TryAdd("type", ReactFormItemType.Input);
            field.TryAdd("input", new
            {
                input.MaxLength,
                input.Placeholder,
            });
        }

        private static string ToNameLower(string name)
        {
            if (name.IsEmpty()) return "";

            var first = name.Substring(0, 1).ToLower();
            var el = name.Length > 1 ? name.Substring(1, name.Length - 1) : "";
            return first + el;
        }
    }

    public class Rule
    {
        public bool Required { get; set; }

        public string Message { get; set; }

        public string Pattern { get; set; }
    }
}