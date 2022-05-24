using System;
using System.Text;
using Module;

namespace EditorModule
{
    public class CsProperty: CsField
    {
        /// <summary>
        /// 是否有get
        /// </summary>
        public bool isGet;
        /// <summary>
        /// 是否有set
        /// </summary>
        public bool isSet;
        public string getString;
        public string setString;

        public CsProperty(CsFieldPrefix prefix, Type type, string name) : base(prefix, type, name)
        {
        }

        public CsProperty(Type type, string name) : base(type, name)
        {
        }

        public override StringBuilder GetStr()
        {
            StringBuilder str = base.GetStr();
            
            str.Append(" {");
            if (isGet || (!isGet && !isSet))
            {
                string getContent = this.getString.IsNullOrEmpty() ? ";" : "{" + this.getString + "}";
                str.Append(" get");
                str.Append(getContent);
            }
            
            if (isSet)
            {
                string setContent = setString.IsNullOrEmpty() ? ";" : "{"+setString+"}";
                str.Append(" set");
                str.Append(setContent);
            }
            str.Append(" }\n");
            return str;
        }
    }
}