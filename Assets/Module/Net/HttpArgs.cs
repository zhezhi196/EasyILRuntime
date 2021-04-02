/*
 * 脚本名称：HttpArgs
 * 项目名称：FrameWork
 * 脚本作者：黄哲智
 * 创建时间：2018-04-18 15:12:56
 * 脚本作用：
*/


using System.Collections.Generic;

namespace Module
{
    public class HttpArgs
    {
        private Dictionary<string, object> args = new Dictionary<string, object>();

        public int Length
        {
            get { return args.Count; }
        }

        public Dictionary<string, object> Paramaters
        {
            get { return args; }
        }

        public void AddArgs(string name, object value)
        {
            if (!args.ContainsKey(name))
            {
                args.Add(name, value);
            }
        }

        public void RemoveArgs(string name)
        {
            if (args.ContainsKey(name))
            {
                args.Remove(name);
            }
        }
    }

}
