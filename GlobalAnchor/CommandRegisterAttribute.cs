namespace CmdParser.GlobalAnchor
{
    /// <summary>
    /// 特性修饰: 该类型的命令 RegisteredCommands 将会被调用注册进指令集
    /// </summary>
    [AttributeUsage(AttributeTargets.Class|AttributeTargets.Method, AllowMultiple = false)]
    public class CommandRegisterAttribute : Attribute
    {
        public const string DEFAULT_TAG = "DEFAULT";

        /// <summary>
        /// 特性标签, 可用于分组
        /// </summary>
        public string[] Tag { get; set; } = new string[] { DEFAULT_TAG };

        public CommandRegisterAttribute()
        { }

        public CommandRegisterAttribute(string tag)
        {
            Tag = new string[] { tag };
        }

        public CommandRegisterAttribute(params string[] tags)
        {
            Tag = tags;
        }

        public bool ContainsTag(string tag)
        {
            return Tag.Contains(tag);
        }
    }
}