/*
 * 脚本名称：ReturnValueBase
 * 脚本作者：FrameWork
 * 脚本作者：黄哲智
 * 创建时间：2018-03-22 17:46:07
 * 脚本作用：
*/

namespace Module
{
    public abstract class ReturnValueBase
    {
        public int code { get; set; }
        public string msg { get; set; }

        /// <summary>
        /// 判断是否成功
        /// </summary>
        /// <returns></returns>
        public bool GetState()
        {
            if (code == 200) return true;
            return false;
        }
    }
}

