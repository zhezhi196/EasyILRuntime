using Module;

namespace Project.Data
{
    public class Cdkey : ISqlData
    {
        /// <summary>
        /// Cdkey
        /// <summary>
        public int ID { get; set; }
        /// <summary>
        /// 目标ID
        /// <summary>
        public string lockID { get; set; }
    }
}
