using CmdParser.Define;

namespace CmdParser
{
    /// <summary>
    /// 指令对象
    /// </summary>
    public class Command
    {
        /// <summary>
        /// 指令定义
        /// </summary>
        public CommandDefine? define;

        /// <summary>
        /// 指令名称
        /// </summary>
        public string name = string.Empty;

        /// <summary>
        /// 父级指令全名
        /// </summary>
        public string parentFullName = string.Empty;

        /// <summary>
        /// 携带的参数组
        /// </summary>
        public ArgGroup? args = null;

        /// <summary>
        /// 执行指令
        /// </summary>
        /// <param name="executer">指令执行器</param>
        /// <exception cref="ConvertException"></exception>
        /// <returns></returns>
        public Value? Invoke(CommandExecuter executer)
        {
            if (define != null)
            {
                args?.RunSubCommand(executer);
                return define.Invoke(args, executer);
            }
            return null;
        }
    }
}