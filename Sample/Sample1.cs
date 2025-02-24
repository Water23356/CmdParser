using CmdParser.Define;

namespace CmdParser.Sample
{
    public class Sample1
    {
        /// <summary>
        /// 直接运行该函数查看样例运行情况
        /// </summary>
        public void RunSample()
        {
            // Sample2 有更加详细的说明版本
            //通过 VisualConsole 创建一个虚拟控制台对象
            VisualConsole console = new VisualConsole();

            //通过 AddCommand(commandName, commandDescription) 创建一个指令
            //可链式添加参数和待执行指令
            console.AddCommand("square", "计算数值的平方")
               //给指令定义一个位置参数
               .AddParam(new PosParamDefine
               {
                   name = "num",
                   isRequire = true,
                   type = "float",
                   description = "底数",
               })
               //设定指令的执行委托
               .execute = (ce, args) =>
               {
                   //ce: 指令执行的环境对象
                   //args: 解析出的参数集
                   var num = args["num"].Float;
                   var result = num * num;
                   MessageOutput.BroadcastLine($"{num} 的平方是: {result}");
                   return new Value(result);
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
                //通过 VisualConsole.ParseAndRun(string,bool) 可直接进行解析并执行指令
                console.ParseAndRun(input);
            }

            //当然也可以分步进行
            //先进行指令解析, Parse(string,bool) 只会将字符串解析为待执行的命令对象
            //同时会返回一个 string 作为指令启动令牌
            input = Console.ReadLine() + string.Empty;
            var index = console.Parse(input);

            //最后可以通过 令牌 让命令对象被执行
            //如果令牌对应的命令对象不存在, 会有警告消息但不会报错
            console.Run(index);
        }
    }
}