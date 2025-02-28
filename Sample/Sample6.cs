using CmdParser.SubParsers;
using CmdParser.Define;

namespace CmdParser.Sample
{
    internal class Sample6
    {
        /// <summary>
        /// 直接运行该函数查看样例运行情况
        /// </summary>
        public void RunSample()
        {
            // 创建一个控制台对象, 是 VisualConsole 的子类
            // 内置了一个命令输入循环函数, 可以自动解析输入的命令并执行, 适合简单的命令行需求
            // 内置了一个命令 "exit" 用于退出命令模式
            SimpleConsole console = new SimpleConsole();
            var command = console.AddCommand("command", "测试命令");
            // 命令定义内的 allowKvParamRedundant 和 allowPosParamRedundant 用于控制是否允许冗余参数
            // 默认情况下不允许冗余参数, 在解析时遇到未定义的参数填入会报错中断
            // 允许冗余参数后, 会自动将未定义的参数填入到 args.posArgsRedundant 和 args.kvArgsRedundant 中
            command.allowKvParamRedundant = true;   //允许冗余的键值参数
            command.allowPosParamRedundant = true;  //允许冗余的位置参数
            command.execute = (ce, args) =>
            {
                Console.WriteLine("冗余的位置参数: ");
                foreach(var pos in args.posArgsRedundant)
                {
                    Console.WriteLine(pos);
                }
                Console.WriteLine("冗余的键值参数: ");
                foreach(var kv in args.kvArgsRedundant)
                {
                    Console.WriteLine(kv.Key + " = " + kv.Value);
                }

                return Value.Null;
            };
            // 运行 Run() 函数, 会进入命令模式, 一直等待输入命令并执行, 直到输入 "exit" 退出命令模式
            // 死循环, 直到命令 "exit" 或者外部调用 Stop() 退出循环
            console.Run();

            //通过修改该值可以选择 键值参数的解析风格
            // Classic: 参数名参数值之间用 ' '分隔, 例如: -a 123;  --key "value"
            // KeyValuePair: 参数名参数值之间用 '='或':' 分隔, 例如: -a=123;  --key:value
            // 默认风格是 KeyValuePair
            KvParamParser.splitStyle = KvParamParser.SplitStyle.Classic;

            command.AddSubCommand("subcommand", "测试子命令")
                .AddParam(new KvParamDefine
                {
                    name = "key",
                    isMark = true,
                    markValue = true,
                }).AddParam(new KvParamDefine
                {
                    name = "key2",
                }).AddParam(new KvParamDefine
                {
                    name = "key3",
                })
                .execute = (ce,args)=>
                {
                    Console.WriteLine("key = " + args["key"]);
                    Console.WriteLine("key2 = " + args["key2"]);
                    Console.WriteLine("key3 = " + args["key3"]);
                    return Value.Null;
                };

            console.Run();
        }
    }
}