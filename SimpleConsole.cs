namespace CmdParser
{
    /// <summary>
    /// 控制台: 内置了一个命令输入循环函数
    /// </summary>
    public class SimpleConsole: VisualConsole
    {
        public bool running { get; set; } = false;

        public SimpleConsole()
        {
            var cmd = AddCommand("exit", "退出命令模式");
            cmd.allowKvParamRedundant = true;
            cmd.allowPosParamRedundant = true;
            cmd.execute = (ce, args) =>
            {
                running = false;
                Console.WriteLine("离开命令模式");
                return Value.Null;
            };
        }

        public void Run()
        {
            running = true;
            string input;
            while (running)
            {
                Console.Write(Environment.NewLine + ">>  ");
                input = Console.ReadLine() + string.Empty;
                ParseAndRun(input);
            }
        }

        public void Stop()
        {
            running = false;
        }
    }
}