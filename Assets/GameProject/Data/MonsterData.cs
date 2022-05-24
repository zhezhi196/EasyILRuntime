using Module;

namespace Project.Data
{
    public class MonsterData : ISqlData, IMonsterAttribute<float>
    {
        /// <summary>
        /// MonsterData
        /// <summary>
        public int ID { get; set; }
        /// <summary>
        /// 0 人类 1 怪物
        /// <summary>
        public int BiologicalType { get; set; }
        /// <summary>
        /// 0普通 1精英 2boss
        /// <summary>
        public int level { get; set; }
        /// <summary>
        /// 0近战 1远程
        /// <summary>
        public int type { get; set; }
        /// <summary>
        /// 最大生命值
        /// <summary>
        public float hp { get; set; }
        /// <summary>
        /// 最大攻击力
        /// <summary>
        public float att { get; set; }
        /// <summary>
        /// 移动速度
        /// <summary>
        public float moveSpeed { get; set; }
        /// <summary>
        /// 旋转速度
        /// <summary>
        public float rotateSpeed { get; set; }
        /// <summary>
        /// 最大硬直
        /// <summary>
        public int maxLag { get; set; }
    }
}
