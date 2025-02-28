#define TEST
using CmdParser.Define;

namespace CmdParser
{
    /// <summary>
    /// 指令解析执行器
    /// </summary>
    public class CommandExecuter
    {
        private Dictionary<string, string> commandStrings = new Dictionary<string, string>();
        private Dictionary<string, Command> commands = new Dictionary<string, Command>();
        private Dictionary<string, Value> values = new Dictionary<string, Value>();
        private uint subStrCount = 0;

        /// <summary>
        /// 最近检查过的指令定义
        /// </summary>
        public CommandDefine? lastCheck { get; private set; }

        /// <summary>
        /// 默认指令集
        /// </summary>
        public CommandSet? defaultCommandSet { get; set; }

        public CommandExecuter()
        { }

        /// <summary>
        /// 清空缓存区
        /// </summary>
        public void Clear()
        {
            commandStrings.Clear();
            commands.Clear();
            values.Clear();
        }

        /// <summary>
        /// 解析指令字符串并执行
        /// </summary>
        /// <param name="commandStr">待解析字符串</param>
        /// <param name="clear">是否清除缓存区</param>
        /// <exception cref="MissingDefaultCommandSetException"></exception>
        /// <exception cref="ParseException"></exception>
        /// <exception cref="ConvertException"></exception>
        public void ParseAndExecute(string commandStr, bool clear = true)
        {
            if (clear)
                Clear();
            var str = AddCommandStr(commandStr);
            if (defaultCommandSet != null)
            {
                Parse(defaultCommandSet);
            }
            else
            {
                throw new MissingDefaultCommandSetException(this);
            }
            Execute(str);
        }

        /// <summary>
        /// 解析一组指令字符串并执行
        /// </summary>
        /// <param name="commandStrs">命令串组</param>
        /// <param name="clear">解析组之前是否清空之前的解析缓存</param>
        public void ParseAndExecute(string[] commandStrs, bool clear = true)
        {
            if (clear)
                Clear();
            foreach (var str in commandStrs)
            {
                ParseAndExecute(str, false);
            }
        }

        /// <summary>
        /// 解析指令字符串
        /// </summary>
        /// <param name="commandSet">使用指令集</param>
        /// <exception cref="ParseException"></exception>
        public void Parse(CommandSet commandSet)
        {
            ParseTask task = new ParseTask(this);
            while (commandStrings.Count > 0)
            {
                var pop = commandStrings.First();
                commandStrings.Remove(pop.Key);

                var command = task.Parse(commandSet, pop.Value);
                if (command != null)
                    commands[pop.Key] = command;
            }
        }

        /// <summary>
        /// 解析指令字符串
        /// </summary>
        /// <param name="commandStr">待解析字符串</param>
        /// <param name="clear">是否清除缓存区</param>
        /// <param name="commandSet">使用指令集,为空使用默认指令集</param>
        /// <returns></returns>
        /// <exception cref="MissingDefaultCommandSetException"></exception>
        /// <exception cref="ParseException"></exception>
        public string Parse(string commandStr, bool clear = true, CommandSet? commandSet = null)
        {
            if (clear)
                Clear();

            var index = AddCommandStr(commandStr);
            if (commandSet != null)
            {
                Parse(commandSet);
            }
            else if (defaultCommandSet != null)
            {
                Parse(defaultCommandSet);
            }
            else
            {
                throw new MissingDefaultCommandSetException(this);
            }
            return index;
        }

        /// <summary>
        /// 执行指定命令
        /// </summary>
        /// <param name="index">命令缓存名称</param>
        /// <exception cref="ConvertException"></exception>
        public Value Execute(string index)
        {
            if (commands.TryGetValue(index, out var command))
            {
                var result = command.Invoke(this) ?? Value.Null;
                foreach (var varName in command.resultVarName)
                {
                    values[varName] = result;
                    _Log($"存入变量: {varName}={result}");
                }
                return result;
            }
            MessageOutput.BroadcastLine($"指令不存在: {index}");
            return Value.Null;
        }

        /// <summary>
        /// 获取命令型参数的值;
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Value GetSubVarValue(string index)
        {
            if (values.TryGetValue(index, out var value))
                return value;
            MessageOutput.BroadcastLine($"变量不存在: {index}");
            return Value.Null;
        }

        /// <summary>
        /// 判断是否存在指定命令
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool ContainsCommand(string index)
        {
            return commands.ContainsKey(index);
        }

        /// <summary>
        /// 添加待解析指令字符串, 缓存区最多允许存放4,294,967,296条
        /// </summary>
        /// <param name="commandStr">待解析字符串</param>
        /// <returns></returns>
        public string AddCommandStr(string commandStr)
        {
            subStrCount = subStrCount + 1;
            string key = $"@subc:{subStrCount}";
            commandStrings[key] = commandStr;
            return key;
        }
        /// <summary>
        /// 获取一个迭代器: 返回环境内的 变量名和变量值
        /// </summary>
        /// <returns></returns>
        public IEnumerable<(string, Value)> GetVarValues()
        {
            foreach(var value in values)
            {
                yield return (value.Key, value.Value);
            }
        }
        /// <summary>
        /// 获取一个迭代器: 返回环境内的 指令索引和指令串
        /// </summary>
        /// <returns></returns>
        public IEnumerable<(string, string)> GetCommandStrings()
        {
            foreach (var value in commandStrings)
            {
                yield return (value.Key, value.Value);
            }
        }

        protected static void _Log(string message)
        {
#if TEST
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            MessageOutput.BroadcastLine(message);
#endif
        }
    }
}