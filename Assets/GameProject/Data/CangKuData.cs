using Module;

namespace Project.Data
{
    public class CangKuData : ISqlData
    {
        /// <summary>
        /// CangKuData
        /// <summary>
        public int ID { get; set; }
        /// <summary>
        /// 物品ID
        /// <summary>
        public int PropData { get; set; }
        /// <summary>
        /// 是否仓库
        /// <summary>
        public int isCollection { get; set; }
        /// <summary>
        /// 仓库类型
        /// <summary>
        public int collectionType { get; set; }
        /// <summary>
        /// 仓库顺序
        /// <summary>
        public int collectionIndex { get; set; }
        /// <summary>
        /// 仓库显示数量
        /// <summary>
        public int collectionShowCount { get; set; }
    }
}
