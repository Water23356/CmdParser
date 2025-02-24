namespace CmdParser.SubParsers
{
    public class DicKvPairParser : ParserBase
    {
        public const char SPLIT_CHAR = ':';
        public const char SPLIT_CHAR2 = '=';

        private KvPair pair = new KvPair();
        private int state = 0;

        //解析结构 xxx:xxx
        //s1: 忽略空白, 直到 x->push, 中断->pop
        //push pop->s2
        //s2: 忽略空白, 直到 ':''='->s3, 中断->意外中断, x->非法结构
        //s3: 忽略空白, 直到 x->push, 中断->pop
        //push pop->pop
        public DicKvPairParser(string name) : base(name)
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
                default:
                    throw new UnknownParseState(this, state);
            }
        }

        private void S1(char c)
        {
            if (char.IsWhiteSpace(c)) return;//忽略
            switch (c)
            {
                case END:
                    Pop();
                    break;

                default:
                    Push(ParserBuilder.KEY_FIELD);
                    state = 2;
                    break;
            }
        }

        private void S2(char c)
        {
            if (char.IsWhiteSpace(c)) return;//忽略
            switch (c)
            {
                case END:
                    throw new UnexpectedInterruptionException(c, task!.index);

                case SPLIT_CHAR:
                case SPLIT_CHAR2:
                    state = 3;
                    break;

                default:
                    throw new FormatException(c, $"'{SPLIT_CHAR}'或'{SPLIT_CHAR2}'");
            }
        }

        private void S3(char c)
        {
            if (char.IsWhiteSpace(c)) return;//忽略
            switch (c)
            {
                case END:
                    Pop();
                    break;

                default:
                    Push(ParserBuilder.VALUE_FIELD);
                    state = 4;
                    break;
            }
        }

        public override void OnSubExit(ParserBase sub, char c)
        {
            switch (sub.name)
            {
                case ParserBuilder.KEY_FIELD:
                    pair.key = sub.tmp.String;
                    S2(c);
                    break;

                case ParserBuilder.VALUE_FIELD:
                    pair.value = sub.tmp;
                    Pop();
                    break;

                default:
                    break;
            }
        }

        public override void OnExit(char c)
        {
            SetTempValue(pair);
        }
    }
}