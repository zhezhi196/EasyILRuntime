using System;
using SDK;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Module
{
    [Serializable]
    public struct AnalysticEventInfo
    {
        [HorizontalGroup(),HideLabel]
        public ChannelType channel;
        [HorizontalGroup()]
        public E_AnalyticsType[] analystic;
    }
    [CreateAssetMenu(menuName = "HZZ/配置")]
    public class Config : ScriptableObject
    {
        public ChannelType channel;
        public string commonButtonAudio;
        public int ButtonAnalyticsType;
        public AnalysticEventInfo[] analystic;
        public FontInfo[] font;

        private static Config _config;
        public static Config globleConfig
        {
            get
            {
                if (_config == null)
                {
                    _config = GameObject.Find("GamePlay").GetComponent<IConfig>().config;
                }

                return _config;
            }
        }
#if UNITY_EDITOR
        public static void SetChannel(ChannelType channel)
        {
            globleConfig.channel = channel;
            AssetDatabase.Refresh();
        }
#endif
    }
}