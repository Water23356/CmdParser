using System.Text;

namespace CmdParser.SubParsers
{
    public class StringFieldParser : ParserBase
    {
        /// <summary>
        /// 开始或结尾标记
        /// </summary>
        public const char SE_CHAR = '"';

        /// <summary>
        /// 转译符号
        /// </summary>
        public const char TRAN_CHAR = '\\';

        private StringBuilder sb = new StringBuilder();
        private int state = 0;
        //解析结构: "xxxx"
        //解析结构: \n
        //s1: 忽略空白符, '"'->s2, 中断->pop, x->非法结构
        //s2: x->x, '\'->s3, '"'->s4, 中断->意外中断
        //s3: x->s2, 中断->意外中断
        //s4: 等待当前字符结束->pop

        public char Translate(char c)
        {
            switch (c)
            {
                case '\\': return '\\';
                case 'n': return '\n';
                case 'r': return '\r';
                case 't': return '\t';
                default:
                    //默认直接返回原始值
                    return c;
            }
        }

        public StringFieldParser(string name) : base(name)
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
                case SE_CHAR:
                    state = 2;
                    break;

                case END:
                    Pop();
                    break;

                default:
                    throw new FormatException(c, SE_CHAR);
            }
        }

        private void S2(char c)
        {
            switch (c)
            {
                case SE_CHAR:
                    state = 4;
                    break;

                case END:
                    throw new UnexpectedInterruptionException(c, task!.index);

                case TRAN_CHAR:
                    state = 3;
                    break;

                default:
                    sb.Append(c);
                    break;
            }
        }

        private void S3(char c)
        {
            switch (c)
            {
                case END:
                    throw new UnexpectedInterruptionException(c, task!.index);

                default:
                    sb.Append(Translate(c));
                    state = 2;
                    break;
            }
        }

        private void S4(char c)
        {
            Pop();
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