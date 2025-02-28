//#define TEST

using CmdParser.Define;
using CmdParser.SubParsers;

namespace CmdParser
{
    /// <summary>
    /// 指令解析任务
    /// </summary>
    public class ParseTask
    {
        private ParseStack stack = new ParseStack();

        /// <summary>
        /// 字符解析索引
        /// </summary>
        public int index { get; set; } = 0;

        /// <summary>
        /// 待解析字符串
        /// </summary>
        public string cstring { get; private set; } = string.Empty;

        /// <summary>
        /// 上级指令执行器
        /// </summary>
        public CommandExecuter executer { get; private set; }

        /// <summary>
        /// 使用的解析指令集
        /// </summary>
        public CommandSet? commandSet { get; private set; }

        /// <summary>
        /// 解析结果
        /// </summary>
        public Command? result { get; private set; }

        public ParseTask(CommandExecuter builder)
        {
            this.executer = builder;
        }

        /// <summary>
        /// 添加待解析指令字符串
        /// </summary>
        /// <returns>返回字符串编号</returns>
        public string AddCommandStr(string commandStr)
        {
            return executer?.AddCommandStr(commandStr) ?? string.Empty;
        }

        /// <summary>
        /// 清空缓存
        /// </summary>
        public void Clear()
        {
            stack.Clear();
            index = 0;
            cstring = string.Empty;
            result = null;
        }

        /// <summary>
        /// 解析指令字符串
        /// </summary>
        /// <param name="set">使用的解析指令集</param>
        /// <param name="cstring">待解析串</param>
        /// <exception cref="ParseException"></exception>
        /// <returns></returns>
        public Command? Parse(CommandSet set, string cstring)
        {
            Clear();
            this.cstring = cstring;
            this.commandSet = set;

            //添加指令解析组件
            CommandParser parser = (CommandParser)Push(ParserBuilder.COMMAND);
            _LogCurrentParseState(parser, current, index);

            while (index < cstring.Length)
            {
                index += 1;
                try
                {
                    ParseNext();
                }
                catch (ParseException ex)
                {
                    ex.RelatedDefine = parser.define;
                    throw ex;
                }
            }
            //从解析组件中取出结果
            result = (Command)parser.tmp.value!;
            _LogParseOver();
            return result;
        }

        /// <summary>
        /// 使用指定解析器解析字符串
        /// </summary>
        /// <param name="set">使用的解析指令集</param>
        /// <param name="str">待解析串</param>
        /// <param name="parser">使用的解析器</param>
        /// <returns></returns>
        public Value ParseStrWith(CommandSet set, string str, ParserBase parser)
        {
            Clear();
            this.cstring = cstring;
            this.commandSet = set;

            //添加指令解析组件
            Push(parser);
            _LogCurrentParseState(parser, current, index);

            while (index < cstring.Length)
            {
                index += 1;
                try
                {
                    ParseNext();
                }
                catch (ParseException ex)
                {
                    var cp = parser as CommandParser;
                    ex.RelatedDefine = cp?.define;
                    throw ex;
                }
            }
            _LogParseOver();
            return parser.tmp;
        }

        /// <summary>
        /// 当前解析字符, 结束字符返回'\0'
        /// </summary>
        public char current
        {
            get
            {
                if (index < 0 || index >= cstring.Length)
                    return '\0';
                return cstring[index];
            }
        }

        /// <summary>
        /// 判断是否解析完毕
        /// </summary>
        /// <returns></returns>
        private bool IsParseOver()
        {
            return index >= cstring.Length;
        }

        /// <summary>
        /// 解析下一个字符
        /// </summary>
        /// <exception cref="MissingParserException"></exception>
        private void ParseNext()
        {
            char c = current;
            var parser = stack.Peek();
            _LogCurrentParseState(parser, c, index);
            if (parser == null)
            {
                if(IsParseOver())
                {
                    return;
                }
                throw new MissingParserException($"解析器栈为空: 字符: '{c}' 索引: {index}");
            }
            else
            {
                parser.OnUpdate(c);
            }
        }

        /// <summary>
        /// 压入解析器
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ParserBase Push(string name)
        {
            _LogPaserPush(name);
            var parser = stack.Push(name);
            parser.task = this;
            parser.OnEnter(current);
            return parser;
        }
        /// <summary>
        /// 压入解析器
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>

        public ParserBase Push(ParserBase parser)
        {
            _LogPaserPush(parser);
            stack.Push(parser);
            parser.task = this;
            parser.OnEnter(current);
            return parser;
        }

        /// <summary>
        /// 弹出解析器
        /// </summary>
        /// <returns></returns>
        public ParserBase Pop()
        {
            var parser = stack.Pop();
            parser.OnExit(current);

            var parent = stack.Peek();
            _LogPaserPop(parent, parser);
            if (parent != null)
            {
                parent.OnSubExit(parser, current);
            }

            return parser;
        }

        private void _LogCurrentParseState(ParserBase? parser,char c,int index)
        {
#if TEST
            Console.ForegroundColor = ConsoleColor.DarkGray;
            MessageOutput.BroadcastLine($"[{parser?.name ?? "null"}]: 字符: {((c == '\0') ? "END" : c)} 索引: {index}");
            Console.ForegroundColor = ConsoleColor.White;
#endif
        }
        private void _LogPaserPop(ParserBase? current,ParserBase pop)
        {
#if TEST
            Console.ForegroundColor = ConsoleColor.DarkRed;
            MessageOutput.BroadcastLine($"[{current?.name ?? "null"}]: 弹出解析器: '{pop.name}', 弹出值: '{pop.tmp}'");
            Console.ForegroundColor = ConsoleColor.White;
#endif
        }
        private void _LogPaserPush(ParserBase push)
        {
#if TEST
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            MessageOutput.BroadcastLine($"[{stack.Peek()?.name ?? "null"}]: 压入解析器: '{push.name}'");
            Console.ForegroundColor = ConsoleColor.White;
#endif
        }
        private void _LogPaserPush(string push)
        {
#if TEST
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            MessageOutput.BroadcastLine($"[{stack.Peek()?.name ?? "null"}]: 压入解析器: '{push}'");
            Console.ForegroundColor = ConsoleColor.White;
#endif
        }
        private void _LogParseOver()
        {
#if TEST
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            MessageOutput.BroadcastLine("解析完毕\n");
            Console.ForegroundColor = ConsoleColor.White;
#endif

        }
    }
}