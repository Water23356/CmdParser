namespace CmdParser.SubParsers
{
    public class ValueFieldParser : ParserBase
    {
        public const char COMMAND_HEAD = '%';
        public const char DIC_HEAD = '{';
        public const char ARR_HEAD = '[';
        public const char STR_HEAD = '"';

        private int state = 0;
        //解析结构: xxx
        //解析结构: [xxx]
        //解析结构: {xxx}
        //解析结构: "xxx"
        //解析结构: %"xxx"
        //s1: 忽略空白符, '[','%','{','"'->push, 中断->pop, x->push
        //push->s2
        //s2-> '[','%','{','"',空白符->pop,  中断->pop

        public ValueFieldParser(string name) : base(name)
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
                case END:
                    Pop();
                    break;

                case ARR_HEAD:
                    state = 1;
                    Push(ParserBuilder.ARRAY_FIED);
                    break;

                case DIC_HEAD:
                    state = 1;
                    Push(ParserBuilder.DIC_FIELD);
                    break;

                case STR_HEAD:
                    state = 1;
                    Push(ParserBuilder.STRING_FIELD);
                    break;

                case COMMAND_HEAD:
                    state = 1;
                    Push(ParserBuilder.COMMAND_STRING);
                    break;

                default:
                    state = 1;
                    Push(ParserBuilder.COMMON_FIELD);
                    break;
            }
        }

        private void S2(char c)
        {
            Pop();
        }

        public override void OnExit(char c)
        {
        }

        public override void OnSubExit(ParserBase sub, char c)
        {
            switch (sub.name)
            {
                case ParserBuilder.COMMAND_STRING:
                case ParserBuilder.STRING_FIELD:
                case ParserBuilder.DIC_FIELD:
                case ParserBuilder.COMMON_FIELD:
                case ParserBuilder.ARRAY_FIED:
                    tmp = sub.tmp;
                    S2(c);
                    break;

                default:
                    throw new UnexpectedSubparserException(sub);
            }
        }
    }
}