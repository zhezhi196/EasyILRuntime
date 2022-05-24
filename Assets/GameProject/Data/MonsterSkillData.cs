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
    }
}
