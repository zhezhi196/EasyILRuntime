using Module;

namespace Project.Data
{
    public class ServiceData : ISqlData
    {
        /// <summary>
        /// ServiceData
        /// <summary>
        public int ID { get; set; }
        /// <summary>
        /// icon Key
        /// <summary>
        public string icon { get; set; }
        /// <summary>
        /// 描述
        /// <summary>
        public string des { get; set; }
        /// <summary>
        /// 获取时的描述
        /// <summary>
        public string getdes { get; set; }
        /// <summary>
        /// 类名
        /// <summary>
        public string field { get; set; }
        /// <summary>
        /// 状态
        /// <summary>
        public int switchStation { get; set; }
        /// <summary>
        /// 全局(1 :是)
        /// <summary>
        public int global { get; set; }
        /// <summary>
        /// 标题
        /// <summary>
        public string title { get; set; }
    }
}
