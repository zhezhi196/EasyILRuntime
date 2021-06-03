using System;
using Module.Set;
using UnityEngine;

namespace Module
{
    public enum QualityType
    {
        Low,
        Medium,
        High,
    }
    
    public class GraphicSetting : SettingConfig
    {
        public Quality quality = new Quality();
        public Fps fps = new Fps();

        public static (string name, QualityType quality, int fps) graphicSetting;
        public override void Init()
        {
            graphicSetting = GetQuality();
            quality.Init();
            fps.Init();
        }

        public override void Update()
        {
            fps.Update();
        }

        public (string name, QualityType quality, int fps) GetQuality()
        {
#if UNITY_EDITOR
            return ("Editor", QualityType.High, 60);
#elif UNITY_IOS
            return CheckIOS();
#elif UNITY_ANDROID
            return CheckAndroid();
#endif
        }

        private (string name, QualityType quality, int fps) CheckIOS()
        {
            string model = SystemInfo.deviceModel;
            if (model.Equals(@"iPhone3,1"))
            {
                return (@"iPhone 4", QualityType.Low, 30);
            }

            if (model.Equals(@"iPhone3,2"))
            {
                return (@"iPhone 4", QualityType.Low, 30);
            }

            if (model.Equals(@"iPhone3,3"))
            {
                return (@"iPhone 4", QualityType.Low, 30);
            }

            if (model.Equals(@"iPhone4,1"))
            {
                return (@"iPhone 4S", QualityType.Low, 30);
            }

            if (model.Equals(@"iPhone5,1"))
            {
                return (@"iPhone 5", QualityType.Low, 30);
            }

            if (model.Equals(@"iPhone5,2"))
            {
                return (@"iPhone 5 (GSM+CDMA)", QualityType.Low, 30);
            }

            if (model.Equals(@"iPhone5,3"))
            {
                return (@"iPhone 5c (GSM)", QualityType.Low, 30);
            }

            if (model.Equals(@"iPhone5,4"))
            {
                return (@"iPhone 5c (GSM+CDMA)", QualityType.Low, 30);
            }

            if (model.Equals(@"iPhone6,1"))
            {
                return (@"iPhone 5s (GSM)", QualityType.Low, 30);
            }

            if (model.Equals(@"iPhone6,2"))
            {
                return (@"iPhone 5s (GSM+CDMA)", QualityType.Low, 30);
            }

            if (model.Equals(@"iPhone7,1"))
            {
                return (@"iPhone 6 Plus", QualityType.Low, 30);
            }

            if (model.Equals(@"iPhone7,2"))
            {
                return (@"iPhone 6", QualityType.Low, 30);
            }

            //提高6s的帧率，看看能加载速度如何
            if (model.Equals(@"iPhone8,1"))
            {
                return (@"iPhone 6s", QualityType.Low, 60);
            }

            if (model.Equals(@"iPhone8,2"))
            {
                return (@"iPhone 6s Plus", QualityType.Low, 60);
            }

            if (model.Equals(@"iPhone8,4"))
            {
                return (@"iPhone SE", QualityType.Low, 60);
            }

            // 日行两款手机型号均为日本独占，可能使用索尼FeliCa支付方案而不是苹果支付
            if (model.Equals(@"iPhone9,1"))
            {
                return (@"国行、日版、港行iPhone 7", QualityType.Medium, 60);
            }

            if (model.Equals(@"iPhone9,2"))
            {
                return (@"港行、国行iPhone 7 Plus", QualityType.Medium, 60);
            }

            if (model.Equals(@"iPhone9,3"))
            {
                return (@"美版、台版iPhone 7", QualityType.Medium, 60);
            }

            if (model.Equals(@"iPhone9,4"))
            {
                return (@"美版、台版iPhone 7 Plus", QualityType.Medium, 60);
            }

            if (model.Equals(@"iPhone10,1"))
            {
                return (@"iPhone_8", QualityType.Medium, 60);
            }

            if (model.Equals(@"iPhone10,4"))
            {
                return (@"iPhone_8", QualityType.Medium, 60);
            }

            if (model.Equals(@"iPhone10,2"))
            {
                return (@"iPhone_8_Plus", QualityType.Medium, 60);
            }

            if (model.Equals(@"iPhone10,5"))
            {
                return (@"iPhone_8_Plus", QualityType.Medium, 60);
            }

            if (model.Equals(@"iPhone10,3"))
            {
                return (@"iPhone_X", QualityType.Medium, 60);
            }

            if (model.Equals(@"iPhone10,6"))
            {
                return (@"iPhone_X", QualityType.Medium, 60);
            }

            if (model.Equals(@"iPod1,1"))
            {
                return (@"iPod Touch 1G", QualityType.Low, 30);
            }

            if (model.Equals(@"iPod2,1"))
            {
                return (@"iPod Touch 2G", QualityType.Low, 30);
            }

            if (model.Equals(@"iPod3,1"))
            {
                return (@"iPod Touch 3G", QualityType.Low, 30);
            }

            if (model.Equals(@"iPod4,1"))
            {
                return (@"iPod Touch 4G", QualityType.Low, 30);
            }

            if (model.Equals(@"iPod5,1"))
            {
                return (@"iPod Touch (5 Gen)", QualityType.Low, 30);
            }

            if (model.Equals(@"iPad1,1"))
            {
                return (@"iPad", QualityType.Low, 30);
            }

            if (model.Equals(@"iPad1,2"))
            {
                return (@"iPad 3G", QualityType.Low, 30);
            }

            if (model.Equals(@"iPad2,1"))
            {
                return (@"iPad 2 (WiFi)", QualityType.Low, 30);
            }

            if (model.Equals(@"iPad2,2"))
            {
                return (@"iPad 2", QualityType.Low, 30);
            }

            if (model.Equals(@"iPad2,3"))
            {
                return (@"iPad 2 (CDMA)", QualityType.Low, 30);
            }

            if (model.Equals(@"iPad2,4"))
            {
                return (@"iPad 2", QualityType.Low, 30);
            }

            if (model.Equals(@"iPad2,5"))
            {
                return (@"iPad Mini (WiFi)", QualityType.Low, 30);
            }

            if (model.Equals(@"iPad2,6"))
            {
                return (@"iPad Mini", QualityType.Low, 30);
            }

            if (model.Equals(@"iPad2,7"))
            {
                return (@"iPad Mini (GSM+CDMA)", QualityType.Low, 30);
            }

            if (model.Equals(@"iPad3,1"))
            {
                return (@"iPad 3 (WiFi)", QualityType.Low, 30);
            }

            if (model.Equals(@"iPad3,2"))
            {
                return (@"iPad 3 (GSM+CDMA)", QualityType.Low, 30);
            }

            if (model.Equals(@"iPad3,3"))
            {
                return (@"iPad 3", QualityType.Low, 30);
            }

            if (model.Equals(@"iPad3,4"))
            {
                return (@"iPad 4 (WiFi)", QualityType.Low, 30);
            }

            if (model.Equals(@"iPad3,5"))
            {
                return (@"iPad 4", QualityType.Low, 30);
            }

            if (model.Equals(@"iPad3,6"))
            {
                return (@"iPad 4 (GSM+CDMA)", QualityType.Low, 30);
            }

            if (model.Equals(@"iPad4,1"))
            {
                return (@"iPad Air (WiFi)", QualityType.Medium, 30);
            }

            if (model.Equals(@"iPad4,2"))
            {
                return (@"iPad Air (Cellular)", QualityType.Medium, 30);
            }

            if (model.Equals(@"iPad4,4"))
            {
                return (@"iPad Mini 2 (WiFi)", QualityType.Medium, 30);
            }

            if (model.Equals(@"iPad4,5"))
            {
                return (@"iPad Mini 2 (Cellular)", QualityType.Medium, 30);
            }

            if (model.Equals(@"iPad4,6"))
            {
                return (@"iPad Mini 2", QualityType.Medium, 30);
            }

            if (model.Equals(@"iPad4,7"))
            {
                return (@"iPad Mini 3", QualityType.Medium, 30);
            }

            if (model.Equals(@"iPad4,8"))
            {
                return (@"iPad Mini 3", QualityType.Medium, 30);
            }

            if (model.Equals(@"iPad4,9"))
            {
                return (@"iPad Mini 3", QualityType.Medium, 30);
            }

            if (model.Equals(@"iPad5,1"))
            {
                return (@"iPad Mini 4 (WiFi)", QualityType.Medium, 60);
            }

            if (model.Equals(@"iPad5,2"))
            {
                return (@"iPad Mini 4 (LTE)", QualityType.Medium, 60);
            }

            if (model.Equals(@"iPad5,3"))
            {
                return (@"iPad Air 2", QualityType.Medium, 60);
            }

            if (model.Equals(@"iPad5,4"))
            {
                return (@"iPad Air 2", QualityType.Medium, 60);
            }

            if (model.Equals(@"iPad6,3"))
            {
                return (@"iPad Pro 9.7", QualityType.High, 60);
            }

            if (model.Equals(@"iPad6,4"))
            {
                return (@"iPad Pro 9.7", QualityType.High, 60);
            }

            if (model.Equals(@"iPad6,7"))
            {
                return (@"iPad Pro 12.9", QualityType.High, 60);
            }

            if (model.Equals(@"iPad6,8"))
            {
                return (@"iPad Pro 12.9", QualityType.High, 60);
            }

            if (model.Equals(@"AppleTV2,1"))
            {
                return (@"Apple TV 2", QualityType.Low, 30);
            }

            if (model.Equals(@"AppleTV3,1"))
            {
                return (@"Apple TV 3", QualityType.Low, 30);
            }

            if (model.Equals(@"AppleTV3,2"))
            {
                return (@"Apple TV 3", QualityType.Low, 30);
            }

            if (model.Equals(@"AppleTV5,3"))
            {
                return (@"Apple TV 4", QualityType.Low, 30);
            }

            if (model.Equals(@"i386"))
            {
                return (@"Simulator", QualityType.Low, 30);
            }

            if (model.Equals(@"x86_64"))
            {
                return (@"Simulator", QualityType.Low, 30);
            }


            if (model.Contains("iPhone"))
            {
                string str = model.Replace("iPhone", "");
                string[] strs = str.Split(',');
                int ver = System.Convert.ToInt32(strs[0]);
                //iPhone 11及以上
                if (ver >= 11)
                {
                    return (model, QualityType.High, 60);
                }
            }

            if (model.Contains("iPad"))
            {
                string str = model.Replace("iPad", "");
                string[] strs = str.Split(',');
                int ver = System.Convert.ToInt32(strs[0]);
                //iPad 7及以上
                if (ver >= 7)
                {
                    return (model, QualityType.High, 60);
                }
            }

            return ("Ios Unknow", QualityType.Low, 30);
        }

        private (string name, QualityType quality, int fps) CheckAndroid()
        {
            float base_store_size = 1000f;
            string name = SystemInfo.operatingSystem;
            QualityType quality = QualityType.Low;
            int fps = 60;
            GameDebug.LogErrorFormat("安卓手机配置: 系统内存{0},系统显存{1},系统主频{2}", SystemInfo.systemMemorySize, SystemInfo.graphicsMemorySize,
                SystemInfo.processorFrequency);
            //if (SystemInfo.systemMemorySize >= base_store_size * 4 && SystemInfo.processorFrequency >= 2200)
            //{
            //    quality = QualityType.High;
            //    fps = 60;
            //}
            //else if(SystemInfo.systemMemorySize >= base_store_size * 2 && SystemInfo.processorFrequency >= 1500)
            //{
            //    quality = QualityType.Medium;
            //    fps = 60;
            //}
            //else if (SystemInfo.systemMemorySize >= base_store_size * 1 && SystemInfo.processorFrequency >= 1200)
            //{
            //    quality = QualityType.Low;
            //    fps = 30;
            //}
            //else
            //{
            //    quality = QualityType.Low;
            //    fps = 30;
            //}

            if (SystemInfo.graphicsMemorySize >= 2000 && SystemInfo.systemMemorySize >= 6000)
            {
                quality = QualityType.High;
                fps = 60;
            }
            else if (SystemInfo.graphicsMemorySize >= 1000 && SystemInfo.systemMemorySize >= 3000)
            {
                quality = QualityType.Medium;
                fps = 60;
            }
            else
            {
                quality = QualityType.Low;
                fps = 30;
            }

            return (name, quality, fps);
        }
    }
}