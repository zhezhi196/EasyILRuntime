using System;
using Sirenix.OdinInspector;

namespace Module
{
    [Flags]
    public enum UIButtonFlag
    {
        NoAudio = 1,
        Analystics = 2,
    }

    public enum NoChannelLogical
    {
        None,
        Hide,
        Uninteractive
    }

    [Serializable]
    public class ButtonConfig
    {
        public static string defaultAudio = "tongyongButton";
        public UIButtonFlag flag;
        public string audio;
        public int analysticsKey;
        public ChannelType channel = (ChannelType) (-1);
        public NoChannelLogical logical;
    }
}