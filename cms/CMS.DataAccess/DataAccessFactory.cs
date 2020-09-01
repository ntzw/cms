using System;
using System.Collections.Concurrent;
using System.Linq;
using Extension;
using Foundation.Application;
using Foundation.DataAccess.Interface;

namespace DataAccess
{
    /// <summary>
    /// 数据库工厂
    ///
    /// 用于动态生成数据层访问实例
    ///
    /// 切换数据库，配置appsettings.json 的 currentConnectionStringName 节点即可
    /// </summary>
    public static class DataAccessFactory
    {
        private static readonly ConcurrentDictionary<string, object> Dappers =
            new ConcurrentDictionary<string, object>();

        public static T GetInstance<T>() where T : IDataAccess
        {
            string currentSqlName = ConfigHelper.GetAppSetting("currentConnectionStringName");
            if (currentSqlName.IsEmpty()) throw new Exception("请设置默认连接字符串名称");

            var interfaceType = typeof(T);
            string cacheName = "Dapper_" + interfaceType.FullName;

            if (!Dappers.ContainsKey(cacheName) || Dappers[cacheName] == null)
            {
                var classType = GetInstanceClassType(interfaceType, currentSqlName);
                if (classType == null) throw new Exception("未找到相应的类");

                Dappers[cacheName] = (T) Activator.CreateInstance(classType);
            }

            return (T) Dappers[cacheName];
        }

        private static Type GetInstanceClassType(Type interfaceType, string currentSqlName)
        {
            return interfaceType.Assembly.GetTypes().ToList().Find(temp =>
                string.Equals(temp.FullName, GetInstanceClassTypeFullName(interfaceType, currentSqlName),
                    StringComparison.OrdinalIgnoreCase));
        }

        private static string GetInstanceClassTypeFullName(Type interfaceType, string currentSqlName)
        {
            if (interfaceType.Namespace == null) throw new Exception("未获取到命名空间");

            string classTypeNamespace =
                interfaceType.Namespace.ToLower().Replace("interface", currentSqlName.ToLower());
            string classTypeName = interfaceType.Name.ToLower().TrimStart('i');

            return $"{classTypeNamespace}.{classTypeName}";
        }
    }
}