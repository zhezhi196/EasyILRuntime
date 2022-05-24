using Module;

namespace Project.Data
{
    public class DropData : ISqlData
    {
        /// <summary>
        /// DropData
        /// <summary>
        public int ID { get; set; }
        /// <summary>
        /// 没有掉落的概率
        /// <summary>
        public float noDropRate { get; set; }
        /// <summary>
        /// 是否存档
        /// <summary>
        public int saveData { get; set; }
        /// <summary>
        /// 奖励ID
        /// <summary>
        public string rewardID { get; set; }
        /// <summary>
        /// 最小数量
        /// <summary>
        public string min { get; set; }
        /// <summary>
        /// 最大数量
        /// <summary>
        public string max { get; set; }
        /// <summary>
        /// 权重
        /// <summary>
        public string weight { get; set; }
    }
}
