using Module;

namespace Project.Data
{
    public class PropData : ISqlData
    {
        /// <summary>
        /// PropData
        /// <summary>
        public int ID { get; set; }
        /// <summary>
        /// 是否放入背包
        /// <summary>
        public int putBag { get; set; }
        /// <summary>
        /// 相同类型放一起
        /// <summary>
        public int sameFoder { get; set; }
        /// <summary>
        /// 是否显示背包数量
        /// <summary>
        public int bagShowCount { get; set; }
        /// <summary>
        /// 背包显示使用按钮
        /// <summary>
        public int bagShowUse { get; set; }
        /// <summary>
        /// 预制名
        /// <summary>
        public string prefab { get; set; }
        /// <summary>
        /// 标题
        /// <summary>
        public string title { get; set; }
        /// <summary>
        /// 描述
        /// <summary>
        public string des { get; set; }
        /// <summary>
        /// 背包描述
        /// <summary>
        public string bagDes { get; set; }
        /// <summary>
        /// 获取描述
        /// <summary>
        public string getDes { get; set; }
        /// <summary>
        /// 普通icon
        /// <summary>
        public string icon { get; set; }
        /// <summary>
        /// 高清icon
        /// <summary>
        public string highIcon { get; set; }
        /// <summary>
        /// 锁icon
        /// <summary>
        public string lockIcon { get; set; }
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
        /// <summary>
        /// 特殊类
        /// <summary>
        public string field { get; set; }
    }
}
