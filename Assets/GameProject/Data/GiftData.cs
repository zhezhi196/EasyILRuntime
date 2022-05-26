using Module;

namespace Project.Data
{
    public class GiftData : ISqlData, IAttribute<string>
    {
        /// <summary>
        /// GiftData
        /// <summary>
        public int ID { get; set; }
        /// <summary>
        /// 分支类型
        /// <summary>
        public int branchType { get; set; }
        /// <summary>
        /// 天赋类型(0属性天赋;1buff天赋)
        /// <summary>
        public int giftType { get; set; }
        /// <summary>
        /// 顺序
        /// <summary>
        public int index { get; set; }
        /// <summary>
        /// icon key
        /// <summary>
        public string icon { get; set; }
        /// <summary>
        /// 名称
        /// <summary>
        public int title { get; set; }
        /// <summary>
        /// 描述
        /// <summary>
        public int describe { get; set; }
        /// <summary>
        /// 参数1
        /// <summary>
        public float giftArg1 { get; set; }
        /// <summary>
        /// 参数2
        /// <summary>
        public float giftArg2 { get; set; }
        /// <summary>
        /// 参数3
        /// <summary>
        public float giftArg3 { get; set; }
        /// <summary>
        /// 货币类型
        /// <summary>
        public int costType { get; set; }
        /// <summary>
        /// 学习消耗
        /// <summary>
        public int cost { get; set; }
        /// <summary>
        /// 图片
        /// <summary>
        public string giftPic { get; set; }
        /// <summary>
        /// 视频
        /// <summary>
        public string giftVideo { get; set; }
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
