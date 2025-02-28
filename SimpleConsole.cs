using CmdParser.Define;
using CmdParser.GlobalAnchor;

namespace CmdParser
{
    /// <summary>
    /// 简易控制台
    /// <code>
    /// 内置指令:
    ///     exit: 退出命令模式
    ///     clear-mode: 修改控制台默认清除行为
    ///
    /// </code>
    /// </summary>
    public class SimpleConsole : VisualConsole
    {
        public bool running { get; set; } = false;
        public bool defaultClear { get; set; } = true;

        public SimpleConsole()
        {
            _RegisterCommand(commandSet);
        }

        public void Run()
        {
            running = true;
            string input;
            while (running)
            {
                Console.Write(Environment.NewLine + ">>  ");
                input = Console.ReadLine() + string.Empty;
                ParseAndRun(input, defaultClear);
            }
        }

        public void Stop()
        {
            running = false;
        }

        [CommandRegister("@SimpleConsole")]
        private void _RegisterCommand(CommandSet set)
        {
            var cmd = set.AddCommand("exit", "退出命令模式");
            cmd.allowKvParamRedundant = true;
            cmd.allowPosParamRedundant = true;
            cmd.execute = (ce, args) =>
            {
                running = false;
                Console.WriteLine("离开命令模式");
                return Value.Null;
            };

            var varCommand = set.AddCommand("var", "环境变量指令");
            varCommand.AddSubCommand("list", "打印当前环境中的所有变量")
                .execute = (ce, args) =>
                {
                    int count = 0;
                    foreach ((var varName, var varValue) in ce.GetVarValues())
                    {
                        MessageOutput.BroadcastLine($"@{varName}: {varValue}");
                        count += 1;
                    }
                    foreach ((var varName, var varValue) in ce.GetCommandStrings())
                    {
                        MessageOutput.BroadcastLine($"@{varName}: {varValue}");
                        count += 1;
                    }
                    if (count == 0)
                        MessageOutput.BroadcastLine("当前环境无定义变量");
                    return Value.Null;
                };
            varCommand.AddSubCommand("clear", "清空当前环境中的所有变量")
                .AddParam(new KvParamDefine
                {
                    name = "mode",
                    epithet="m",
                    description="设置默认清除模式",
                    isRequire =false,
                    defaultValue = false
                })
                .AddParam(new KvParamDefine
                {
                    name = "mode-info",
                    epithet = "i",
                    description = "显示设置信息",
                    defaultValue = false,
                    type = "bool",
                    isRequire = false,
                    isMark = true,
                    markValue = true,
                })
                .execute = (ce, args) =>
                {
                    bool used = false;
                    if(!args.IsDefaultParam("mode-info"))
                    {
                        MessageOutput.BroadcastLine($"当前清除模式: {defaultClear}");
                        used = true;
                    }
                    if (!args.IsDefaultParam("mode"))
                    {
                        defaultClear = args["mode"].Bool;
                        used = true;
                    }
                    if(!used)
                    {
                        Clear();
                        MessageOutput.BroadcastLine("已清空定义变量");
                    }
                    return Value.Null;
                };
        }
    }
}