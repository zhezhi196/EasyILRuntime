using UnityEngine;
using UnityEditor;
using System.Runtime.InteropServices;

namespace SDK
{
    public class PlatformIOS : Platform
    {
#if UNITY_IOS
        [DllImport("__Internal")]
        public static extern string GetPlatformInfo();
        [DllImport("__Internal")]
        public static extern string GetServerInfo();

        protected override void InitDeviceInofo()
        {
            string info = GetPlatformInfo();
            Debug.Log("getPlatformInfo retrun:--------------\n" + info);
            LitJson.JsonData jsonInfo = LitJson.JsonMapper.ToObject(info);
            Model = (string)jsonInfo["model"];
            Os = "IOS";
            OsVersion = (string)jsonInfo["osVersion"];
            Region = (string)jsonInfo["region"];
            Manufacturer = (string)jsonInfo["manufacturer"];
            GameID = 101;
            ChannelID = 2;
            Version = Application.version;
            UserID = SystemInfo.deviceUniqueIdentifier;
        }

        protected override void InitServerInfo()
        {
            string info = GetServerInfo();
            Debug.Log("getPlatformInfo retrun:--------------\n" + info);
            LitJson.JsonData jsonInfo = LitJson.JsonMapper.ToObject(info);
            Server_Ip = (string)jsonInfo["server_ip"];
            Server_Port = (string)jsonInfo["server_port"];
            CDN_Url = (string)jsonInfo["cdn_url"];
            Debug.Log(string.Format("getPlatformInfo retrun:[ip={0}][port={1}][cdn={2}]",
                Server_Ip, Server_Port, CDN_Url));
        }
#endif
    }
}
