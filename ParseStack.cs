namespace CmdParser
{
    public class ParseStack
    {
        private Stack<ParserBase> stack = new Stack<ParserBase>();

        public void Push(ParserBase parser)
        {
            if (parser == null) throw new UnknownParserException("空的解析器");
            stack.Push(parser);
        }

        public ParserBase Push(string name)
        {
            var parser = ParserBuilder.Build(name);
            Push(parser);
            return parser;
        }

        public ParserBase Pop()
        {
            return stack.Pop();
        }

        public ParserBase? Peek()
        {
            if (stack.TryPeek(out var parser)) return parser;
            return null;
        }

        public void Clear()
        {
            stack.Clear();
        }
    }
}