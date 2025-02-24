using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CmdParser
{
    public partial struct Value
    {
        /// <summary>
        /// 解析为一般对象
        /// </summary>
        /// <param name="targetType"></param>
        /// <param name="origin"></param>
        /// <param name="outValue"></param>
        /// <returns></returns>
        private static object? ConvertToObject(Type targetType, Value origin)
        {
            var dic = origin.Dic;
            var instance = Activator.CreateInstance(targetType);
            //获取所有可写属性
            var properties = GetWritableProperties(targetType);
            var fields = GetWritableFields(targetType);
            foreach (var property in properties)
            {
                if (dic.TryGetValue(property.Name, out var value))
                {
                    //将字典的值转换为对应数据类型
                    var rs = value.ConvertTo(property.PropertyType);
                    property.SetValue(instance, rs);
                }
            }
            foreach (var field in fields)
            {
                if (dic.TryGetValue(field.Name, out var value))
                {
                    //将字典的值转换为对应数据类型
                    var rs = value.ConvertTo(field.FieldType);
                    field.SetValue(instance, rs);
                }
            }
            return instance;
        }


        /// <summary>
        /// 获取所有可写字段
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static FieldInfo[] GetWritableFields(Type type)
        {
            // 获取所有可写的字段（包括私有字段）
            return type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly)
                       .Where(field => field.IsInitOnly == false) // 排除只读字段
                       .ToArray();
        }

        /// <summary>
        /// 获取所有可写属性
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static PropertyInfo[] GetWritableProperties(Type type)
        {
            // 获取所有可写的属性
            return type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly)
                       .Where(property => property.CanWrite)
                       .ToArray();
        }
    }
}
