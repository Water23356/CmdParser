namespace CmdParser.Sample
{
    internal class Sample7
    {

        /// <summary>
        /// 直接运行该函数查看样例运行情况
        /// </summary>
        public void RunSample()
        {
            string text =
@"测试指令: 
  @sum = add 0.12 9.34
  var list
  var clear -i
  var clear -m=false
  var clear -i
  @sum = add 3.15 ""-45.62""
  var list
  @sum = @sum2 = add 3.14 9.6321
  var list
  @sum3 = add 3.21 @sum
  var list
  var clear
  var list
  @sum3 = add 3.21 @sum
";
            Console.WriteLine("Sample7");
            Console.WriteLine(text);
            SimpleConsole console = new SimpleConsole();
            console.AddCommand("add", "加法")
                .AddParam(new Define.PosParamDefine
                {
                    name = "a",
                    type = "float",
                    isRequire = true,
                })
                .AddParam(new Define.PosParamDefine
                {
                    name = "b",
                    type = "float",
                    isRequire = true,
                })
                .execute = (ce, args) =>
                {
                    float a = args["a"].Float;
                    float b = args["b"].Float;
                    float result = a + b;
                    MessageOutput.BroadcastLine($"{a}+{b}={result}");
                    return new Value(result);
                };


            console.Run();
        }
    }
}