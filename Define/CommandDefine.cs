//#define TEST
using System.Text;

namespace CmdParser.Define
{
    public class CommandDefine
    {
        /// <summary>
        /// 指令名称
        /// </summary>
        public string name = string.Empty;

        /// <summary>
        /// 指令全名
        /// </summary>
        public string fullName
        {
            get
            {
                return string.Concat(parent?.fullName ?? string.Empty, ' ', name).Trim();
            }
        }

        /// <summary>
        /// 指令描述
        /// </summary>
        public string description { get; set; } = string.Empty;

        /// <summary>
        /// 父级指令对象
        /// </summary>
        public CommandDefine? parent { get; private set; }

        /// <summary>
        /// 位置参数
        /// </summary>
        private List<PosParamDefine> posParams = new();

        private Dictionary<string, PosParamDefine> dic_posParams = new();

        /// <summary>
        /// 选项参数
        /// </summary>
        private Dictionary<string, KvParamDefine> kvParams = new();

        /// <summary>
        /// 子指令
        /// </summary>
        private Dictionary<string, CommandDefine> subCommands = new();

        /// <summary>
        /// 该指令执行的效果
        /// </summary>
        public Func<CommandExecuter, ArgGroup, Value>? execute;

        /// <summary>
        /// 是否允许填写冗余的位置参数
        /// </summary>
        public bool allowPosParamRedundant { get; set; } = false;

        /// <summary>
        /// 是否允许填写冗余的键值参数
        /// </summary>
        public bool allowKvParamRedundant { get; set; } = false;

        #region 容器

        /// <summary>
        /// 添加位置参数定义
        /// </summary>
        /// <param name="newDefine"></param>
        public CommandDefine AddParam(PosParamDefine newDefine)
        {
            PosParamDefine? last = null;
            if (posParams.Count > 0)
            {
                last = posParams[posParams.Count - 1];
            }
            //检查非法添加, 不可缺省参数 不可添加在 可缺省参数后面
            if (last != null)
            {
                if (!last.isRequire && newDefine.isRequire)
                {
                    throw new IllegalParameterAppendException(newDefine);
                }
            }

            //如果有重名参数, 则移除它
            if (dic_posParams.TryGetValue(newDefine.name, out var define))
            {
                posParams.Remove(define);
            }
            dic_posParams[newDefine.name] = newDefine;
            posParams.Add(newDefine);
            return this;
        }

        /// <summary>
        /// 添加选项参数定义
        /// </summary>
        /// <param name="kvp"></param>
        public CommandDefine AddParam(KvParamDefine kvp)
        {
            kvParams[kvp.name] = kvp;
            return this;
        }

        /// <summary>
        /// 获取指定位置上的位置参数定义
        /// </summary>
        /// <param name="index"></param>
        /// <returns>未定义则返回null</returns>
        public PosParamDefine? GetPosParamDefine(int index)
        {
            if (index < 0 || index >= posParams.Count)
            {
                return null;
            }
            return posParams[index];
        }

        /// <summary>
        /// 获取指定位置参数定义
        /// </summary>
        /// <param name="name">参数名</param>
        /// <returns>未定义则返回null</returns>
        public PosParamDefine? GetPosParamDefine(string name)
        {
            if (dic_posParams.TryGetValue(name, out var value))
                return value;
            return null;
        }

        /// <summary>
        /// 获取指定选项参数定义
        /// </summary>
        /// <param name="name">参数名</param>
        /// <returns>未定义则返回null</returns>
        public KvParamDefine? GetKeyValueParamDefine(string name)
        {
            if (kvParams.TryGetValue(name, out var value))
                return value;
            return null;
        }

        /// <summary>
        /// 通过别称获取选项参数定义
        /// </summary>
        /// <param name="epithet"></param>
        /// <returns></returns>
        public KvParamDefine? GetKVParamDefineWithEpithet(string epithet)
        {
            //检查别名是否匹配
            foreach (var df in kvParams.Values)
            {
                if (string.IsNullOrEmpty(df.epithet))
                    continue;
                if (df.epithet == epithet)
                {
                    return df;
                }
            }
            return null;
        }

        /// <summary>
        /// 获取指定子指令定义
        /// </summary>
        /// <param name="name"></param>
        /// <returns>未定义返回 null</returns>
        public CommandDefine? GetSubCommand(string name)
        {
            if (subCommands.TryGetValue(name, out var value))
                return value;
            return null;
        }

        /// <summary>
        /// 移除子指令
        /// </summary>
        /// <param name="name"></param>
        public void RemoveSubCommand(string name)
        {
            subCommands.Remove(name);
        }

        /// <summary>
        /// 添加子指令
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public CommandDefine AddSubCommand(string name)
        {
            var sub = new CommandDefine(name);
            sub.parent = this;
            subCommands.Add(name, sub);
            return sub;
        }
        /// <summary>
        /// 添加子指令
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>

        public CommandDefine AddSubCommand(string name, string description)
        {
            var sub = new CommandDefine(name, description);
            sub.parent = this;
            subCommands.Add(name, sub);
            return sub;
        }
        /// <summary>
        /// 位置参数迭代器
        /// </summary>
        /// <returns></returns>
        public IEnumerable<PosParamDefine> PosParamDefines()
        {
            foreach (var df in posParams)
            {
                yield return df;
            }
        }
        /// <summary>
        /// 键值参数迭代器
        /// </summary>
        /// <returns></returns>
        public IEnumerable<KvParamDefine> KvParamDefines()
        {
            foreach (var df in kvParams.Values)
            {
                yield return df;
            }
        }

        public bool ContainsSubCommand(string name)
        {
            return subCommands.ContainsKey(name);
        }

        #endregion 容器

        public CommandDefine(string name, string description = "")
        {
            this.name = name;
            this.description = description;
            AddParam(new KvParamDefine()
            {
                name = "help",
                epithet = "h",
                isMark = true,
                markValue = true,
                description = "显示帮助信息",
                isRequire = false,
                type = "bool",
                defaultValue = "false",
            });
        }

        /// <summary>
        /// 检查参数表, 弹出帮助信息, 帮助信息会中断指令的执行
        /// </summary>
        /// <param name="group"></param>
        /// <returns>是否中断指令执行</returns>
        private bool CheckHelp(ArgGroup? group)
        {
            if (group == null) return false;
            if (group.Contains("help"))
            {
                MessageOutput.BroadcastLine(GetHelpInfo());
                return true;
            }
            return false;
        }

        /// <summary>
        /// 检查参数组的合法性, 自动运行子指令将结果转化为参数值
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        public void CheckArgGroup(ArgGroup? group, CommandExecuter? executer = null)
        {
            List<object> missing = new List<object>();
            foreach (var df in posParams)
            {
                if (!(group?.Contains(df.name) ?? false))//不存在该参数
                {
                    if (df.isRequire)//如果是必须的位参
                    {
                        missing.Add(df);
                    }
                    else//否则自动填充为默认值
                    {
                        _LogAutoFillArg(df.name);
                        group?.Add(new Arg()
                        {
                            name = df.name,
                            value = new Value(df.defaultValue),
                            isPosParam = true,
                            useEpithet = false,
                        });
                    }
                }
            }

            foreach (var df in kvParams.Values)
            {
                if (!(group?.Contains(df.name) ?? false))
                {
                    if (df.isRequire)//如果是必须的参数
                    {
                        missing.Add(df);
                    }
                    else//否则自动填充为默认值
                    {
                        _LogAutoFillArg(df.name);
                        group?.Add(new Arg()
                        {
                            name = df.name,
                            value = new Value(df.defaultValue),
                            isPosParam = false,
                            useEpithet = false,
                        });
                    }
                }
            }

            if (missing.Count > 0)
            {
                throw new MissingRequiredParamException(this, missing.ToArray());
            }
        }

        /// <summary>
        /// 执行指令
        /// </summary>
        /// <param name="args">传入参数</param>
        /// <param name="executer">关联指令缓存集</param>
        /// <exception cref="ConvertException"></exception>
        /// <returns></returns>
        public Value? Invoke(ArgGroup? args, CommandExecuter? executer = null)
        {
            if (CheckHelp(args))
                return null;
            CheckArgGroup(args, executer);

            if (args == null)
                args = new ArgGroup(this);
            try
            {
                var result = execute?.Invoke(((executer != null) ? executer : new CommandExecuter()), args);
                return result;
            }
            catch (ConvertException ex)
            {
                ex.RelatedDefine = this;
                throw ex;
            }
        }

        public override string ToString()
        {
            return $"<command_define>: {name} : {description}";
        }

        public string GetHelpInfo()
        {
            StringBuilder sb = new StringBuilder($"{fullName}: {description}\n");
            foreach (var pos in posParams)
            {
                sb.Append($"   {pos}");
            }
            foreach (var kv in kvParams.Values)
            {
                sb.Append($"   {kv}");
            }
            foreach (var sub in subCommands.Values)
            {
                sb.Append($"  {sub.GetHelpInfo()}");
            }
            return sb.ToString();
        }

        private void _LogAutoFillArg(string name)
        {
#if TEST
            Console.ForegroundColor = ConsoleColor.Green;
            MessageOutput.BroadcastLine($"自动填充参数: {name}");
            Console.ForegroundColor = ConsoleColor.White;
#endif
        }
    }
}