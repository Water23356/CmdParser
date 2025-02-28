////#define TEST

using System.Globalization;

namespace CmdParser
{
    public partial struct Value
    {
        /// <summary>
        /// 如果这是一个嵌套的Value容器, 可以用来转换为其他数据类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T? ConvertTo<T>()
        {
            return (T?)ConvertTo(typeof(T));
        }

        /// <summary>
        /// 转换为目标类型
        /// </summary>
        /// <param name="targetType"></param>
        /// <returns></returns>
        /// <exception cref="ConvertException"></exception>
        public object? ConvertTo(Type targetType)
        {
            _Log($"尝试转换为类型: {targetType.Name}");

            //基本类型转换: string,bool,object
            object? result;
            if (TryConvertToNormalValue(targetType, this, out result))
            {
                _Log($"转换成功: {result}");
                return result;
            }
            //数字类型转换
            if (TryConvertToNumber(targetType, this, out result))
            {
                _Log($"转换成功: {result}");
                return result;
            }
            //数组类型转换
            if (TryConvertToArray(targetType, this, out result))
            {
                _Log($"转换成功: {result}");
                return result;
            }
            //字典类型转换
            if(TryConvertToDictionary(targetType, this,out result))
            {
                _Log($"转换成功: {result}");
                return result;
            }
            //集合类型转换
            if (TryConvertToCollection(targetType, this, out result))
            {
                _Log($"转换成功: {result}");
                return result;
            }
            //一般类型转换
            return ConvertToObject(targetType, this);
        }

        private static bool TryConvertToNormalValue(Type targetType, Value origin, out object? outValue)
        {
            _Log($"尝试转换为: 基础类型");
            //基本类型转换
            if (targetType == typeof(object))//object为目标类型则会直接取值返回
            {
                outValue = origin._value;
                return true;
            }
            if (targetType == typeof(bool))
            {
                outValue = origin.Bool;
                return true;
            }
            if (targetType == typeof(string))
            {
                outValue = origin.String;
                return true;
            }

            outValue = null;
            return false;
        }

        /// <summary>
        /// 判断该对象值是否为数字类型
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static bool IsNumberType(object? value)
        {
            if (value == null)
            {
                return false;
            }
            return IsNumericType(value.GetType());
        }

        /// <summary>
        /// 转换为数字类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        private static T ConvertToNumberType<T>(object value)
        {
            try
            {
                // 尝试使用 Convert.ChangeType 进行转换
                return (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Cannot convert {value} to type {typeof(T).Name}.", ex);
            }
        }

        /// <summary>
        /// 判断是否为数字类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsNumericType(Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;

                default:
                    return false;
            }
        }
        /// <summary>
        /// 调试用
        /// </summary>
        /// <param name="message"></param>
        private static void _Log(string message)
        {
#if TEST
            MessageOutput.BroadcastLine(message);
#endif
        }
    }
}