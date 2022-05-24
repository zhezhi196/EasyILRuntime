using System;
using System.Collections.Generic;
using System.Text;

namespace EditorModule
{
    public class CsConstructor
    {
        public CsFieldPrefix prefix;
        /// <summary>
        /// 参数
        /// </summary>
        public readonly List<MethodVariable> variable;
        /// <summary>
        /// 函数内容
        /// </summary>
        public CsMethodInfo content = new CsMethodInfo();

        public CsConstructor(CsFieldPrefix prefix, params MethodVariable[] var)
        {
            this.prefix = prefix;
            this.variable = new List<MethodVariable>(var);
        }
        
        public CsConstructor(params MethodVariable[] var)
        {
            this.variable = new List<MethodVariable>(var);
        }

        public void WriteMethod(string content)
        {
            if (this.content == null)
            {
                this.content = new CsMethodInfo();
            }

            this.content.Append(content);
        }

        public string GetVariable()
        {
            List<string> temp = new List<string>();
            variable.Sort((a,b)=> { return a.prefix.CompareTo(b.prefix); });
            for (int i = 0; i < variable.Count; i++)
            {
                temp.Add(variable[i].GetStr());
            }

            return "(" + string.Join(", ", temp) + ")";
        }

        public virtual StringBuilder GetStr(string methodName, int braceIndex)
        {
            StringBuilder mainCode = new StringBuilder();
            mainCode.Append(prefix.ToString().ToLower());
            mainCode.Append(" ");
            mainCode.Append(methodName);
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
    }
}