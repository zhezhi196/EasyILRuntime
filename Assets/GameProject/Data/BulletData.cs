using Module;

namespace Project.Data
{
    public class BulletData : ISqlData
    {
        /// <summary>
        /// BulletData
        /// <summary>
        public int ID { get; set; }
        /// <summary>
        /// 类型
        /// <summary>
        public int type { get; set; }
        /// <summary>
        /// 物品ID
        /// <summary>
        public int propId { get; set; }
        /// <summary>
        /// 制造消耗类型
        /// <summary>
        public string costType { get; set; }
        /// <summary>
        /// 制造一发子弹的价格
        /// <summary>
        public string costCount { get; set; }
        /// <summary>
        /// 制造子弹一组多少个
        /// <summary>
        public int createCount { get; set; }
        /// <summary>
        /// 看广告得物品
        /// <summary>
        public int adId { get; set; }
        /// <summary>
        /// 描述
        /// <summary>
        public string Des { get; set; }
    }
}
