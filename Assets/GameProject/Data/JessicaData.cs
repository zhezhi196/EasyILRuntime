using Module;

namespace Project.Data
{
    public class JessicaData : IGameRewardData
    {
        /// <summary>
        /// JessicaData
        /// <summary>
        public int ID { get; set; }
        /// <summary>
        /// 类型(1补给)
        /// <summary>
        public int type { get; set; }
        /// <summary>
        /// 杰西卡类型
        /// <summary>
        public int jessicaType { get; set; }
        /// <summary>
        /// 奖励ID
        /// <summary>
        public string rewardId { get; set; }
        /// <summary>
        /// 广告ID
        /// <summary>
        public int adsId { get; set; }
        /// <summary>
        /// 奖励数量
        /// <summary>
        public string rewardCount { get; set; }
        /// <summary>
        /// 消耗合金
        /// <summary>
        public int alloyCost { get; set; }
        /// <summary>
        /// 消耗零件
        /// <summary>
        public int partsCost { get; set; }
        /// <summary>
        /// 消耗记忆碎片
        /// <summary>
        public int memoryCost { get; set; }
        /// <summary>
        /// 商店图标
        /// <summary>
        public string Icon { get; set; }
        /// <summary>
        /// 标题
        /// <summary>
        public string title { get; set; }
        /// <summary>
        /// 描述
        /// <summary>
        public string des { get; set; }
        /// <summary>
        /// 折扣
        /// <summary>
        public float off { get; set; }
    }
}
