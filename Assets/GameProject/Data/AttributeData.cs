using Module;

namespace Project.Data
{
    public class AttributeData : ISqlData
    {
        /// <summary>
        /// AttributeData
        /// <summary>
        public int ID { get; set; }
        /// <summary>
        /// 字段
        /// <summary>
        public string field { get; set; }
        /// <summary>
        /// 描述
        /// <summary>
        public string des { get; set; }
        /// <summary>
        /// 技能描述
        /// <summary>
        public string skillDes { get; set; }
        /// <summary>
        /// 评分
        /// <summary>
        public float gs { get; set; }
    }
}
