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

        public string fullName => parentFullName +' '+ name;

        /// <summary>
        /// 携带的参数组
        /// </summary>
        public ArgGroup? args = null;

        /// <summary>
        /// 结果暂存变量名称
        /// </summary>
        public List<string> resultVarName = new();

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
                //正式运行前, 需要执行依赖的子指令, 并获取其返回值填充待确认的参数值
                //或将环境的中的变量填充到参数中
                args?.FillArgValue(executer);
                return define.Invoke(args, executer);
            }
            return null;
        }
    }
}