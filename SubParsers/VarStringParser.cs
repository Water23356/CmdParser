namespace CmdParser.SubParsers
{
    /// <summary>
    /// 解析变量字符串
    /// </summary>
    internal class VarStringParser: ParserBase
    {
        public const char HEAD_CHAR = '@';
        private int state = 0;
        //解析结构: @xxxx
        //s1: 忽略空白符, @->s2:, x->非法结构, END->pop
        //s2: x->s3,push, END->意外中断
        //push->pop

        public VarStringParser(string name) : base(name)
        {
        }
        public override void OnEnter(char c)
        {
            state = 1;
            _Log($"startWith='{c}'");
            S1(c);
        }

        public override void OnUpdate(char c)
        {
            switch (state)
            {
                case 1: S1(c); break;
                case 2: S2(c); break;
                default:
                    throw new UnknownParseState(this, state);
            }
        }
        private void S1(char c)
        {
            if (char.IsWhiteSpace(c)) return;//忽略
            switch (c)
            {
                case HEAD_CHAR:
                    state = 2;
                    break;

                case END:
                    Pop();
                    break;

                default:
                    throw new FormatException(c, HEAD_CHAR);
            }
        }
        private void S2(char c)
        {
            if (char.IsWhiteSpace(c)) return;//忽略
            switch (c)
            {

                case END:
                    throw new UnexpectedInterruptionException(c, task!.index);

                default:
                    state = 3;
                    Push(ParserBuilder.KEY_FIELD);
                    break;

            }
        }
        public override void OnExit(char c)
        {
        }

        public override void OnSubExit(ParserBase sub, char c)
        {
            switch (sub.name)
            {
                case ParserBuilder.KEY_FIELD:
                    SetTempValue(new VarString(sub.tmp.String));
                    Pop();
                    break;

                default:
                    throw new UnexpectedSubparserException(sub);
            }
        }
    }
}