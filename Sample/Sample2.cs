using CmdParser.Define;

namespace CmdParser.Sample
{
    /// <summary>
    /// 使用样例(详细注释说明)
    /// </summary>
    public class Sample2
    {
        /// <summary>
        /// 直接运行该函数查看样例运行情况
        /// </summary>
        public void RunSample()
        {
            //通过 VisualConsole 创建一个虚拟控制台对象
            // VisualConsole 内置了一个空的指令集 和 指令解析运行器
            VisualConsole console = new VisualConsole();

            //通过 AddCommand(commandName, commandDescription) 创建一个指令,
            //这将返回一个 CommandDefine 指令定义对象
            console.AddCommand("create_file", "创建文件")
                .AddParam(new PosParamDefine
                {
                    //CommandDefine.AddParam(): 给指令添加参数定义, 该函数执行后会返回指令定义对象本身, 可以实现链式添加参数
                    // - 位置参数: PosParamDefine
                    // - 可选参数: KvParamDefine
                    name = "filename",
                    description = "文件名(含后缀)",
                    type = "string",
                    isRequire = true,
                })
                .AddParam(new PosParamDefine
                {
                    name = "content",
                    description = "文件内容",
                    type = "string",
                    defaultValue = string.Empty,
                    isRequire = false,
                })
                .AddParam(new KvParamDefine
                {
                    name = "overwrite",
                    epithet = "o",
                    description = "是否覆盖已有文件",
                    type = "bool",
                    isMark = true,
                    markValue = true,
                    defaultValue = false,
                    isRequire = false,
                })
                .execute = (ce, args) =>
                {
                    //CommandDefine.execute:    给指令指定一个执行委托
                    //ce: CommandExecutor 当前指令运行的环境, (指令解析执行器)
                    //args: Args 解析出的参数组

                    //Args: 实现了索引获取参数值, 如果访问不存在的参数会报错, 返回 Value 类型对象
                    //Value: 是一个数据容器, 可以通过 .String .Int .Bool .Float 方法快速获取其转换对象,
                    //转换失败会导致报错
                    string fileName = args["filename"].String;
                    string content = args["content"].String;
                    bool overWrite = args["overwrite"].Bool;

                    if (File.Exists(fileName))
                    {
                        if (overWrite)
                        {
                            File.WriteAllText(fileName, content);
                        }
                    }
                    else
                    {
                        File.WriteAllText(fileName, content);
                    }

                    //最后该委托要求返回一个 Value 对象
                    //如果这个指令是嵌套指令的话, 这个值将会被解析为参数值放入其他指令对象中
                    //Value有两种空数据, Value.Null 和 Value.Empty
                    //Value.Null: 实际值为null, active=false, 表示是一种未定义的值, 缺省的值, 可以代表容器本身为空
                    //Value.Empty: 实际值为null, active=true, 表示 null 本身, 表示容器值为空
                    return Value.Null;
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

            input = Console.ReadLine() + string.Empty;
            var index = console.Parse(input);
            console.Run(index);
        }
    }
}