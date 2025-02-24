using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmdParser
{
    public partial struct Value
    {
        private static bool TryConvertToArray(Type targetType, Value origin, out object? outValue )
        {
            _Log($"尝试转换为: 数组类型");
            if (!targetType.IsArray)
            {
                outValue = null;
                return false;
            }
            _Log($"时数组类型");

            Value[] values = origin.Array;
            Type elementType = targetType.GetElementType();
            //解析元素组
            List<object> elements = new List<object>();
            foreach (var item in values)
            {
                var rs = item.ConvertTo(elementType);
                if (rs != null)
                    elements.Add(rs);
            }

            Array array = System.Array.CreateInstance(elementType, elements.Count);
            for (int i = 0; i < elements.Count; i++)
            {
                array.SetValue(elements[i], i);
            }
            outValue = array;
            return true;
        }
    }
}
