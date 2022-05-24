using System;
using System.Text;

namespace EditorModule
{
    public class CsMethod : CsConstructor
    {
        /// <summary>
        /// 是否是静态函数
        /// </summary>
        public bool isStatic;
        /// <summary>
        /// 返回值
        /// </summary>
        public Type returnValue = typeof(void);
        /// <summary>
        /// 函数名
        /// </summary>
        public string methodName;
        /// <summary>
        /// 是否是operator
        /// </summary>
        public bool isOperator;
        /// <summary>
        /// operator的类型
        /// </summary>
        public string operatorType;

        public CsMethod(string methodName, CsFieldPrefix prefix, params MethodVariable[] var) : base(var)
        {
            this.methodName = methodName;
        }

        public CsMethod(string methodName, params MethodVariable[] var) : base(var)
        {
            this.methodName = methodName;
        }

        public override StringBuilder GetStr(string methodName, int braceIndex)
        {
            StringBuilder mainCode = new StringBuilder();
            mainCode.Append(prefix.ToString().ToLower());
            mainCode.Append(" ");
            if (isStatic || isOperator)
            {
                mainCode.Append("static ");
            }

            mainCode.Append(CsCode.GetTypeName(returnValue));
            mainCode.Append(" ");

            if (isOperator)
            {
                mainCode.Append("operator ");
                mainCode.Append(operatorType);
            }
            else
            {
                mainCode.Append(methodName);
            }
            mainCode.Append(GetVariable());
            mainCode.Append("\n");
            for (int i = 0; i < braceIndex; i++)
            {
                mainCode.Append("\t");
            }

            mainCode.Append("{\n");
            
            mainCode.Append(content.GetContent(braceIndex + 1));
            
            mainCode.Append("\n");
            for (int i = 0; i < braceIndex; i++)
            {
                mainCode.Append("\t");
            }
            mainCode.Append("}\n");
            return mainCode;
        }

        public void SetOperator(bool b, string s)
        {
            this.isOperator = b;
            if (b)
            {
                isStatic = true;
                operatorType = s;
            }
        }
    }
}