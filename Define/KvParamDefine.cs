using System.Text;

namespace CmdParser.Define
{
    /// <summary>
    /// 选项参数定义
    /// </summary>
    public class KvParamDefine
    {
        /// <summary>
        /// 参数名称
        /// </summary>
        public string name = string.Empty;

        /// <summary>
        /// 别称
        /// </summary>
        public string epithet = string.Empty;

        /// <summary>
        /// 该参数是否必须存在
        /// </summary>
        public bool isRequire = false;

        /// <summary>
        /// 默认值
        /// </summary>
        public object? defaultValue = string.Empty;

        /// <summary>
        /// 是否是标记参数(可无需填值)
        /// </summary>
        public bool isMark = false;

        /// <summary>
        /// 标记值(仅作为标记时填入的值)
        /// </summary>
        public object? markValue = string.Empty;

        #region 帮助信息

        /// <summary>
        /// 参数描述
        /// </summary>
        public string description = string.Empty;

        /// <summary>
        /// 参数类型描述
        /// </summary>
        public string type = string.Empty;

        #endregion 帮助信息

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"-{epithet} --{name}: {description}\n");
            sb.Append($"\ttype: {type}\n");
            sb.Append($"\tdefault: {defaultValue}\n");
            if (isMark)
            {
                sb.Append($"\tmarkValue: {markValue}\n");
            }
            if (isRequire)
                sb.Append("\trequire\n");
            sb.Append('\n');
            return sb.ToString();
        }
    }
}