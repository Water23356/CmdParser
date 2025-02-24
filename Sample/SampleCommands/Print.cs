using CmdParser.Define;
using CmdParser.GlobalAnchor;

namespace CmdParser.Sample.SampleCommands
{
    [CommandRegister()]
    public class Print : ICommandSet
    {
        public void RegisteredCommands(CommandSet set)
        {
            set.AddCommand("print", "打印消息")
                .AddParam(new PosParamDefine
                {
                    name = "message",
                    description = "消息",
                    isRequire = false,
                    defaultValue = "",
                    type = "string",
                })
                .AddParam(new KvParamDefine
                {
                    name = "repeat",
                    epithet = "r",
                    isMark = true,
                    markValue = 2,
                    defaultValue = 1,
                    isRequire = false,
                    type = "int",
                    description = "重复次数"
                })
                .execute = (ce, args) =>
                {
                    var message = args["message"].String;
                    var count = args["repeat"].Int;
                    while (count > 0)
                    {
                        MessageOutput.BroadcastLine(message);
                        count--;
                    }
                    return new Value(message);
                };
        }
        [CommandRegister()]
        private void _RegisterCommands(CommandSet set)
        {
            set.AddCommand("print_2", "打印消息方法2")
                .AddParam(new PosParamDefine
                {
                    name = "message",
                    description = "消息",
                    isRequire = false,
                    defaultValue = "",
                    type = "string",
                })
                .AddParam(new KvParamDefine
                {
                    name = "repeat",
                    epithet = "r",
                    isMark = true,
                    markValue = 2,
                    defaultValue = 1,
                    isRequire = false,
                    type = "int",
                    description = "重复次数"
                })
                .execute = (ce, args) =>
                {
                    var message = args["message"].String;
                    var count = args["repeat"].Int;
                    while (count > 0)
                    {
                        MessageOutput.BroadcastLine(message);
                        count--;
                    }
                    return new Value(message);
                };
        }
    }
}