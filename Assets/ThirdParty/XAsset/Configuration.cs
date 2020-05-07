
using UnityEngine;

namespace xasset
{
    public static class Configuration
    {
        public static bool isEditor
        {
            get { return Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor; }
        }

        public static Channel channer;

        public static bool isLog
        {
            get { return channer == Channel.LocalDebug || channer == Channel.RemoteServer; }
        }

        public static bool isGM
        {
            get { return channer == Channel.LocalDebug || channer == Channel.RemoteServer; }
        }
    }
}