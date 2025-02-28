using System.Globalization;
using System.Reflection;

namespace CmdParser
{
    public partial struct Value
    {
        private static bool TryConvertToNumber(Type targetType, Value origin, out object? outValue)
        {
            _Log($"尝试转换为: 数字类型");
            if (!IsNumericType(targetType))
            {
                outValue = null;
                return false;
            }
            _Log($"是数字类型");

            if (origin._value is null)
            {
                throw new ConvertException($"无法将 null 类型转换为 {targetType.Name}");
            }
            if (IsNumberType(origin._value))//如果目标类型和实际类型都是 数字类型
            {
                outValue = _ConvertToNumberType(origin.value!, targetType);
                return true;
            }
            else if (origin._value is string)
            {
                outValue = _StringParseToNumber(targetType, (string)origin._value);
                if (outValue == null)
                    throw new ConvertException($"无法将 '{origin._value}' 字符串转换为 {targetType.Name}");
                return true;
            }
            else
            {
                throw new ConvertException($"无法将 '{origin._value}' 字符串转换为 {targetType.Name}");
            }
        }

        /// <summary>
        /// 转换为数字类型
        /// </summary>
        /// <param name="value"原始值></param>
        /// <param name="type">目标数字类型</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        private static object _ConvertToNumberType(object value, Type type)
        {
            try
            {
                // 尝试使用 Convert.ChangeType 进行转换
                return Convert.ChangeType(value, type, CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Cannot convert {value} to type {type.Name}.", ex);
            }
        }

        /// <summary>
        /// 将字符串解析为一个数字类型
        /// </summary>
        /// <param name="type">应当是一种数字类型</param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static object? _StringParseToNumber(Type type, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            // 检查是否为数字类型
            if (!IsNumericType(type))
            {
                return null;
            }

            // 尝试调用 Parse 方法
            try
            {
                MethodInfo? parseMethod = type.GetMethod("Parse", new[] { typeof(string), typeof(IFormatProvider) });
                if (parseMethod != null)
                {
                    return parseMethod.Invoke(null, new object[] { value, CultureInfo.InvariantCulture });
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }
    }
}