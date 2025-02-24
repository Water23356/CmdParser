using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CmdParser
{
    public partial struct Value
    {
        /// <summary>
        /// 解析为字典类型
        /// </summary>
        /// <param name="targetType"></param>
        /// <param name="origin"></param>
        /// <param name="outValue"></param>
        /// <returns></returns>
        private static bool TryConvertToDictionary(Type targetType, Value origin, out object? outValue)
        {
            _Log($"尝试转换为: 字典类型");
            if (!IsKeyValuePairCollection(targetType))
            {
                outValue = null;
                return false;
            }
            _Log($"是字典类型");

            object? instance = Activator.CreateInstance(targetType);

            var dic = origin.Dic;
            foreach(var kvp in dic )
            {
                AddKeyValuePair(instance, kvp.Key, kvp.Value.value);
            }

            outValue = instance;
            return true;
        }
        public static void AddKeyValuePair(object collectionInstance, object key, object value)
        {
            Type collectionType = collectionInstance.GetType();
            MethodInfo addMethod = collectionType.GetMethod("Add");
            if (addMethod == null)
                throw new InvalidOperationException("The collection type does not support adding key-value pairs.");

            addMethod.Invoke(collectionInstance, new[] { key, value });
        }
        public static bool IsKeyValuePairCollection(Type type)
        {
            // 检查是否实现了非泛型 IDictionary 接口
            if (typeof(IDictionary).IsAssignableFrom(type))
                return true;

            // 检查是否实现了泛型 IDictionary<TKey, TValue> 接口
            if (type.IsGenericType)
            {
                Type[] interfaces = type.GetInterfaces();
                foreach (Type iface in interfaces)
                {
                    if (iface.IsGenericType && iface.GetGenericTypeDefinition() == typeof(IDictionary<,>))
                        return true;
                }
            }
            return false;
        }
    }
}
