using System.Text;

namespace CmdParser.SubParsers
{
    public class CommonFieldParser : ParserBase
    {
        public const string END_CHARS = ",}\"]";

        private StringBuilder sb = new StringBuilder();
        private int state = 0;
        //解析: xxxx
        //s1: 忽略空白符, x->x,s2, 中断->pop
        //s2: 空白符->pop, x->x, 中断->pop, 结束符->pop

        public CommonFieldParser(string name) : base(name)
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

                default:
                    sb.Append(c);
                    state = 2;
                    break;
            }
        }

        private void S2(char c)
        {
            if (char.IsWhiteSpace(c))
            {
                Pop();
                return;
            }
            if (END_CHARS.Contains(c))
            {
                Pop();
                return;
            }
            switch (c)
            {
                case END:
                    Pop();
                    break;

                default:
                    sb.Append(c);
                    state = 2;
                    break;
            }
        }

        public override void OnExit(char c)
        {
            SetTempValue(sb.ToString());
        }

        public override void OnSubExit(ParserBase sub, char c)
        {
            //无子解析器
        }
    }
}