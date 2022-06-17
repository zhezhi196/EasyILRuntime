using Module;

namespace Project.Data
{
    public class MapData : ISqlData
    {
        /// <summary>
        /// MapData
        /// <summary>
        public int ID { get; set; }
        /// <summary>
        /// 地图ID
        /// <summary>
        public int mapId { get; set; }
        /// <summary>
        /// 场景名字
        /// <summary>
        public string mapName { get; set; }
        /// <summary>
        /// 区域名字
        /// <summary>
        public string areaName { get; set; }
        /// <summary>
        /// 区域个数
        /// <summary>
        public int areaCount { get; set; }
    }
}
