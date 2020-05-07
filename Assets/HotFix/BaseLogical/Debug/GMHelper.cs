using System.Collections;
using System.Collections.Generic;
using Module;
using UnityEngine;
using xasset;

namespace HotFix
{
    public static class GMHelper 
    {
        public static void Init()
        {
            if (!Configuration.isGM)

            {
                GMConsole.Instance.Init();
                GMConsole.Instance.RegisterCommandCallback("cache", args =>
                {
                    PlayerPrefs.DeleteAll();
                    return "cache";
                }, "清除缓存成功");
            }

        }
    }
}
