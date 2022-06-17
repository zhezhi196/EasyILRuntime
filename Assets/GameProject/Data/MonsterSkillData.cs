using Module;

namespace Project.Data
{
    public class MonsterSkillData : ISqlData
    {
        /// <summary>
        /// MonsterSkillData
        /// <summary>
        public int ID { get; set; }
        /// <summary>
        /// 权重
        /// <summary>
        public int weight { get; set; }
        /// <summary>
        /// cd
        /// <summary>
        public float cd { get; set; }
        /// <summary>
        /// 伤害系数
        /// <summary>
        public string damage { get; set; }
        /// <summary>
        /// 受击图片
        /// <summary>
        public int iconHurt { get; set; }
        /// <summary>
        /// 参数1
        /// <summary>
        public string mosterArg1 { get; set; }
        /// <summary>
        /// 参数2
        /// <summary>
        public string mosterArg2 { get; set; }
        /// <summary>
        /// 是否有玩家挣脱动画
        /// <summary>
        public int outAnim { get; set; }
        /// <summary>
        /// 是否有处决玩家动画
        /// <summary>
        public int excuteAnim { get; set; }
    }
}
