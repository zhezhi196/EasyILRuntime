using Module;

namespace Project.Data
{
    public class AideSkillData : ISqlData, IAttribute<string>
    {
        /// <summary>
        /// AideSkillData
        /// <summary>
        public int ID { get; set; }
        /// <summary>
        /// 切换状态
        /// <summary>
        public int switchStation { get; set; }
        /// <summary>
        /// 技能名称
        /// <summary>
        public string skillName { get; set; }
        /// <summary>
        /// 使用者
        /// <summary>
        public int owner { get; set; }
        /// <summary>
        /// 名字
        /// <summary>
        public string title { get; set; }
        /// <summary>
        /// 描述
        /// <summary>
        public string des { get; set; }
        /// <summary>
        /// icon
        /// <summary>
        public string icon { get; set; }
        /// <summary>
        /// 伤害系数
        /// <summary>
        public string value { get; set; }
        /// <summary>
        /// cd
        /// <summary>
        public float cd { get; set; }
        /// <summary>
        /// 持续时间
        /// <summary>
        public float duationTime { get; set; }
        /// <summary>
        /// 最大攻击人数
        /// <summary>
        public int maxHurtCount { get; set; }
        /// <summary>
        /// 硬直
        /// <summary>
        public int lagValue { get; set; }
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
