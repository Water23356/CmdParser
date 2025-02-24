using System.Text;

namespace CmdParser
{
    /// <summary>
    /// 值容器
    /// </summary>
    public partial struct Value
    {
        private object? _value;

        /// <summary>
        /// 是否是有效值(用于无效值检测, 如果没有填写value, 该值为false)
        /// </summary>
        public bool active { get; set; }

        /// <summary>
        /// 实际值
        /// </summary>
        public object? value
        {
            get => _value;
            set
            {
                _value = value;
            }
        }

        /// <summary>
        /// 转换为字符串
        /// </summary>
        public string String
        {
            get => value?.ToString() ?? string.Empty;
        }

        /// <summary>
        /// 获取或者转换为int类型, null->0
        /// </summary>
        public int Int
        {
            get
            {
                if (_value is null) return 0;
                if (IsNumberType(_value))
                {
                    return ConvertToNumberType<int>(_value);
                }

                if (int.TryParse(String, out var value))
                    return value;
                throw new ConvertException($"转换为 int 类型失败:  stringValue={String}  realType={this.value?.GetType()}");
            }
        }

        /// <summary>
        /// 获取或者转换为float类型, null->0
        /// </summary>
        public float Float
        {
            get
            {
                if (_value is null) return 0;
                if (IsNumberType(_value))
                {
                    return ConvertToNumberType<int>(_value);
                }

                if (float.TryParse(String, out var value))
                    return value;
                throw new ConvertException($"转换为 float 类型失败:  stringValue={String}  realType={this.value?.GetType()}");
            }
        }

        /// <summary>
        /// 获取或者转换为bool类型, null->false
        /// </summary>
        public bool Bool
        {
            get
            {
                if (_value is null) return false;
                if (_value is bool) return (bool)_value;

                var str = (value?.ToString() ?? string.Empty).ToLower();
                if (str == "true") return true;
                if (str == "false") return false;
                throw new ConvertException($"转换为 bool 类型失败:  stringValue={String}  realType={this.value?.GetType()}");
            }
        }

        /// <summary>
        /// 获取或者转换为数组类型, null->[]
        /// </summary>
        public Value[] Array
        {
            get
            {
                if (_value is null) return new Value[0];

                if (value is Value[])
                    return (Value[])value;
                if (value is ICollection<Value>)
                    return ((ICollection<Value>)_value).ToArray();

                throw new ConvertException($"转换为 数组 类型失败:  stringValue={String}  realType={this.value?.GetType()}");
            }
        }

        /// <summary>
        /// 获取或者转换为字典类型, null->{}
        /// </summary>
        public Dictionary<string, Value> Dic
        {
            get
            {
                if (_value is null)
                    return new Dictionary<string, Value>();
                if (value is Dictionary<string, Value>)
                    return (Dictionary<string, Value>)value;
                throw new ConvertException($"转换为 字典 类型失败:  stringValue={String}  realType={this.value?.GetType()}");
            }
        }

        /// <summary>
        /// 获取指令串, 转换失败则返回空串
        /// </summary>
        public CommandString Cstr
        {
            get
            {
                if (value is CommandString)
                    return (CommandString)value;
                return new CommandString();
            }
        }

        /// <summary>
        /// 转换为指令对象
        /// </summary>
        public Command Command
        {
            get
            {
                if (value is Command)
                    return (Command)value;
                throw new ConvertException($"转换为 Command 类型失败:  stringValue={String}  realType={this.value?.GetType()}");
            }
        }

        /// <summary>
        /// 转换为键值对
        /// </summary>
        public KvPair Pair
        {
            get
            {
                if (value is KvPair)
                    return (KvPair)value;
                throw new ConvertException($"转换为 KeyValuePiar 类型失败:  stringValue={String}  realType={this.value?.GetType()}");
            }
        }

        /// <summary>
        /// 转换为参数对象
        /// </summary>
        public Arg Arg
        {
            get
            {
                if (value is Arg) return (Arg)value;
                throw new ConvertException($"转换为 Arg 类型失败:  stringValue={String}  realType={this.value?.GetType()}");
            }
        }

        public Value()
        {
            active = false;
        }

        public Value(object? value)
        {
            this.value = value;
            active = true;
        }

        public override string ToString()
        {
            if (_value is Value[])
            {
                Value[] array = (Value[])_value;
                StringBuilder sb = new StringBuilder();
                sb.Append("[");
                foreach (var v in array)
                {
                    sb.Append(v.ToString());
                    sb.Append(',');
                }
                sb.Remove(sb.Length - 1, 1);
                sb.Append("]");
                return sb.ToString();
            }
            else if (_value is Dictionary<string, Value>)
            {
                Dictionary<string, Value> dic = (Dictionary<string, Value>)_value;
                StringBuilder sb = new StringBuilder();
                sb.Append('{');
                foreach (var pair in dic)
                {
                    sb.Append($"{pair.Key}:{pair.Value}");
                    sb.Append(',');
                }
                sb.Remove(sb.Length - 1, 1);
                sb.Append("}");
                return sb.ToString();
            }
            return value?.ToString() ?? "<null>";
        }

        /// <summary>
        /// 返回一个无效的空值
        /// </summary>
        public static Value Null { get { return new Value(); } }

        /// <summary>
        /// 返回一个有效的空值
        /// </summary>
        public static Value Empty { get { return new Value(null); } }
    }

    /// <summary>
    /// 键值对
    /// </summary>
    public class KvPair
    {
        public string key = string.Empty;
        public Value value;

        public void Clear()
        {
            key = string.Empty;
            value = new Value();
        }

        /// <summary>
        /// 是否为空
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                return (key == string.Empty);
            }
        }

        public override string ToString()
        {
            return $"{{{key}:{value}}}";
        }
    }
}