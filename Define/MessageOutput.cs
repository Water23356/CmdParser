namespace CmdParser
{
    public static class MessageOutput
    {
        public static event Action<string> BroadcastEvent = Console.Write;

        /// <summary>
        /// 广播消息
        /// </summary>
        /// <param name="message"></param>
        public static void Broadcast(string message)
        {
            BroadcastEvent?.Invoke(message);
        }

        /// <summary>
        /// 广播消息并换行
        /// </summary>
        /// <param name="message"></param>
        public static void BroadcastLine(string message)
        {
            BroadcastEvent?.Invoke(message + Environment.NewLine);
        }
        public static void BroadcastLine()
        {
            BroadcastEvent?.Invoke(Environment.NewLine);
        }

        /// <summary>
        /// 广播消息
        /// </summary>
        /// <param name="message"></param>

        public static void Broadcast(object message)
        {
            BroadcastEvent?.Invoke(message.ToString() ?? string.Empty);
        }

        /// <summary>
        /// 广播消息并换行
        /// </summary>
        /// <param name="message"></param>
        public static void BroadcastLine(object message)
        {
            BroadcastEvent?.Invoke(message.ToString() ?? string.Empty + Environment.NewLine);
        }
    }
}