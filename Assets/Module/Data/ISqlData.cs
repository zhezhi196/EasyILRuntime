/*
 * 脚本名称：NetValueBase
 * 脚本作者：FrameWork
 * 脚本作者：黄哲智
 * 创建时间：2018-03-19 20:44:24
 * 脚本作用：
*/

using SQLite.Attributes;

namespace Module
{
    public interface ISqlData
    {
        [PrimaryKey, AutoIncrement] int ID { get; set; }
    }
}
