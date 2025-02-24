using System.Text;

namespace CmdParser.Define
{
    /// <summary>
    /// 位置参数定义
    /// </summary>
    public class PosParamDefine
    {
        /// <summary>
        /// 参数名称
        /// </summary>
        public string name = string.Empty;

        /// <summary>
        /// 默认值
        /// </summary>
        public object? defaultValue = null;

        #region 帮助信息

        /// <summary>
        /// 参数描述
        /// </summary>
        public string description = string.Empty;

        /// <summary>
        /// 参数类型描述
        /// </summary>
        public string type = string.Empty;

        /// <summary>
        /// 是否必须填入
        /// </summary>
        public bool isRequire = false;

        #endregion 帮助信息

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"{name}: {description}\n");
            sb.Append($"\ttype: {type}\n");
            sb.Append($"\tdefault: {defaultValue}\n");
            if (isRequire)
                sb.Append("\trequire\n");
            sb.Append('\n');
            return sb.ToString();
        }
    }
}