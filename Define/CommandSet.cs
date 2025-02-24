using CmdParser.GlobalAnchor;
using System.Collections;
using System.Reflection;
using System.Text;

namespace CmdParser.Define
{
    /// <summary>
    /// 指令集
    /// </summary>
    public class CommandSet : ICollection<CommandDefine>
    {
        private Dictionary<string, CommandDefine> commands = new Dictionary<string, CommandDefine>();

        public int Count => commands.Count;

        public bool IsReadOnly => true;

        /// <summary>
        /// 添加指令
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public CommandDefine AddCommand(string name)
        {
            var define = new CommandDefine(name);
            Add(define);
            return define;
        }

        /// <summary>
        /// 添加指令
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public CommandDefine AddCommand(string name, string description)
        {
            var define = new CommandDefine(name, description);
            Add(define);
            return define;
        }

        /// <summary>
        /// 添加指令
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public CommandDefine AddCommand(string name, CommandDefine parent, string description = "")
        {
            var define = parent.AddSubCommand(name);
            define.description = description;
            return define;
        }

        /// <summary>
        /// 移除指令
        /// </summary>
        /// <param name="name"></param>
        public void RemoveCommand(string name)
        {
            commands.Remove(name);
        }

        /// <summary>
        /// 获取指令
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public CommandDefine? GetCommand(string name)
        {
            if (commands.TryGetValue(name, out var define)) return define;
            return null;
        }

        /// <summary>
        /// 打印所有指令
        /// </summary>
        public void PrintAllCommand()
        {
            foreach (var command in commands.Values)
            {
                MessageOutput.BroadcastLine(command);
            }
        }

        /// <summary>
        /// 添加指令
        /// </summary>
        /// <param name="item"></param>
        public void Add(CommandDefine item)
        {
            commands[item.name] = item;
        }

        /// <summary>
        /// 清空
        /// </summary>
        public void Clear()
        {
            commands.Clear();
        }

        /// <summary>
        /// 该指令是否存在
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool Contains(string name)
        {
            return commands.ContainsKey(name);
        }

        /// <summary>
        /// 该指令是否存在
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(CommandDefine item)
        {
            return commands.Values.Contains(item);
        }

        /// <summary>
        /// 复制到数组
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(CommandDefine[] array, int arrayIndex)
        {
            commands.Values.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// 移除指令
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(CommandDefine item)
        {
            return commands.Remove(item.name);
        }

        /// <summary>
        /// 获取枚举器
        /// </summary>
        /// <returns></returns>
        public IEnumerator<CommandDefine> GetEnumerator()
        {
            return commands.Values.GetEnumerator();
        }

        /// <summary>
        /// 获取枚举器
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public CommandSet()
        {
            var cmd = AddCommand("help", "显示帮助信息");
            cmd.execute = (ce, args) =>
            {
                MessageOutput.BroadcastLine(GetHelpInfo());
                return Value.Null;
            };
        }

        /// <summary>
        /// 获取帮助信息
        /// </summary>
        /// <returns></returns>
        public string GetHelpInfo()
        {
            var sb = new StringBuilder();
            foreach (var command in commands.Values)
            {
                sb.AppendLine(command.GetHelpInfo());
            }
            return sb.ToString();
        }

        /// <summary>
        /// 使用反射注册 当前程序集内所有 ICommandSet 接口内定义的指令
        /// <code>
        /// 要求目标类必须是 ICommandSet 的实现类, 同时拥有 CommandRegisterAttribute 属性修饰;
        /// 可以通过 tag 进行额外限定
        /// </code>
        /// </summary>
        /// <param name="limitTag">是否限定标签</param>
        /// <param name="tag">限定标签</param>
        public void RegisterAllSet(bool limitTag = false, string tag = CommandRegisterAttribute.DEFAULT_TAG)
        {
            // 获取当前程序集（也可以指定其他程序集）
            Assembly assembly = Assembly.GetExecutingAssembly();
            Type[] types = assembly.GetTypes();
            foreach (Type type in types)
            {
                //检查 ICommandSet 接口派生类
                if (typeof(ICommandSet).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
                {
                    object instance = Activator.CreateInstance(type);
                    if (instance == null) continue;
                    //检查 CommandRegisterAttribute 修饰
                    CommandRegisterAttribute attribute = (CommandRegisterAttribute)Attribute.GetCustomAttribute(type, typeof(CommandRegisterAttribute));
                    if (attribute == null) continue;
                    if (limitTag)
                    {
                        if (!attribute.Tag.Contains(tag)) continue;
                    }
                    MethodInfo method = type.GetMethod(ICommandSet.RegisterMethodName);
                    if (method == null) continue;
                    method.Invoke(instance, new[] { this });
                }
                //获取公开,私有,实例,静态方法对象
                MethodInfo[] methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                foreach (var item in methods)
                {
                    var attribute = (CommandRegisterAttribute)Attribute.GetCustomAttribute(item, typeof(CommandRegisterAttribute));
                    if (attribute == null) continue;
                    if (limitTag)
                    {
                        if (!attribute.Tag.Contains(tag)) continue;
                    }
                    if (item.Name != ICommandSet.RegisterMethodName)
                    {
                        if (item.IsStatic)
                        {
                            item.Invoke(null, new object[] { this });
                        }
                        else
                        {
                            object instance = Activator.CreateInstance(type);
                            if (instance == null) continue;
                            item.Invoke(instance, new object[] { this });
                        }
                    }
                }
            }
        }
    }
}