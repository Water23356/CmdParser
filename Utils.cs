namespace CmdParser
{
    internal class Utils
    {
        public static void TestIsNull(object obj, string name)
        {
            MessageOutput.BroadcastLine($"非空检测: {name}={obj != null}");
        }
    }
}