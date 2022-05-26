using Module;

namespace Project.Data
{
    public class AchievementData : ISqlData
    {
        /// <summary>
        /// AchievementData
        /// <summary>
        public int ID { get; set; }
        /// <summary>
        /// 标题
        /// <summary>
        public string title { get; set; }
        /// <summary>
        /// 描述
        /// <summary>
        public string des { get; set; }
        /// <summary>
        /// 图标
        /// <summary>
        public string icon { get; set; }
        /// <summary>
        /// 目标数量
        /// <summary>
        public int targetCount { get; set; }
        /// <summary>
        /// 奖励
        /// <summary>
        public string reward { get; set; }
        /// <summary>
        /// 奖励数量
        /// <summary>
        public string rewardCount { get; set; }
        /// <summary>
        /// 字段
        /// <summary>
        public string field { get; set; }
        /// <summary>
        /// 获得描述
        /// <summary>
        public string getDes { get; set; }
        /// <summary>
        /// 前置ID
        /// <summary>
        public int lastid { get; set; }
    }
}
