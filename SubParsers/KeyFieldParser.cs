using System.Text;

namespace CmdParser.SubParsers
{
    public class KeyFieldParser : ParserBase
    {
        public const string END_CHARS = "=:";
        private StringBuilder sb = new StringBuilder();
        private int state = 0;
        //解析: xxxx
        //s1: 忽略空白符, '"'->push, x->x,s2, 中断->pop, 非法字符->非法字符
        //push->s3
        //s2: 空白符->pop, x->x, 中断->pop, 结束符->pop,非法字符->非法字符
        //s3: 等待当前字符结束->pop

        public KeyFieldParser(string name) : base(name)
        {
        }

        /// <summary>
        /// 判断字符串是否合法
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private bool IsLegal(char c)
        {
            return char.IsLetterOrDigit(c) || c == '_' || c == '-';
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
                case StringFieldParser.SE_CHAR:
                    Push(ParserBuilder.STRING_FIELD);
                    state = 3;
                    break;

                case END:
                    Pop();
                    break;

                default:
                    if (IsLegal(c))
                    {
                        _Log($"拾取: {c}, state={state}");
                        sb.Append(c);
                        state = 2;
                    }
                    else
                    {
                        throw new ContainsIllegalException(c, task!.index);
                    }
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
                    if (IsLegal(c))
                    {
                        _Log($"拾取: {c}, state={state}");
                        sb.Append(c);
                        state = 2;
                    }
                    else
                    {
                        throw new ContainsIllegalException(c, task!.index);
                    }
                    break;
            }
        }

        public override void OnExit(char c)
        {
            if (state != 3)//因为这时候的值直接取子解析的值, 而不是自身的stringbuilder
                SetTempValue(sb.ToString());
            _Log($"sb: {sb.ToString()}");
            Log(tmp);
        }

        public override void OnSubExit(ParserBase sub, char c)
        {
            //无子解析器
            switch (sub.name)
            {
                case ParserBuilder.STRING_FIELD:
                    tmp = sub.tmp;
                    Pop();
                    break;

                default:
                    throw new UnexpectedSubparserException(sub);
            }
        }
    }
}