using UnityEngine;

namespace SDK
{
    public class PlatformAndroid : Platform
    {
        //protected override void InitDeviceInofo()
        //{
        //    base.InitDeviceInofo();
        //    Model = "Pc";
        //    OsVersion = "2018.4.3f";
        //    Region = "China";

        //    using (AndroidJavaClass jc = new AndroidJavaClass("com.chenguan.util.CommonUtil"))
        //    {
        //        using (AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("Instance"))
        //        {
        //            Debug.Log("getPlatformInfo ");
        //            string info = jo.Call<string>("getPlatformInfo");
        //            Debug.Log("getPlatformInfo retrun:--------------\n" + info);
        //            LitJson.JsonData jsonInfo = LitJson.JsonMapper.ToObject(info);
        //            Model = (string)jsonInfo["model"];
        //            Os = "Android";
        //            OsVersion = (string)jsonInfo["osVersion"];
        //            Region = (string)jsonInfo["region"];
        //            Manufacturer = (string)jsonInfo["manufacturer"];
        //            GameID = 101;
        //            ChannelID = 3;
        //            Version = Application.version;
        //            UserID = SystemInfo.deviceUniqueIdentifier;
        //        }
        //    }

        //    Debug.Log("deviceModel:" + SystemInfo.deviceModel);
        //    Debug.Log("deviceName:" + SystemInfo.deviceName);
        //    Debug.Log("deviceType:" + SystemInfo.deviceType);
        //    Debug.Log("deviceUniqueIdentifier:" + SystemInfo.deviceUniqueIdentifier);
        //    Debug.Log("operatingSystem:" + SystemInfo.operatingSystem);
        //    Debug.Log("operatingSystemFamily:" + SystemInfo.operatingSystemFamily);
        //    Debug.Log("unsupportedIdentifier:" + SystemInfo.unsupportedIdentifier);
        //}

        protected override void InitServerInfo()
        {
            base.InitServerInfo();
        }
    }
}
