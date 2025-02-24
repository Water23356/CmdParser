namespace CmdParser.SubParsers
{
    public class DicFieldParser : ParserBase
    {
        public const char SPLIT_CHAR = ',';
        public const char START_CHAR = '{';
        public const char END_CHAR = '}';
        public const string END_CHAR_SET = "\"]";//这些终止符忽略

        private Dictionary<string, Value> values = new();
        private int state = 0;
        //解析结构: {xxx,xxx}
        //状态:
        //s1: 忽略空白, 直到: '{'->s2 ,中断->中断, x->非法结构
        //s2: 忽略空白, 直到: x->push, 中断->意外中断, '}'->s4, ','->忽略
        //push pop: -> s3
        //s3: 忽略空白, 直到 ','->s2 , '}'->s4, 中断->意外中断, x->非法结构
        //s4: 等待当前字符结束->pop

        public DicFieldParser(string name) : base(name)
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
                case 3: S3(c); break;
                case 4: S4(c); break;
                default:
                    throw new UnknownParseState(this, state);
            }
        }

        private void S1(char c)
        {
            if (char.IsWhiteSpace(c)) return;//忽略
            switch (c)
            {
                case START_CHAR:
                    state = 2;
                    break;

                case END:
                    Pop();
                    break;

                default:
                    throw new FormatException(c, START_CHAR);
            }
        }

        private void S2(char c)
        {
            if (char.IsWhiteSpace(c)) return;//忽略
            switch (c)
            {
                case END:
                    throw new UnexpectedInterruptionException(c, task!.index);
                case END_CHAR:
                    state = 4;
                    break;

                case SPLIT_CHAR:
                    break;

                default:
                    Push(ParserBuilder.KV_FIELD);
                    state = 3;
                    break;
            }
        }

        private void S3(char c)
        {
            if (char.IsWhiteSpace(c)) return;//忽略
            if (END_CHAR_SET.Contains(c)) return;//忽略
            switch (c)
            {
                case END:
                    throw new UnexpectedInterruptionException(c, task!.index);
                case END_CHAR:
                    state = 4;
                    break;

                case SPLIT_CHAR:
                    state = 2;
                    break;

                default:
                    throw new FormatException(c, END_CHAR);
            }
        }

        private void S4(char c)
        {
            Pop();
        }

        public override void OnExit(char c)
        {
            SetTempValue(values);
        }

        public override void OnSubExit(ParserBase sub, char c)
        {
            if (sub.name == ParserBuilder.KV_FIELD)
            {
                var pair = sub.tmp.Pair;
                if (!pair.IsEmpty)
                {
                    values[pair.key] = pair.value;
                }

                S3(c);
            }
            else
            {
                throw new UnexpectedSubparserException(sub);
            }
        }
    }
}