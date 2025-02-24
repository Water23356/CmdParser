using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CmdParser
{
    public partial struct Value
    {
        /// <summary>
        /// 解析为集合类型, 例如 List, 存在 Add 添加方法的集合
        /// </summary>
        /// <param name="targetType"></param>
        /// <param name="origin"></param>
        /// <param name="outValue"></param>
        /// <returns></returns>
        private static bool TryConvertToCollection(Type targetType, Value origin, out object? outValue)
        {
            _Log($"尝试转换为: 集合类型");
            if (!IsCollectionType(targetType))
            {
                outValue = null;
                return false;
            }
            _Log($"是集合类型");

            //获取集合元素类型
            Type elementType;
            if (targetType.IsGenericType)
            {
                elementType = targetType.GetGenericArguments()[0];
                // 获取泛型参数类型
            }
            else
            {
                // 非泛型集合，默认为 object 类型
                elementType = typeof(object);
            }

            //解析元素组
            var array = origin.Array;
            List<object> elements = new List<object>();
            foreach (var item in array)
            {
                var rs = item.ConvertTo(elementType);
                if (rs != null)
                    elements.Add(rs);
            }

            //创建容器实例
            object? collectionInstance = Activator.CreateInstance(targetType, array.Length);
            MethodInfo? addMethod = targetType.GetMethod("Add");
            //填入元素
            foreach (var element in elements)
            {
                //转换元素类型
                object convertedElement = Convert.ChangeType(element, elementType);
                addMethod.Invoke(collectionInstance, new object[] { convertedElement });
            }

            outValue = collectionInstance;
            return true;
        }
        /// <summary>
        /// 判断是否为集合类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsCollectionType(Type type)
        {
            // 排除字符串类型（特殊情况）
            if (type == typeof(string))
            {
                return false;
            }

            // 检查是否实现了 IEnumerable 接口
            if (typeof(IEnumerable).IsAssignableFrom(type))
            {
                // 如果是泛型类型，进一步检查是否是泛型集合类型
                if (type.IsGenericType)
                {
                    Type[] genericInterfaces = type.GetInterfaces()
                                                   .Where(i => i.IsGenericType)
                                                   .Select(i => i.GetGenericTypeDefinition())
                                                   .ToArray();

                    // 检查是否实现了 IEnumerable<T>
                    return genericInterfaces.Contains(typeof(IEnumerable<>));
                }

                // 非泛型集合（如 Array 或非泛型集合类型）
                return true;
            }

            // 不是集合类型
            return false;
        }


    }
}
