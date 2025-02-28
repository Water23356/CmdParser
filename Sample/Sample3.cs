using CmdParser.Define;

namespace CmdParser.Sample
{
    /// <summary>
    /// 使用样例(详细注释说明)
    /// </summary>
    public class Sample3
    {
        /// <summary>
        /// 直接运行该函数查看样例运行情况
        /// </summary>
        public void RunSample()
        {
            Console.WriteLine("Sample3");
            VisualConsole console = new VisualConsole();

            //参数值支持的类型有:
            //数字类型
            //布尔值
            //字符串

            //支持对象反序列化
            //这部分的编写和解析格式和json相似, 但是结构没有那么严格
            //(键无需使用""包裹)(键与值之间既可以用 : 分隔 也可以用 = 分隔)
            //嵌套数组
            //嵌套字典
            //嵌套指令串

            console.AddCommand("check", "查看结构信息")
            .AddParam(new PosParamDefine
            {
                name = "info",
                isRequire = true,
                type = "CommandInfo",
                defaultValue = string.Empty,
                description = "结构字符串"
            })
            .execute = (ce, args) =>
            {
                CommandInfo? info = args["info"].ConvertTo<CommandInfo>();
                if (info != null)
                {
                    info.Print();
                }
                else
                {
                    MessageOutput.BroadcastLine("解析失败");
                }
                return new Value(info);
            };


            bool cmdMode = true;
            string input = string.Empty;
            console.AddCommand("exit", "结束命令模式")
                .execute = (ce, args) =>
                {
                    cmdMode = false;
                    Console.WriteLine("离开命令模式");
                    return Value.Null;
                };

            while (cmdMode)
            {
                Console.Write(Environment.NewLine + ">>  ");
                input = Console.ReadLine() + string.Empty;
                console.ParseAndRun(input);
            }
        }
    }

    /// <summary>
    /// 一个测试的类型, 用于测试 Value 反序列化功能
    /// </summary>
    class CommandInfo
    {
        public int num;
        public string text = string.Empty;
        public bool isRequire;
        public CommandInfo? subInfo;

        //public bool[]? status;
        public List<bool> status;
        public Dictionary<string, string>? dic;

        public void Print()
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            MessageOutput.BroadcastLine("CommandInfo:");
            MessageOutput.BroadcastLine($"  num: {num}");
            MessageOutput.BroadcastLine($"  text: {text}");
            MessageOutput.BroadcastLine($"  isRequire: {isRequire}");
            MessageOutput.BroadcastLine($"  status: ");
            if (status != null)
            {
                foreach (var st in status)
                    MessageOutput.BroadcastLine($"\t{st}");
            }
            MessageOutput.BroadcastLine($"  dic: ");
            if (dic != null)
            {
                foreach (var pair in dic)
                    MessageOutput.BroadcastLine($"\t{pair.Key}:{pair.Value}");
            }
            MessageOutput.BroadcastLine($"  subInfo: {subInfo != null}");
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}