using Module;

namespace Project.Data
{
    public class DailyTaskData : ISqlData
    {
        /// <summary>
        /// DailyTaskData
        /// <summary>
        public int ID { get; set; }
        /// <summary>
        /// 图标
        /// <summary>
        public string icon { get; set; }
        /// <summary>
        /// 任务名称
        /// <summary>
        public string taskName { get; set; }
        /// <summary>
        /// 任务描述
        /// <summary>
        public string taskContent { get; set; }
        /// <summary>
        /// 是否必现
        /// <summary>
        public int autoMustShow { get; set; }
        /// <summary>
        /// 字段
        /// <summary>
        public string field { get; set; }
        /// <summary>
        /// 是否能双倍
        /// <summary>
        public int canDouble { get; set; }
        /// <summary>
        /// 任务概率
        /// <summary>
        public string rate { get; set; }
        /// <summary>
        /// 任务目标数量
        /// <summary>
        public string taskComplete { get; set; }
        /// <summary>
        /// 获得文本
        /// <summary>
        public string getDes { get; set; }
        /// <summary>
        /// 奖励数量
        /// <summary>
        public string rewardCount { get; set; }
        /// <summary>
        /// 奖励概率
        /// <summary>
        public string rewardRate { get; set; }
        /// <summary>
        /// 奖励
        /// <summary>
        public string reward { get; set; }
    }
}
