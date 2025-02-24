namespace CmdParser
{
    /// <summary>
    /// 指令字符串
    /// </summary>
    public struct CommandString
    {
        /// <summary>
        /// 原始字符串
        /// </summary>
        public string str = string.Empty;

        /// <summary>
        /// 指令编号
        /// </summary>
        public string commandIndex = string.Empty;

        public CommandString()
        { }

        public CommandString(string str)
        { this.str = str; }

        public override string ToString()
        {
            return $"str={str}  commandIndex={commandIndex}";
        }

        public void Clear()
        {
            str = string.Empty;
            commandIndex = string.Empty;
        }

        public bool IsEmpty
        {
            get
            {
                return commandIndex == string.Empty;
            }
        }
    }
}