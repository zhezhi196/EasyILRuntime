using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace xasset
{
    public static class Versions
    {
        public static Dictionary<string, string> data = new Dictionary<string, string>();
        public const string versionFile = "download.txt";
        public const string appVersionFile = "app_ver.txt";
        public const string versionText = "versions.txt";
        private const char splitKey = '=';

        public static bool Load()
        {
            string relativeUpdatePath1 = Assets.GetRelativeUpdatePath(appVersionFile);
            if (File.Exists(relativeUpdatePath1) && new Version(File.ReadAllText(relativeUpdatePath1)) < new Version(Application.version))
            {
                Clear();
            }

            string relativeUpdatePath2 = Assets.GetRelativeUpdatePath(versionFile);
            if (!File.Exists(relativeUpdatePath2)) return false;

            using (StreamReader streamReader = new StreamReader(relativeUpdatePath2))
            {
                string str;
                while ((str = streamReader.ReadLine()) != null)
                {
                    if (!(str == string.Empty))
                    {
                        string[] strArray = str.Split(splitKey);
                        if (strArray.Length > 1)
                            data.Add(strArray[0], strArray[1]);
                    }
                }
            }

            return true;
        }

        public static void Clear()
        {
            data.Clear();
            string directoryName = Path.GetDirectoryName(Assets.updatePath);
            if (!Directory.Exists(directoryName)) return;

            Directory.Delete(directoryName, true);
        }

        public static void Set(string key, string version)
        {
            data[key] = version;
        }

        public static string Get(string key)
        {
            string str;
            data.TryGetValue(key, out str);
            return str;
        }

        public static void Save()
        {
            string directoryName = Path.GetDirectoryName(Assets.updatePath);
            if (!Directory.Exists(directoryName)) Directory.CreateDirectory(directoryName);
            string path = Assets.updatePath + versionFile;
            if (File.Exists(path)) File.Delete(path);

            if (data.Count > 0)
            {
                using (StreamWriter streamWriter = new StreamWriter(path))
                {
                    foreach (KeyValuePair<string, string> keyValuePair in data)
                    {
                        streamWriter.WriteLine(keyValuePair.Key + splitKey + keyValuePair.Value);
                    }

                    streamWriter.Flush();
                    streamWriter.Close();
                }
            }

            string relativeUpdatePath = Assets.GetRelativeUpdatePath(appVersionFile);
            if (File.Exists(relativeUpdatePath)) File.Delete(relativeUpdatePath);
            
            File.WriteAllText(relativeUpdatePath, Application.version);
        }
    }
}