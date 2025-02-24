using CmdParser.Define;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmdParser.GlobalAnchor
{
    /// <summary>
    /// 指令集接口
    /// </summary>
    public interface ICommandSet
    {
        public const string RegisterMethodName = "RegisteredCommands";

        /// <summary>
        /// 向指令集中注册指令
        /// </summary>
        /// <param name="set"></param>
        public void RegisteredCommands(CommandSet set);
    }
}
