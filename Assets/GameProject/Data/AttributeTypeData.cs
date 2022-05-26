using Module;

namespace Project.Data
{
    public class AttributeTypeData : IAttribute<int>, ISqlData
    {
        /// <summary>
        /// AttributeTypeData
        /// <summary>
        public int ID { get; set; }
        /// <summary>
        /// 目标ID
        /// <summary>
        public int targetID { get; set; }
        /// <summary>
        /// 最大生命
        /// <summary>
        public int hp { get; set; }
        /// <summary>
        /// 枪械攻击力
        /// <summary>
        public int gunAtt { get; set; }
        /// <summary>
        /// 移动速度
        /// <summary>
        public int moveSpeed { get; set; }
        /// <summary>
        /// 枪械爆头伤害
        /// <summary>
        public int gunHeadAttK { get; set; }
        /// <summary>
        /// 近战伤害
        /// <summary>
        public int meleeAtt { get; set; }
        /// <summary>
        /// 硬直
        /// <summary>
        public int paralysis { get; set; }
        /// <summary>
        /// 体力
        /// <summary>
        public int strength { get; set; }
        /// <summary>
        /// 能量
        /// <summary>
        public int energy { get; set; }
        /// <summary>
        /// 暴击概率
        /// <summary>
        public int violentAttP { get; set; }
        /// <summary>
        /// 暴击伤害系数
        /// <summary>
        public int violentAttK { get; set; }
        /// <summary>
        /// 下蹲移动系数
        /// <summary>
        public int crouchMovek { get; set; }
        /// <summary>
        /// 瞄准移动系数
        /// <summary>
        public int aimMoveK { get; set; }
        /// <summary>
        /// 奔跑移动系数
        /// <summary>
        public int runMoveK { get; set; }
        /// <summary>
        /// 硬直暴击率
        /// <summary>
        public int hardAttP { get; set; }
        /// <summary>
        /// 硬直暴击系数
        /// <summary>
        public int hardAttK { get; set; }
        /// <summary>
        /// 射速
        /// <summary>
        public int shotInterval { get; set; }
        /// <summary>
        /// 弹夹容量
        /// <summary>
        public int bulletCount { get; set; }
        /// <summary>
        /// 子弹上限
        /// <summary>
        public int bulletBag { get; set; }
        /// <summary>
        /// 精准度
        /// <summary>
        public int accurate { get; set; }
        /// <summary>
        /// 后坐力
        /// <summary>
        public int recoilForce { get; set; }
        /// <summary>
        /// 攻击范围
        /// <summary>
        public int attRange { get; set; }
        /// <summary>
        /// 暗杀伤害
        /// <summary>
        public int assDamage { get; set; }
    }
}
