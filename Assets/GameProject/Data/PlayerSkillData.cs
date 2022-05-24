using Module;

namespace Project.Data
{
    public class PlayerSkillData : ISqlData, IAttribute<string>
    {
        /// <summary>
        /// PlayerSkillData
        /// <summary>
        public int ID { get; set; }
        public int switchStation { get; set; }
        /// <summary>
        /// 描述
        /// <summary>
        public string des { get; set; }
        /// <summary>
        /// icon
        /// <summary>
        public string icon { get; set; }
        public string skillName { get; set; }
        /// <summary>
        /// 品质
        /// <summary>
        public int quility { get; set; }
        /// <summary>
        /// 形态
        /// <summary>
        public int morphology { get; set; }
        /// <summary>
        /// 武器
        /// <summary>
        public int weapon { get; set; }
        /// <summary>
        /// 名字
        /// <summary>
        public string title { get; set; }
        /// <summary>
        /// 类型0: 主动技能 1 buff技能 2光环技能 3属性技能4其他被动技能
        /// <summary>
        public int type { get; set; }
        /// <summary>
        /// 槽类型
        /// <summary>
        public int slotType { get; set; }
        /// <summary>
        /// 等级
        /// <summary>
        public int level { get; set; }
        /// <summary>
        /// 随机权重
        /// <summary>
        public int randomWeight { get; set; }
        /// <summary>
        /// 背面所需数量
        /// <summary>
        public int backPices { get; set; }
        /// <summary>
        /// 随机到背面的概率权重
        /// <summary>
        public int randomBack { get; set; }
        /// <summary>
        /// 正面所需数量
        /// <summary>
        public int frontPices { get; set; }
        /// <summary>
        /// 升级所需经验 -1代表满级了
        /// <summary>
        public int updateExp { get; set; }
        /// <summary>
        /// 背面经验
        /// <summary>
        public int backExp { get; set; }
        /// <summary>
        /// 正面经验
        /// <summary>
        public int frontExp { get; set; }
        /// <summary>
        /// cd
        /// <summary>
        public float cd { get; set; }
        /// <summary>
        /// 伤害
        /// <summary>
        public string damage { get; set; }
        /// <summary>
        /// 硬直
        /// <summary>
        public string lag { get; set; }
        /// <summary>
        /// 最大攻击人数
        /// <summary>
        public int maxHurtCount { get; set; }
        /// <summary>
        /// buff时间
        /// <summary>
        public float buffTime { get; set; }
        /// <summary>
        /// 光环半径
        /// <summary>
        public float haloRadius { get; set; }
        /// <summary>
        /// 光环数值
        /// <summary>
        public string haloValue { get; set; }
        /// <summary>
        /// 其他数值
        /// <summary>
        public float otherValue1 { get; set; }
        /// <summary>
        /// 其他数值
        /// <summary>
        public float otherValue2 { get; set; }
        /// <summary>
        /// 其他数值
        /// <summary>
        public float otherValue3 { get; set; }
        /// <summary>
        /// 生命值
        /// <summary>
        public string hp { get; set; }
        /// <summary>
        /// 攻击力
        /// <summary>
        public string att { get; set; }
        /// <summary>
        /// 移动速度
        /// <summary>
        public string moveSpeed { get; set; }
        /// <summary>
        /// 最大怒气值
        /// <summary>
        public string anger { get; set; }
        /// <summary>
        /// 暴击率
        /// <summary>
        public string critical { get; set; }
        /// <summary>
        /// 暴击伤害
        /// <summary>
        public string criticalDamage { get; set; }
        /// <summary>
        /// 旋转速度
        /// <summary>
        public string rotateSpeed { get; set; }
        /// <summary>
        /// 怒气圈半径
        /// <summary>
        public string angerRadius { get; set; }
        /// <summary>
        /// 怒气增长速度
        /// <summary>
        public string angerUpSpeed { get; set; }
        /// <summary>
        /// 怒气下降速度
        /// <summary>
        public string angerDownSpeed { get; set; }
        /// <summary>
        /// 狂暴时间
        /// <summary>
        public string angryTime { get; set; }
        /// <summary>
        /// 攻击速度
        /// <summary>
        public string attSpeed { get; set; }
        /// <summary>
        /// 远程攻击射程
        /// <summary>
        public string shotRange { get; set; }
    }
}
