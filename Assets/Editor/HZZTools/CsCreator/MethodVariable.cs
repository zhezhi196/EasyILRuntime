using System;
using Module;

namespace EditorModule
{
    public class MethodVariable
    {
        /// <summary>
        /// 函数参数的类型
        /// </summary>
        public Type type;
        /// <summary>
        /// 函数参数的名字
        /// </summary>
        public string name;
        /// <summary>
        /// 函数参数的前后缀
        /// </summary>
        public MethodPrefix prefix;
        /// <summary>
        /// 默认值
        /// </summary>
        public string defaultValue;

        public MethodVariable(Type type, string name)
        {
            this.type = type;
            this.name = name;
        }

        public string GetStr()
        {
            string temp = string.Empty;
            if (prefix == MethodPrefix.Params)
            {
                temp = prefix.ToString().ToLower() + " " + CsCode.GetTypeName(type) + "[] " + name;
            }
            else if (prefix == MethodPrefix.None)
            {
                temp = CsCode.GetTypeName(type) + " " + name;
            }

            if (!defaultValue.IsNullOrEmpty())
            {
                temp += (" = " + defaultValue);
            }

            return temp;
        }
    }
}