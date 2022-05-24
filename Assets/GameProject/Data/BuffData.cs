using Module;

namespace Project.Data
{
    public class BuffData : ISqlData
    {
        /// <summary>
        /// BuffData
        /// <summary>
        public int ID { get; set; }
        /// <summary>
        /// 名称
        /// <summary>
        public string name { get; set; }
        /// <summary>
        /// 最大数量
        /// <summary>
        public int maxCount { get; set; }
        /// <summary>
        /// 持续时间
        /// <summary>
        public float totalTime { get; set; }
        /// <summary>
        /// 间隔
        /// <summary>
        public float interval { get; set; }
    }
}
