namespace CmdParser.SubParsers
{
    public class CommandStrParser : ParserBase
    {
        public const char HEAD_CHAR = '%';
        public const char STR_CHAR = '"';

        private CommandString cstr = new CommandString();
        private int state = 0;
        //解析结构: %"xxxx"
        //s1: 忽略空白符, '%'->s2, x->非法结构, 中断->pop
        //s2: 忽略空白符, '"'->push, x->非法结构, 中断->意外中断
        //push->pop
        //s3: pop

        public CommandStrParser(string name) : base(name)
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
            _Log($"state='{state}'");
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
                case STR_CHAR:
                    Push(ParserBuilder.STRING_FIELD);
                    break;

                case END:
                    throw new UnexpectedInterruptionException(c, task!.index);

                default:
                    throw new FormatException(c, STR_CHAR);
            }
        }

        public override void OnExit(char c)
        {
            var index = task?.AddCommandStr(cstr.str) ?? string.Empty;
            cstr.commandIndex = index;
            SetTempValue(cstr);
        }

        public override void OnSubExit(ParserBase sub, char c)
        {
            switch (sub.name)
            {
                case ParserBuilder.STRING_FIELD:
                    cstr!.str = sub.tmp.String;
                    Pop();
                    break;

                default:
                    throw new UnexpectedSubparserException(sub);
            }
        }
    }
}