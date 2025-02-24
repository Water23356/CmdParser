using CmdParser.Define;
using System.Text;

namespace CmdParser
{
    /// <summary>
    /// 未知解析器异常
    /// </summary>
    public class UnknownParserException : Exception
    {
        public UnknownParserException(string message) : base(message)
        {
        }
    }

    /// <summary>
    /// 缺失默认解析器异常
    /// </summary>
    public class MissingDefaultCommandSetException : Exception
    {
        public MissingDefaultCommandSetException(CommandExecuter executer) : base($"CommandExecuter(hash={executer.GetHashCode()}): 缺失默认解析指令集")
        {
        }
    }

    /// <summary>
    /// 意外的子解析器异常(一般是解析器实现出问题)
    /// </summary>
    public class UnexpectedSubparserException : ParseException
    {
        public UnexpectedSubparserException(string message) : base(message)
        {
        }

        public UnexpectedSubparserException(ParserBase sub) : base($"意外的子解析器: {sub?.name ?? "NULL"}")
        {
        }
    }

    /// <summary>
    /// 转换异常
    /// </summary>
    internal class ConvertException : Exception
    {
        public CommandDefine? RelatedDefine { get; set; }
        public ConvertException(string msg) : base(msg)
        {
        }
    }

    #region 解析异常


    /// <summary>
    /// 解析异常
    /// </summary>
    public class ParseException : Exception
    {
        public CommandDefine? RelatedDefine { get; set; }
        public ParseException() { }
        public ParseException(string message) : base(message)
        {
        }
    }
    /// <summary>
    /// 缺失解析器异常
    /// </summary>
    public class MissingParserException : ParseException
    {
        public MissingParserException(string msg) : base(msg)
        {
        }
    }

    /// <summary>
    /// 包含非法字符异常
    /// </summary>
    public class ContainsIllegalException : ParseException
    {
        public ContainsIllegalException(string message) : base(message)
        {
        }

        public ContainsIllegalException(char c, int index) : base($"包含非法字符: '{c}' 位置={index}")
        {
        }
    }

    /// <summary>
    /// 意外的解析中断异常
    /// </summary>
    internal class UnexpectedInterruptionException : ParseException
    {
        public UnexpectedInterruptionException(char c, int index) : base($"意外中断: 字符='{c}' 位置={index}")
        {
        }
    }

    /// <summary>
    /// 未知指令异常(指令集未定义指令)
    /// </summary>
    public class UnknownCommandException : ParseException
    {
        public UnknownCommandException(string msg) : base(msg)
        {
        }

        public UnknownCommandException(Command command) : base($"未知指令:{string.Concat(command.parentFullName, ' ', command.name)} ")
        {
        }
    }

    /// <summary>
    /// 意外的解析器状态异常
    /// </summary>
    public class UnknownParseState : Exception
    {
        public UnknownParseState(string message) : base(message)
        {
        }

        public UnknownParseState(ParserBase p, int state) : base($"意外的解析器状态: parser={p?.name ?? "NULL"} state={state}")
        {
        }

        public UnknownParseState(ParserBase p, string state) : base($"意外的解析器状态: parser={p?.name ?? "NULL"} state={state}")
        {
        }
    }

    /// <summary>
    /// 未定义位置参数异常
    /// </summary>
    public class UndefinedPosArgException : ParseException
    {
        public UndefinedPosArgException(string msg) : base(msg)
        {
        }

        public UndefinedPosArgException(CommandDefine df, int index) : base($"{df.name} 未定义第 {index} 个位置参数")
        {
        }
    }

    /// <summary>
    /// 未定义选项参数异常
    /// </summary>
    public class UndefinedKVArgException : ParseException
    {
        public UndefinedKVArgException(string msg) : base(msg)
        {
        }

        public UndefinedKVArgException(CommandDefine df, Arg arg) : base($"{df.name} 未定义参数={arg.name}")
        {
        }
    }

    /// <summary>
    /// 非标记选项参数缺失参数值异常
    /// </summary>
    public class KVParamMissingValueException : ParseException
    {
        public KVParamMissingValueException(string msg) : base(msg)
        {
        }

        public KVParamMissingValueException(CommandDefine cdf, KvParamDefine df) : base($"{cdf.name}: 参数={df.name} 未填入值")
        {
        }
    }

    /// <summary>
    /// 缺少必要参数异常
    /// </summary>
    public class MissingRequiredParamException : ParseException
    {
        public MissingRequiredParamException(string msg) : base(msg)
        {
        }

        public MissingRequiredParamException(CommandDefine cdf, PosParamDefine df) 
            : base($"{cdf.name} 缺失必要的位置参数={df.name}") { }
        public MissingRequiredParamException(CommandDefine cdf, KvParamDefine df) 
            : base($"{cdf.name} 缺失必要的选项参数={df.name}") { }
        public MissingRequiredParamException(CommandDefine cdf, object[] missings)
            : base(GetMessage(cdf, missings)) { }
        

        private static string GetMessage(CommandDefine cdf, object[] missings)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"<{cdf.fullName}>: ");
            sb.Append($"缺失必要参数: ");
            foreach(var p in missings)
            {
                sb.Append(' ');
                var pdf = p as PosParamDefine;
                if (pdf != null)
                {
                    sb.Append(pdf.name);
                    continue;
                }
                var kvdf = p as KvParamDefine;
                if (kvdf != null)
                {
                    sb.Append(kvdf.name);
                    continue;
                }
            }
            return sb.ToString();
        }
    }

    /// <summary>
    /// 非法添加参数异常
    /// </summary>
    public class IllegalParameterAppendException : ParseException
    {
        public IllegalParameterAppendException(string msg) : base(msg)
        {
        }

        public IllegalParameterAppendException(PosParamDefine df) : base($"无法添加 {df.name} 参数; 因为不可缺省参数不可在可缺省参数后面添加")
        {
        }
    }

    /// <summary>
    /// 格式异常
    /// </summary>
    public class FormatException : ParseException
    {
        public FormatException(char c, string correct) : base($"格式错误: '{c}'  此处应当为: '{correct}'")
        {
        }

        public FormatException(char c, char correct) : base($"格式错误: '{c}'  此处应当为: '{correct}'")
        {
        }
    }

    #endregion 解析异常
}