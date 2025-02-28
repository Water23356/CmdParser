////#define TEST

namespace CmdParser
{
    public abstract class ParserBase
    {
        public const char END = '\0';
        private string _name = string.Empty;
        private Value _tmp = new Value();

        /// <summary>
        /// 解析器名称
        /// </summary>
        public virtual string name { get => _name; set => _name = value; }

        /// <summary>
        /// 解析器缓存区
        /// </summary>
        public virtual Value tmp { get => _tmp; set => _tmp = value; }

        /// <summary>
        /// 所在的任务对象
        /// </summary>
        public ParseTask? task { get; set; }

        public ParserBase(string name)
        { this.name = name; }

        protected void Push(string name)
        {
            task?.Push(name);
        }

        protected ParserBase? Pop()
        {
            return task?.Pop();
        }

        /// <summary>
        /// 加入解析器时触发
        /// <code>
        /// 注意: 该函数不应包含拾取字符的逻辑;
        /// 拾取字符的逻辑应当在 OnUpdate() 内执行, 避免重复拾取
        /// </code>
        /// </summary>
        /// <param name="c">当前字符</param>
        public abstract void OnEnter(char c);

        /// <summary>
        /// 拾取字符时触发
        /// </summary>
        /// <param name="c">当前字符</param>
        public abstract void OnUpdate(char c);

        /// <summary>
        /// 当子级解析组件被移除时触发
        /// </summary>
        /// <param name="sub">被移除的子级解析组件</param>
        /// <param name="c">当前字符</param>
        public abstract void OnSubExit(ParserBase sub, char c);

        /// <summary>
        /// 从解析器中移除时触发
        /// </summary>
        /// <param name="c">当前字符</param>
        public abstract void OnExit(char c);

        public virtual void Clear()
        {
            tmp = new Value();
            task = null;
        }

        /// <summary>
        /// 更改缓存区的值
        /// </summary>
        /// <param name="value"></param>
        public void SetTempValue(object value)
        {
            tmp = new Value(value);
        }

        /// <summary>
        /// 子类debug时使用该方法输出调试信息
        /// </summary>
        /// <param name="message"></param>
        /// <param name="color"></param>
        protected void _Log(string message, ConsoleColor color = ConsoleColor.White)
        {
#if TEST
            Console.ForegroundColor = color;
            MessageOutput.BroadcastLine(message);
#endif
        }
        protected void Log(object message, ConsoleColor color = ConsoleColor.White)
        {
#if TEST
            Console.ForegroundColor = color;
            MessageOutput.BroadcastLine(message);
#endif
        }
    }
}