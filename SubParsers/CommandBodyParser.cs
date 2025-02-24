namespace CmdParser.SubParsers
{
    public class CommandBodyParser : ParserBase
    {
        public const char PARAM_HEAD = '-';

        private int state = 0;
        //解析结构: xxx
        //解析结构: -xxx
        //解析结构: --xxx
        //s1: 忽略空白符, '-'->s2, x->push(val), 中断->pop
        //push->s4
        //s2: '-'->s3, x->push(ep), 中断->意外中断
        //push->s4
        //s3: x->push(kp), 中断->意外中断
        //push->s4
        //s4: pop

        public CommandBodyParser(string name) : base(name)
        {
        }

        public override void OnEnter(char c)
        {
            state = 1;
            S1(c);
            _Log($"startWith='{c}'");
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
                case END:
                    Pop();
                    break;

                case PARAM_HEAD:
                    state = 2;
                    break;

                default:
                    Push(ParserBuilder.VALUE_FIELD);
                    break;
            }
        }

        private void S2(char c)
        {
            switch (c)
            {
                case END:
                    throw new UnexpectedInterruptionException(c, task!.index);

                case PARAM_HEAD:
                    state = 3;
                    break;

                default:
                    Push(ParserBuilder.KV_PARAM);
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
                    Push(ParserBuilder.KV_PARAM);
                    break;
            }
        }

        private void S4(char c)
        {
            Pop();
        }

        public override void OnExit(char c)
        {
            switch (state)
            {
                case 1:
                    Arg posArg = new Arg();
                    posArg.isPosParam = true;
                    posArg.value = tmp;
                    break;

                case 2:
                    Arg kvArg = tmp.Arg;
                    kvArg.isPosParam = false;
                    kvArg.useEpithet = true;

                    break;

                case 3:
                    Arg arg = tmp.Arg;
                    arg.isPosParam = false;
                    arg.useEpithet = false;
                    break;

                default:
                    throw new UnknownParseState(this, state);
            }
        }

        public override void OnSubExit(ParserBase sub, char c)
        {
            switch (sub.name)
            {
                case ParserBuilder.VALUE_FIELD:
                    SetTempValue(new Arg()
                    {
                        value = sub.tmp,
                        isPosParam = true,
                    });
                    S4(c);
                    //Log($"封装位置参数: {tmp.value.GetType()} : value={tmp.Arg.value} hash={tmp.GetHashCode()}");
                    break;

                case ParserBuilder.KV_PARAM:
                    tmp = sub.tmp;
                    S4(c);
                    //Log($"封装选项参数: {tmp.value.GetType()} : name={tmp.Arg.name} value={tmp.Arg.value} hash={tmp.GetHashCode()}");
                    break;

                default:
                    throw new UnexpectedSubparserException(sub);
            }
        }
    }
}