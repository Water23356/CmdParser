//#define TEST

using CmdParser.Define;
using System.Collections;

namespace CmdParser
{
    public class ArgGroup : ICollection<Arg>
    {
        public CommandDefine define { get; private set; }

        public List<Value> posArgsRedundant { get; set; } = new List<Value>();
        public Dictionary<string, Value> kvArgsRedundant { get; set; } = new Dictionary<string, Value>();

        public int Count => args.Count;

        public bool IsReadOnly => false;

        private Dictionary<string, Arg> args = new Dictionary<string, Arg>();
        private int posArgCount = 0;

        public ArgGroup(CommandDefine commandDefine)
        {
            this.define = commandDefine;
            posArgCount = 0;
        }

        /// <summary>
        /// 获取参数值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public Value this[string key]
        {
            get
            {
                return args[key].value!;
            }
        }

        /// <summary>
        /// 获取未定义的位置参数
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public Value GetUndefinedPosParam(int index)
        {
            return posArgsRedundant[index];
        }

        /// <summary>
        /// 获取未定义的键值参数
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public Value GetUndefinedKvParam(string key)
        {
            return kvArgsRedundant[key];
        }

        public bool Contains(string name)
        {
            return args.ContainsKey(name);
        }

        /// <summary>
        /// 运行子指令, 并替换对应的参数值
        /// </summary>
        public void RunSubCommand(CommandExecuter executer)
        {
            var list = args.Values.ToArray();
            foreach (var arg in list)
            {
                var cstr = arg.value.Cstr;
                if (!cstr.IsEmpty)
                {
                    var result = executer.Execute(cstr.commandIndex);
                    arg!.value = result;
                }
            }
        }

        public void Add(Arg arg)
        {
            if (arg == null)
            {
                throw new ArgumentNullException("不能添加 NULL 作为参数");
            }

            if (arg.isPosParam)//位置参数
            {
                var df = define.GetPosParamDefine(posArgCount++);//获取该位置上参数定义
                if (df == null)//缺失参数定义
                {
                    if (define.allowPosParamRedundant)//允许冗余参数
                    {
                        //冗余参数存入未定义组内
                        posArgsRedundant.Add(arg.value);
                    }
                    else
                    {
                        throw new UndefinedPosArgException(define, posArgCount);
                    }
                }
                else
                {
                    arg.name = df.name;
                    args[arg.name] = arg;
                    _Log($"添加参数: {arg}");
                }
            }
            else if (arg.useEpithet)//别称选项参数
            {
                var df = define.GetKVParamDefineWithEpithet(arg.name);//获取选项参数定义

                if (df == null)//缺失参数定义
                {
                    if (define.allowKvParamRedundant)//允许冗余参数
                    {
                        //冗余参数存入未定义组内
                        kvArgsRedundant[arg.name] = arg.value;
                    }
                    else
                    {
                        throw new UndefinedKVArgException(define, arg);
                    }
                }
                else
                {
                    if (!arg.value.active)//如果参数的值未填入
                    {
                        if (df.isMark)//标记型参数, 填入标记值
                        {
                            arg.name = df.name;
                            arg.value = new Value(df.markValue);
                            args[arg.name] = arg;
                            _Log($"添加参数: {arg}");
                        }
                        else//非标记参数则强制需要填入值
                        {
                            throw new KVParamMissingValueException(define, df);
                        }
                    }
                    else
                    {
                        arg.name = df.name;
                        args[arg.name] = arg;
                        _Log($"添加参数: {arg}");
                    }
                }
            }
            else//一般选项参数
            {
                var df = define.GetKeyValueParamDefine(arg.name);//获取选项参数定义
                if (df == null)//缺失参数定义
                {
                    if (define.allowKvParamRedundant)//允许冗余参数
                    {
                        //冗余参数存入未定义组内
                        kvArgsRedundant[arg.name] = arg.value;
                    }
                    else
                    {
                        throw new UndefinedKVArgException(define, arg);
                    }
                }
                else
                {
                    if (!arg.value.active)//如果参数的值未填入
                    {
                        if (df.isMark)//标记型参数, 填入标记值
                        {
                            arg.value = new Value(df.markValue);
                            args[arg.name] = arg;
                            _Log($"添加参数: {arg}");
                        }
                        else//非标记参数则强制需要填入值
                        {
                            throw new KVParamMissingValueException(define, df);
                        }
                    }
                    else
                    {
                        args[arg.name] = arg;
                        _Log($"添加参数: {arg}");
                    }
                }
            }
        }

        public void Clear()
        {
            args.Clear();
        }

        public bool Contains(Arg item)
        {
            return args.ContainsKey(item.name);
        }

        public void CopyTo(Arg[] array, int arrayIndex)
        {
            args.Values.CopyTo(array, arrayIndex);
        }

        public bool Remove(Arg item)
        {
            return args.Remove(item.name);
        }

        public IEnumerator<Arg> GetEnumerator()
        {
            return args.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return args.Values.GetEnumerator();
        }

        private void _Log(string message)
        {
#if TEST
            Console.ForegroundColor = ConsoleColor.Green;
            MessageOutput.BroadcastLine(message);
            Console.ForegroundColor = ConsoleColor.White;
#endif
        }
    }

    /// <summary>
    /// 参数
    /// </summary>
    public class Arg
    {
        /// <summary>
        /// 参数填入名称
        /// </summary>
        public string name = string.Empty;

        /// <summary>
        /// 参数填入数值
        /// </summary>
        public Value value;

        /// <summary>
        /// 是否为位置参数, 否->选项参数
        /// </summary>
        public bool isPosParam;

        /// <summary>
        /// 是否使用别称
        /// </summary>
        public bool useEpithet;

        public override string ToString()
        {
            return $"<参数>[{name}]={value.ToString()}  isPos: {isPosParam}  epithet: {useEpithet}";
        }
    }
}