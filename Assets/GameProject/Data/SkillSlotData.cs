using Module;

namespace Project.Data
{
    public class SkillSlotData : ISqlData
    {
        /// <summary>
        /// SkillSlotData
        /// <summary>
        public int ID { get; set; }
        /// <summary>
        /// 序列
        /// <summary>
        public int index { get; set; }
        /// <summary>
        /// 槽类型
        /// <summary>
        public int type { get; set; }
        /// <summary>
        /// 加成
        /// <summary>
        public float plus { get; set; }
        public int Capacity { get; set; }
    }
}
