using Module;

namespace Project.Data
{
    public class IapData : IRwardData,ICurrencyData
    {
        /// <summary>
        /// IapData
        /// <summary>
        public int ID { get; set; }
        /// <summary>
        /// 类型
        /// <summary>
        public int type { get; set; }
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
        /// sku
        /// <summary>
        public string sku { get; set; }
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
        /// <summary>
        /// 价格(在手机上仅供显示)
        /// <summary>
        public string showPrice { get; set; }
        /// <summary>
        /// 原始价格(仅显示)
        /// <summary>
        public float orignPrice { get; set; }
        /// <summary>
        /// 原始数量(仅显示)
        /// <summary>
        public int orignCount { get; set; }
        /// <summary>
        /// 折扣值(仅显示)
        /// <summary>
        public float sale { get; set; }
        /// <summary>
        /// 刷新时间
        /// <summary>
        public int deadTune { get; set; }
    }
}
