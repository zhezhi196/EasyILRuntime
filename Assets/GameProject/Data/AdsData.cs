using Module;

namespace Project.Data
{
    public class AdsData : IAdsData
    {
        /// <summary>
        /// AdsData
        /// <summary>
        public int ID { get; set; }
        /// <summary>
        /// 类型
        /// <summary>
        public int adsType { get; set; }
        /// <summary>
        /// 同类型排序
        /// <summary>
        public int level { get; set; }
        /// <summary>
        /// 奖励ID
        /// <summary>
        public string rewardID { get; set; }
        /// <summary>
        /// 奖励数量
        /// <summary>
        public string rewardCount { get; set; }
        /// <summary>
        /// 礼包名称
        /// <summary>
        public string title { get; set; }
        /// <summary>
        /// 礼包图片
        /// <summary>
        public string icon { get; set; }
        /// <summary>
        /// 描述
        /// <summary>
        public string des { get; set; }
        /// <summary>
        /// 礼包描述
        /// <summary>
        public string getDes { get; set; }
        /// <summary>
        /// 状态
        /// <summary>
        public int switchStation { get; set; }
        /// <summary>
        /// 最大购买数量
        /// <summary>
        public int maxPay { get; set; }
    }
}
