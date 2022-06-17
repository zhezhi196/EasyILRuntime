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
        /// 最大生命值
        /// <summary>
        public float hp { get; set; }
        /// <summary>
        /// 攻击力
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
        /// 暗杀直接杀死
        /// <summary>
        public int assKill { get; set; }
        /// <summary>
        /// 视野角度1
        /// <summary>
        public float visualRange1 { get; set; }
        /// <summary>
        /// 视野距离1
        /// <summary>
        public float visualDistance1 { get; set; }
        /// <summary>
        /// 警觉时间1
        /// <summary>
        public float alertTime1 { get; set; }
        /// <summary>
        /// 视野角度2
        /// <summary>
        public float visualRange2 { get; set; }
        /// <summary>
        /// 视野距离2
        /// <summary>
        public float visualDistance2 { get; set; }
        /// <summary>
        /// 警觉时间2
        /// <summary>
        public float alertTime2 { get; set; }
        /// <summary>
        /// 吼叫听力范围
        /// <summary>
        public float roarRange { get; set; }
        /// <summary>
        /// 头部硬直
        /// <summary>
        public int headStiff { get; set; }
        /// <summary>
        /// 身体硬直
        /// <summary>
        public int bodyStiff { get; set; }
        /// <summary>
        /// 跪地硬直
        /// <summary>
        public int kneelStiff { get; set; }
        /// <summary>
        /// 掉落
        /// <summary>
        public int dropId { get; set; }
        /// <summary>
        /// 巡逻时间
        /// <summary>
        public float xunTime { get; set; }
        /// <summary>
        /// 对峙最小范围
        /// <summary>
        public float standMin { get; set; }
        /// <summary>
        /// 对峙最大范围
        /// <summary>
        public float standMax { get; set; }
        /// <summary>
        /// 对峙时间
        /// <summary>
        public float standTime { get; set; }
        /// <summary>
        /// 奔跑速度系数
        /// <summary>
        public float runMoveK { get; set; }
        /// <summary>
        /// 重置CD概率
        /// <summary>
        public float skillCD { get; set; }
        /// <summary>
        /// 是否有玩家挣脱动画
        /// <summary>
        public int outAnim { get; set; }
        /// <summary>
        /// 是否有处决玩家动画
        /// <summary>
        public int excuteAnim { get; set; }
    }
}
