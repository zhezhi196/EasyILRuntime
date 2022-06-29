using System.Globalization;
using System.Threading;
using Module.Set;
using UnityEngine;

namespace Module
{
    public static class Setting
    {
        public static AudioSetting audioSetting = new AudioSetting();
        public static GraphicSetting graphicSetting = new GraphicSetting();
        public static ControllerSetting controllerSetting = new ControllerSetting();
        public static UILocationSetting uiLocationSetting = new UILocationSetting();

        public static AsyncLoadProcess Init(AsyncLoadProcess process)
        {
            process.Reset();
            audioSetting.Init();
            graphicSetting.Init();
            controllerSetting.Init();
            uiLocationSetting.Init();
            process.SetComplete();
            return process;
        }

        public static void Update()
        {
            graphicSetting.Update();
        }
    }
}