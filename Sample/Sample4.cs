using CmdParser.Define;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmdParser.Sample
{

    /// <summary>
    /// 使用样例(详细注释说明)
    /// </summary>
    public class Sample4
    {
        /// <summary>
        /// 直接运行该函数查看样例运行情况
        /// </summary>
        public void RunSample(string[] args)
        {
            Console.WriteLine("Sample4");
            VisualConsole console = new VisualConsole();
            console.AddCommand("default", "默认指令")
                .AddParam(new PosParamDefine
                {
                    name = "param1",
                    description = "位置参数1",
                    isRequire = true,
                })
                .AddParam(new PosParamDefine
                {
                    name = "param2",
                    description = "位置参数2",
                    isRequire = false,
                })
                .AddParam(new KvParamDefine
                {
                    name = "markParam",
                    isMark = true,
                    markValue = true,
                    defaultValue = false,
                })
                .execute = (ce, args) =>
                {
                    string p1 = args["param1"].String;
                    string p2 = args["param2"].String;
                    string mp = args["markParam"].String;
                    Console.WriteLine();
                    Console.WriteLine($"param1: {p1}");
                    Console.WriteLine($"param2: {p2}");
                    Console.WriteLine($"markParam: {mp}");
                    Console.ReadLine();
                    return Value.Null;
                };

            //如果你希望你程序能够通过控制台类似命令的形式执行功能的话
            //那么可以使用这种调用方式:
            //指定一个指令, 并将 Main() 内的参数数组传入
            console.ParseAndRun("default", args);
        }
    }
}
