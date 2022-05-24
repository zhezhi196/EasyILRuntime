using System;
using System.Text;
using Module;

namespace EditorModule
{
    public class CsField
    {
        /// <summary>
        /// attribute
        /// </summary>
        public string[] attribute;
        /// <summary>
        /// public private
        /// </summary>
        public CsFieldPrefix prefix;
        /// <summary>
        /// 类型
        /// </summary>
        public Type type;
        /// <summary>
        /// 名称
        /// </summary>
        public string fieldName;

        public CsField(CsFieldPrefix prefix, Type type, string name)
        {
            this.prefix = prefix;
            this.type = type;
            this.fieldName = name;
        }
        
        public CsField(Type type, string name)
        {
            this.type = type;
            this.fieldName = name;
        }

        public virtual StringBuilder GetStr()
        {
            StringBuilder result = new StringBuilder();
            
            if (!attribute.IsNullOrEmpty())
            {
                for (int i = 0; i < attribute.Length; i++)
                {
                    result.Append("[");
                    result.Append(attribute[i]);
                    result.Append("]");
                }
            }

            if (!attribute.IsNullOrEmpty())
            {
                result.Append(" ");
            }

            result.Append(prefix.ToString().ToLower());
            result.Append(" ");
            result.Append(CsCode.GetTypeName(type));
            result.Append(" ");
            result.Append(fieldName);
            return result;
        }
    }
}