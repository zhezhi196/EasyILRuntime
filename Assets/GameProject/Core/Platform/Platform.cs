using Module;
using UnityEngine;

namespace SDK
{
    public class Platform
    {
        /// <summary>
        /// 运行设备型号
        /// </summary>
        public string Model { get; protected set; }

        public string Os { get; protected set; }

        /// <summary>
        /// 运行设备版本
        /// </summary>
        public string OsVersion { get; protected set; }
        /// <summary>
        /// 区域
        /// </summary>
        public string Region { get; protected set; }

        public string Imei { get; protected set; }

        public string Manufacturer { get; protected set; }

        public int GameID { get; protected set; }
        public int ChannelID { get; protected set; }
        public string Version { get; protected set; }

        public string UserID { get; protected set; }

        public static Platform Instance
        {
            get
            {
                if(mInstance == null)
                {
                    mInstance = new Platform();
                }
                return mInstance;
            }
        }

        internal static Platform mInstance;

        public virtual AsyncLoadProcess Init(AsyncLoadProcess process)
        {
            InitDeviceInofo();
            InitServerInfo();
            return process;
        }

        protected virtual void InitDeviceInofo()
        {
            Model = "huawei p30";
            Os = "Editor";
#if UNITY_ANDROID
            Os = "android";
            OsVersion = "10";
            ChannelID = 3;
#elif UNITY_IOS
            Os = "IOS";
            OsVersion = "10";
            if (Channel.channel == ChannelType.AppStoreCN)
            {
                ChannelID = 102;
            }
            else
            {
                ChannelID = 2;
            }
#elif UNITY_EDITOR
            Os = "Editor";
            OsVersion = "2018.4.3f";
            ChannelID = 0;
#endif
            Region = "CN";
            Imei = "Imei";
            Manufacturer = "Iphone";
            GameID = 103;
            Version = Application.version;
            UserID = PlayerInfo.pid;
        }

        public string Server_Ip { get; protected set; }
        public string Server_Port { get; protected set; }

        public string CDN_Url { get; protected set; }
        protected virtual void InitServerInfo()
        {
            //Server_Ip = "192.168.0.197";
            //Server_Ip = "127.0.0.1";
            Server_Ip = GameConfig.cdKeyUrl;
            Server_Port = GameConfig.cdKeyPort;

            //热更服务器
            //CDN_Url = "http://192.168.0.199:8080/sniper/AssetBundles/"; // 本地测试服务器
            CDN_Url = "http://sniper-hotdata.chenguanservice.com/"; //正式服务器
        }
    }
}
