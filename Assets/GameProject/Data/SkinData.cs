using Module;

namespace Project.Data
{
    public class SkinData : ISqlData
    {
        /// <summary>
        /// SkinData
        /// <summary>
        public int ID { get; set; }
        /// <summary>
        /// icon Key
        /// <summary>
        public string icon { get; set; }
        /// <summary>
        /// WeaponID
        /// <summary>
        public int weaponID { get; set; }
        /// <summary>
        /// 成就获得图标
        /// <summary>
        public string AchievementIcon { get; set; }
        /// <summary>
        /// 皮肤材质路径
        /// <summary>
        public string matPath { get; set; }
        /// <summary>
        /// 皮肤名字
        /// <summary>
        public int title { get; set; }
        /// <summary>
        /// 皮肤描述
        /// <summary>
        public int getDes { get; set; }
        /// <summary>
        /// 未解锁提醒
        /// <summary>
        public int lockDes { get; set; }
    }
}
