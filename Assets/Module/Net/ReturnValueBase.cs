/*
 * 脚本名称：ReturnValueBase
 * 脚本作者：FrameWork
 * 脚本作者：黄哲智
 * 创建时间：2018-03-22 17:46:07
 * 脚本作用：
*/

namespace Module
{
    public abstract class ReturnValueBase<T> where T : ISqlData
    {
        public int result { get; set; }
        public int msgCode { get; set; }

        /// <summary>
        /// 判断是否成功
        /// </summary>
        /// <returns></returns>
        public bool GetState()
        {
            return result.ToBool();
        }
    }
}

