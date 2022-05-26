using Module;

namespace Project.Data
{
    public class MissionData : ISqlData
    {
        /// <summary>
        /// MissionData
        /// <summary>
        public int ID { get; set; }
        /// <summary>
        /// 目标ID
        /// <summary>
        public int difficulte { get; set; }
        /// <summary>
        /// 模式
        /// <summary>
        public int mode { get; set; }
        /// <summary>
        /// 图标
        /// <summary>
        public string icon { get; set; }
        /// <summary>
        /// 名称
        /// <summary>
        public string title { get; set; }
        /// <summary>
        /// 描述
        /// <summary>
        public string des { get; set; }
        /// <summary>
        /// 解锁描述
        /// <summary>
        public string unlockDes { get; set; }
        /// <summary>
        /// 解锁关卡
        /// <summary>
        public int unlockMission { get; set; }
        /// <summary>
        /// 解锁广告ID
        /// <summary>
        public int unlockIap { get; set; }
    }
}
