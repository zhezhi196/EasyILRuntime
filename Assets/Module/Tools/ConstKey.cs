/*
 * 脚本名称：ConstKey
 * 项目名称：FrameWork
 * 脚本作者：黄哲智
 * 创建时间：2018-01-18 09:17:23
 * 脚本作用：
*/

using System;
using UnityEngine;

namespace Module
{
    public static class ConstKey
    {
        #region 系统

        //---------------------Setting---------------------
        public const string MusicKey = "MUSIC_OPEN";
        public const string SoundKey = "SOUND_OPEN";
        public const string MusicMute = "Music_Mute";
        public const string EffectMute = "Effect_Mute";

        //---------------------分隔符-----------------
        public const char Spite0 = '_';
        public const char Spite1 = '|';
        public const char Spite2 = '^';
        public const string Cheng = "×";

        //---------------------路径-------------------

        //系统事件
        public const string OpenUI = "Event_OpenUI";
        public const string CloseUI = "Event_CloseUI";
        public const string EventKey = "EventSender_key";
        public const string UIOpenStart = "UIOpenStart";
        public const string UIOpenComplete = "UIOpenComplete";
        public const string UICloseStart = "UICloseStart";
        public const string Back = "Back";
        public const string languageLocalKey = "LanguageSetting";
        
        #endregion

        #region 项目设置
        
        /// <summary>
        /// 数据库等信息
        /// </summary>
        public const string SqlPassword = "qwerasdf";
        public const string Config_data = "Config.db";
        public const string Player_data = "Data.db";

        public static string GetChannelConfig(ChannelType channel)
        {
            return GetChannelConfig(channel.ToString());
        }
        
        public static string GetChannelConfig(string channel)
        {
            return $"Config/{channel}";
        }
        
        /// <summary>
        /// 根据类型得到对应的文件夹名
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetFolder(AssetLoad.AssetFolderType type)
        {
            switch (type)
            {
                case AssetLoad.AssetFolderType.Config:
                    return GetChannelConfig(Config.globleConfig.channel);
                case AssetLoad.AssetFolderType.Scenes:
                    return "Scenes";
                case AssetLoad.AssetFolderType.UI:
                    return "UI";
                case AssetLoad.AssetFolderType.Effect:
                    return "Effect";
                case AssetLoad.AssetFolderType.DB:
                    return "DbData/";
                case AssetLoad.AssetFolderType.Bundle:
                    return "Bundles";
                case AssetLoad.AssetFolderType.Altas:
                    return "Altas";
                case AssetLoad.AssetFolderType.WordsCheck:
                    return "chinese_dictionary.txt";
                case AssetLoad.AssetFolderType.Analytics:
                    return "Analytics";
                case AssetLoad.AssetFolderType.Material:
                    return "Mat";
                case AssetLoad.AssetFolderType.Video:
                    return "Video";
            }

            return string.Empty;
        }

        #endregion
    }
    
    /// <summary>
    /// 渠道类型
    /// </summary>
    [Flags]
    public enum ChannelType
    {
        googlePlay = 1 << 0,
        AppStore = 1 << 1,
        AppStoreCN = 1 << 2,
        Huawei = 1 << 3,
        TapTap = 1 << 4,
        TikTok = 1 << 5,
        TapTapHarmony = 1 << 6,
        AppStoreCNHarmony = 1 << 7,
        TikTokHarmony = 1 << 8,
    }
}