using Module;

namespace Project.Data
{
    public class WeaponData : ISqlData, IAttribute<string>
    {
        /// <summary>
        /// WeaponData
        /// <summary>
        public int ID { get; set; }
        /// <summary>
        /// 图标id
        /// <summary>
        public string icon { get; set; }
        /// <summary>
        /// 仓库图标
        /// <summary>
        public string WarehouseIcon { get; set; }
        /// <summary>
        /// 武器名称
        /// <summary>
        public string name { get; set; }
        /// <summary>
        /// 子弹类型
        /// <summary>
        public int ammoType { get; set; }
        /// <summary>
        /// 物品id
        /// <summary>
        public int propId { get; set; }
        /// <summary>
        /// 最大生命
        /// <summary>
        public string hp { get; set; }
        /// <summary>
        /// 枪械攻击力
        /// <summary>
        public string gunAtt { get; set; }
        /// <summary>
        /// 移动速度
        /// <summary>
        public string moveSpeed { get; set; }
        /// <summary>
        /// 枪械爆头伤害
        /// <summary>
        public string gunHeadAttK { get; set; }
        /// <summary>
        /// 近战伤害
        /// <summary>
        public string meleeAtt { get; set; }
        /// <summary>
        /// 硬直
        /// <summary>
        public string paralysis { get; set; }
        /// <summary>
        /// 体力
        /// <summary>
        public string strength { get; set; }
        /// <summary>
        /// 能量
        /// <summary>
        public string energy { get; set; }
        /// <summary>
        /// 暴击概率
        /// <summary>
        public string violentAttP { get; set; }
        /// <summary>
        /// 暴击伤害系数
        /// <summary>
        public string violentAttK { get; set; }
        /// <summary>
        /// 下蹲移动系数
        /// <summary>
        public string crouchMovek { get; set; }
        /// <summary>
        /// 瞄准移动系数
        /// <summary>
        public string aimMoveK { get; set; }
        /// <summary>
        /// 奔跑移动系数
        /// <summary>
        public string runMoveK { get; set; }
        /// <summary>
        /// 硬直暴击率
        /// <summary>
        public string hardAttP { get; set; }
        /// <summary>
        /// 硬直暴击系数
        /// <summary>
        public string hardAttK { get; set; }
        /// <summary>
        /// 射速
        /// <summary>
        public string shotInterval { get; set; }
        /// <summary>
        /// 弹夹容量
        /// <summary>
        public string bulletCount { get; set; }
        /// <summary>
        /// 子弹上限
        /// <summary>
        public string bulletBag { get; set; }
        /// <summary>
        /// 精准度
        /// <summary>
        public string accurate { get; set; }
        /// <summary>
        /// 后坐力
        /// <summary>
        public string recoilForce { get; set; }
        /// <summary>
        /// 攻击范围
        /// <summary>
        public string attRange { get; set; }
        /// <summary>
        /// 暗杀伤害
        /// <summary>
        public string assDamage { get; set; }
    }
}
