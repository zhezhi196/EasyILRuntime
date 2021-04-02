using System;

namespace Module
{
    [Flags]
    public enum UIButtonFlag
    {
        Mute = 1,
        Analystics = 2,
    }

    public class ButtonConfig
    {
        public static string defaultAudio = "tongyongButton";
    }
}