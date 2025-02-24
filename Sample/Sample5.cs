using CmdParser.GlobalAnchor;

namespace CmdParser.Sample
{
    internal class Sample5
    {
        /// <summary>
        /// 直接运行该函数查看样例运行情况
        /// </summary>
        public void RunSample()
        {
            VisualConsole console = new VisualConsole();
            //可以通过 RegisterAllSet 方法来快捷注册指令
            //这样注册指令的代码就可以和执行代码分离开
            //
            //其中 SampleCommands/Print.cs 内定义了一个类
            //当一个类型是 ICommandSet 的实现类同时被 [CommandRegisterAttribute] 修饰时
            //它就会被视为一个允许注册的指令容器,
            //在调用 VisualConsole.RegisterAllSet() 时将会调用其 RegisteredCommands(CommandSet set) 方法进行指令注册

            console.RegisterAllSet();

            //另外还可以通过标签对指令容器进行分组
            //通过 VisualConsole.RegisterAllSet(tag) 的方式, 只会注册具有特定标签的容器内的指令
            //在指令容器类中:
            //[CommandRegisterAttribute(tag1,tag2,tag3)] 来定义容器的标签

            console.RegisterAllSet(CommandRegisterAttribute.DEFAULT_TAG);

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

            Console.ReadLine();
        }
    }
}