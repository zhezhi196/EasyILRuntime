/*
 * 脚本名称：IProto
 * 项目名称：FrameWork
 * 脚本作者：黄哲智
 * 创建时间：2018-01-18 17:12:30
 * 脚本作用：
*/

namespace Module
{
    public interface IProto
    {
        ushort ProtoID { get; }

        byte[] ToArray();
    }
}
