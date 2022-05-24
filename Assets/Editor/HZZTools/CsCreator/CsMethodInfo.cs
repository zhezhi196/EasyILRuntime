using System;
using System.Text;

namespace EditorModule
{
    public class CsMethodInfo
    {
        public StringBuilder content = new StringBuilder();

        public void Append(string content)
        {
            this.content.Append(content);
            this.content.Append(";\n");
        }
        
        public void BeginIf(string content)
        {
        }

        public void EndIf()
        {
        }

        public void BeginDo()
        {
        }

        public void EndDo(string whileContent)
        {
        }

        public void BegindWhile(string conent)
        {
        }

        public void EndWhile()
        {
        }

        public void BeginFor(string content)
        {
        }

        public void EndFor()
        {
        }

        public void BeginForeach(string content)
        {
        }

        public void EndForeach()
        {
        }

        public void BeginTry()
        {
        }

        public void EndTry()
        {
        }

        public void BeginCatch(string content)
        {
        }

        public void EndCatch()
        {
        }

        public void BeginFinaly()
        {
        }

        public void EndFinaly()
        {
        }

        public void BeginLock(string content)
        {
        }

        public void EndLock()
        {
        }

        public void AddReturn(string content)
        {
        }

        private StringBuilder GetString(StringBuilder str, int bracesIndex)
        {
            for (int i = 0; i < bracesIndex; i++)
            {
                str.Insert(0, "\t");
            }

            return str;
        }

        public StringBuilder GetContent(int braceIndex)
        {
            string qianzhui = string.Empty;
            for (int i = 0; i < braceIndex; i++)
            {
                qianzhui = "\t" + qianzhui;
            }

            string qiant = "\n" + qianzhui;

            return GetString(content.Replace("\n", qiant), braceIndex);
            //return GetString( braceIndex);
        }
    }
}