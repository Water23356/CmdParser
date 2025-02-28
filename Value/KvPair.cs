namespace CmdParser
{
    /// <summary>
    /// 键值对
    /// </summary>
    public class KvPair
    {
        public string key = string.Empty;
        public Value value;

        public void Clear()
        {
            key = string.Empty;
            value = new Value();
        }

        /// <summary>
        /// 是否为空
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                return (key == string.Empty);
            }
        }

        public override string ToString()
        {
            return $"{{{key}:{value}}}";
        }
    }
}