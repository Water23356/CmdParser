using CmdParser.Define;
using CmdParser.GlobalAnchor;

namespace CmdParser.CommandSets
{
    internal class RunCommandFile
    {
        [CommandRegister("@RunCommandFile")]
        public static void RegisteredCommands(CommandSet set)
        {
            set.AddCommand("run", "运行一个命令文件")
                .AddParam(new PosParamDefine
                {
                    name = "file",
                    description = "命令文件路径",
                    isRequire = true,
                    defaultValue = "",
                    type = "string",
                })
                .AddParam(new KvParamDefine
                {
                    name = "clear",
                    epithet = "c",
                    description = "清空命令环境",
                    isMark = true,
                    markValue = true,
                    defaultValue = false,
                    type = "bool",
                    isRequire = false,
                })
                .AddParam(new KvParamDefine
                {
                    name = "ignoreError",
                    epithet = "i",
                    description = "忽略命令执行错误, 发生错误时不中断后续执行",
                    isMark = true,
                    markValue = true,
                    defaultValue = false,
                    type = "bool",
                    isRequire = false,
                })
                .execute = (ce, args) =>
                {
                    string path = args["file"].String;
                    if (!File.Exists(path))
                    {
                        MessageOutput.BroadcastLine($"命令文件不存在: {path}");
                        return Value.Null;
                    }
                    var lines = File.ReadAllLines(path);
                    if (args["clear"].Bool)
                        ce.Clear();
                    for (int i = 0; i < lines.Length; i++)
                    {
                        string str = lines[i];
                        if (str.StartsWith("#") || str.Trim().Length == 0) continue;//忽略空行和注释行
                        try
                        {
                            ce.ParseAndExecute(str, false);
                        }
                        catch (Exception ex)
                        {
                            MessageOutput.BroadcastLine(ex.ToString());
                            if (args["ignoreError"].Bool)
                                continue;
                            MessageOutput.BroadcastLine($"指令发生错误: [{i}]:{str}, 剩余指令: {lines.Length - i} 条");
                            break;
                        }
                    }
                    return Value.Null;
                };
        }
    }
}