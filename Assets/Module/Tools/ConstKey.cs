/*
 * 脚本名称：ConstKey
 * 项目名称：FrameWork
 * 脚本作者：黄哲智
 * 创建时间：2018-01-18 09:17:23
 * 脚本作用：
*/

using System;

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

        //---------------------路径-------------------
        public const string JsonConfigPath = "Config/{0}";

        public const string bundlePoint = "GamePlay/BundleUpdate";

        //系统事件
        public const string OpenUI = "Event_OpenUI";
        public const string CloseUI = "Event_CloseUI";

        public const string EventKey = "EventSender_key";
        
        public const string SqlPassword = "";
        public const string Config_data = "Config.db";
        public const string Player_data = "Data.db";

        public const string UIOpenComplete = "UIOpenComplete";
        public const string UICloseStart = "UICloseStart";

        public const string playerTag = "Player";
        
        #endregion

    }
}