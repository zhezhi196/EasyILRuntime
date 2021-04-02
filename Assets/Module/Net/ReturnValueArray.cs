/*
 * 脚本名称：ReturnValueArray
 * 脚本作者：FrameWork
 * 脚本作者：黄哲智
 * 创建时间：2018-03-22 17:48:06
 * 脚本作用：
*/


using System.Runtime.Serialization;

namespace Module
{
    [DataContract]
    public class ReturnValueArray<T> : ReturnValueBase<T> where T : ISqlData
    {
        public T[] data { get; set; }
    }
}
