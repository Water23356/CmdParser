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
        private int subStrCount = 0;
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
        /// <param name="commandSet">使用指令集</param>
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
                return command.Invoke(this) ?? new Value();
            }
            MessageOutput.BroadcastLine($"指令不存在: {index}");
            return new Value();
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
        /// 添加待解析指令字符串, 缓存区最多允许存放10000条
        /// </summary>
        /// <param name="commandStr">待解析字符串</param>
        /// <returns></returns>
        public string AddCommandStr(string commandStr)
        {
            subStrCount = (subStrCount + 1) % 10000;
            string key = subStrCount.ToString();
            commandStrings[key] = commandStr;
            return key;
        }
    }
}