using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmdParser
{
    /// <summary>
    /// 变量字符串
    /// </summary>
    public struct VarString
    {
        public string index;
        public VarString() { index = string.Empty; }
        public VarString(string name) { this.index = name; }
        public bool IsEmpty { get { return string.IsNullOrEmpty(index); } }
        public override string ToString()
        {
            return $"[VarString]: index='{index}'";
        }
    }
}
