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
        /// 关卡等级
        /// <summary>
        public int level { get; set; }
        /// <summary>
        /// 怪物生命上浮
        /// <summary>
        public float monsterHpUp { get; set; }
        /// <summary>
        /// 怪物攻击上浮
        /// <summary>
        public float monsterAttUp { get; set; }
        /// <summary>
        /// 玩家生命上浮
        /// <summary>
        public float playerHpUp { get; set; }
        /// <summary>
        /// 玩家攻击上浮
        /// <summary>
        public float playerAttUp { get; set; }
    }
}
