using CmdParser.Define;

namespace CmdParser.SubParsers
{
    internal class CommandParser : ParserBase
    {
        public const char VAR_HEAD = '@';
        public const char ASSIGN_CHAR = '=';
        public const char ASSIGN_CHAR2 = ':';
        private Command command = new Command();
        public CommandDefine? define { get; private set; }
        private int state = 0;
        //解析结构: xxx yyy yyy
        // value 以分隔符结尾, body 以分隔符' '结尾
        //解析结构: @xxx = xxx yyy yyy
        //s1: 忽略空白符, x->push(key), @->push(var), 中断->pop
        //push(key)->p1
        //push(var)->s3
        //p1: 未定义指令->error, p1->s2
        //s2: 忽略空白符, x->push, 中断->pop
        //push->p2
        //p2: 为指令头->s2, 其他->s2

        //s3: 忽略空白符, '='->push(p1), 中断->意外中断, x->非法结构

        public CommandParser(string name) : base(name)
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

                case VAR_HEAD:
                    state = 3;
                    Push(ParserBuilder.VAR_STRING);
                    break;

                default:
                    state = 2;
                    Push(ParserBuilder.KEY_FIELD);
                    break;
            }
        }

        private void S2(char c)
        {
            if (char.IsWhiteSpace(c)) return;//忽略
            switch (c)
            {
                case END:
                    Pop();
                    break;

                default:
                    Push(ParserBuilder.COMMAND_BODY);
                    state = 2;
                    break;
            }
        }

        private void S3(char c)
        {
            if (char.IsWhiteSpace(c)) return;//忽略
            switch (c)
            {
                case END:
                    throw new UnexpectedInterruptionException(c, task!.index);

                case ASSIGN_CHAR:
                case ASSIGN_CHAR2://赋值符号
                    state = 1;
                    break;

                default:
                    throw new FormatException(c, $"'{ASSIGN_CHAR}'或'{ASSIGN_CHAR2}'");
            }
        }

        public override void OnSubExit(ParserBase sub, char c)
        {
            switch (sub.name)
            {
                case ParserBuilder.KEY_FIELD:
                    command.name = sub.tmp.String;
                    //检查是否在指令集内
                    define = task!.commandSet!.GetCommand(command.name);
                    command.define = define;
                    if (define == null)//未找到对应定义
                    {
                        throw new UnknownCommandException(command);
                    }
                    S2(c);
                    break;

                case ParserBuilder.COMMAND_BODY:
                    //Log($"取出参数 {sub.tmp.value.GetType()} : hash={sub.tmp.GetHashCode()}");

                    var arg = sub.tmp.Arg;
                    if (command.args == null)//首次添加参数时, 需要根据确定指令创建参数组
                    {
                        if (arg.isPosParam)//如果解析的是位置参数, 还可能是子指令名称, 需要另外更新指令状态
                        {
                            var subDefine = define!.GetSubCommand(arg.value.String);
                            if (subDefine != null)//是子指令需要更新指令结构
                            {
                                command.parentFullName = command.name;
                                command.name = subDefine.name;
                                define = subDefine;
                                command.define = define;
                                _Log($"更新子命令结构: {command.name}");
                            }
                            else//不是子指令当做一般参数添加
                            {
                                command.args = new ArgGroup(define);//参数组需要根据指令定义创建, 它会自动检查参数的合法性
                                command.args.Add(arg);
                            }
                        }
                        else
                        {
                            command.args = new ArgGroup(define!);//参数组需要根据指令定义创建, 它会自动检查参数的合法性
                            command.args.Add(arg);
                        }
                    }
                    else//非首次添加参数
                    {
                        command.args.Add(arg);
                    }
                    _Log("参数添加完毕");
                    S2(c);
                    break;

                case ParserBuilder.VAR_STRING:
                    command.resultVarName.Add(sub.tmp.VarString.index);
                    S3(c);
                    break;

                default:
                    throw new UnexpectedSubparserException(sub);
            }
        }

        public override void OnExit(char c)
        {
            SetTempValue(command);
        }
    }
}