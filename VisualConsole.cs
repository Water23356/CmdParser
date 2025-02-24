using CmdParser.Define;

namespace CmdParser
{
    public class VisualConsole
    {
        private CommandSet _set = new CommandSet();

        public CommandSet commandSet
        {
            get => _set;
            set
            {
                if (value == null)
                {
                    MessageOutput.BroadcastLine("指令集不能为空");
                    return;
                }
                _set = value;
                executer.defaultCommandSet = _set;
            }
        }

        public CommandExecuter executer { get; private set; } = new CommandExecuter();

        public VisualConsole()
        {
            executer.defaultCommandSet = commandSet;
        }

        /// <summary>
        /// 解析并执行指令
        /// </summary>
        /// <param name="command">指令串</param>
        /// <param name="clear">是否清空缓存区</param>
        public void ParseAndRun(string command, bool clear = true)
        {
            Console.WriteLine($"解析:{command} clear={clear}");
            try
            {
                executer.ParseAndExecute(command, clear);
            }
            catch (ParseException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                MessageOutput.BroadcastLine($"{ex.Message}");
                Console.ForegroundColor = ConsoleColor.White;
                if (ex.RelatedDefine == null)
                    MessageOutput.BroadcastLine($"help 以获取指令帮助");
                else
                {
                    MessageOutput.BroadcastLine($"{ex.RelatedDefine.fullName} -h 以获取指令帮助");
                    MessageOutput.BroadcastLine($"{ex.RelatedDefine.fullName} --help 以获取指令帮助");
                }
            }
            catch (ConvertException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                MessageOutput.BroadcastLine($"{ex.Message}");
                Console.ForegroundColor = ConsoleColor.White;
                if (ex.RelatedDefine == null)
                    MessageOutput.BroadcastLine($"help 以获取指令帮助");
                else
                {
                    MessageOutput.BroadcastLine($"{ex.RelatedDefine.fullName} -h 以获取指令帮助");
                    MessageOutput.BroadcastLine($"{ex.RelatedDefine.fullName} --help 以获取指令帮助");
                }
            }
        }

        /// <summary>
        /// 解析并执行指令
        /// </summary>
        /// <param name="commandHead">指令头</param>
        /// <param name="args">参数组?</param>
        /// <param name="clear">是否清空缓存区</param>
        public void ParseAndRun(string commandHead, string[] args, bool clear = true)
        {
            ParseAndRun(string.Join(' ', commandHead, string.Join(' ', args)), clear);
        }

        /// <summary>
        /// 仅解析指令
        /// </summary>
        /// <param name="command"></param>
        /// <param name="clear"></param>
        public string Parse(string command, bool clear = true)
        {
            try
            {
                return executer.Parse(command, clear);
            }
            catch (ParseException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                MessageOutput.BroadcastLine($"{ex.Message}");
                Console.ForegroundColor = ConsoleColor.White;
                if (ex.RelatedDefine == null)
                    MessageOutput.BroadcastLine($"help 以获取指令帮助");
                else
                {
                    MessageOutput.BroadcastLine($"{ex.RelatedDefine.fullName} -h 以获取指令帮助");
                    MessageOutput.BroadcastLine($"{ex.RelatedDefine.fullName} --help 以获取指令帮助");
                }
            }
            catch (ConvertException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                MessageOutput.BroadcastLine($"{ex.Message}");
                Console.ForegroundColor = ConsoleColor.White;
                if (ex.RelatedDefine == null)
                    MessageOutput.BroadcastLine($"help 以获取指令帮助");
                else
                {
                    MessageOutput.BroadcastLine($"{ex.RelatedDefine.fullName} -h 以获取指令帮助");
                    MessageOutput.BroadcastLine($"{ex.RelatedDefine.fullName} --help 以获取指令帮助");
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// 运行指令
        /// </summary>
        /// <param name="commandIndex">指令索引</param>
        public void Run(string commandIndex)
        {
            executer.Execute(commandIndex);
        }

        /// <summary>
        /// 清空缓存区
        /// </summary>
        public void Clear()
        {
            executer.Clear();
        }

        /// <summary>
        /// 添加指令
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public CommandDefine AddCommand(string name)
        {
            return commandSet.AddCommand(name);
        }

        /// <summary>
        /// 添加指令
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public CommandDefine AddCommand(string name, string description)
        {
            return commandSet.AddCommand(name, description);
        }

        /// <summary>
        /// 添加指令
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public CommandDefine AddCommand(string name, CommandDefine parent, string description = "")
        {
            return commandSet.AddCommand(name, parent, description);
        }

        /// <summary>
        /// 清空指令集
        /// </summary>
        public void ClearCommandSet()
        {
            commandSet.Clear();
        }

        /// <summary>
        /// 该指令是否存在
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(CommandDefine item)
        {
            return commandSet.Contains(item);
        }

        /// <summary>
        /// 该指令是否存在
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool Contains(string name)
        {
            return commandSet.Contains(name);
        }

        /// <summary>
        /// 注册当前程序集内的所有指令
        /// </summary>
        public void RegisterAllSet()
        {
            commandSet.RegisterAllSet();
        }

        /// <summary>
        /// 注册当前程序集内的特定标签组下的所有指令;
        /// </summary>
        /// <param name="tag">限定标签</param>
        public void RegisterAllSet(string tag)
        {
            commandSet.RegisterAllSet(true, tag);
        }
    }
}