using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using LitJson;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.U2D;

namespace Module
{
    [CreateAssetMenu(menuName = "HZZ/IconManager")]
    public class SpriteLoader : SerializedScriptableObject
    {
        private static Dictionary<string, Queue<(string,Action<Sprite>)>> callbackCache = new Dictionary<string, Queue<(string,Action<Sprite>)>>();

        private static List<IconInfo> loader;
        private static Dictionary<string, Sprite> cache = new Dictionary<string, Sprite>();
        private static string useKey = "pppoooiii";
        public static string commonAtlas = "Common";
        public static string dynamicAltas = "Dynamic";
        

        public static string AssetOutPutPath
        {
            get { return GetAssetOutPutPath(Config.globleConfig.channel.ToString()); }
        }

        public static string GetAssetOutPutPath(string channel)
        {
            return $"{ConstKey.GetChannelConfig(channel)}/Icon.txt";
        }

        public static AsyncLoadProcess Init(AsyncLoadProcess process)
        {
            process.IsDone = false;
            AssetLoad.PreloadAsset<TextAsset>(AssetOutPutPath, handle =>
            {
                if (!handle.Result.text.IsNullOrEmpty())
                {
                    loader = JsonMapper.ToObject<List<IconInfo>>(EncryptionHelper.Xor(handle.Result.text, useKey));
                }
                else
                {
                    loader = new List<IconInfo>();
                }
                AssetLoad.Release(handle);
                process.SetComplete();
            });
            return process;
        }

        public static void LoadIcon(string key, Action<Sprite> callback, bool isCache = true)
        {
            Sprite temp = null;
            if (cache.TryGetValue(key, out temp))
            {
                callback?.Invoke(temp);
            }
            else
            {
                var iconInfo = loader.Find(st => st.k == key);
                if (iconInfo == null)
                {
                    GameDebug.LogErrorFormat("无法找到图片{0}", key);
                    callback?.Invoke(null);
                }
                else
                {
                    string altasName = iconInfo.a;
                    Queue<(string,Action<Sprite>)> callbackResult = null;
                    if (!callbackCache.TryGetValue(altasName, out callbackResult))
                    {
                        callbackResult = new Queue<(string,Action<Sprite>)>();
                        callbackCache.Add(altasName, callbackResult);
                    }

                    callbackResult.Enqueue((key,callback));
                    if (callbackResult.Count == 1)
                    {
                        AssetLoad.PreloadAsset<SpriteAtlas>(
                            $"{ConstKey.GetFolder(AssetLoad.AssetFolderType.Altas)}/{iconInfo.a}.spriteatlas", handle =>
                            {
                                int callbackCount = callbackResult.Count;
                                for (int i = 0; i < callbackCount; i++)
                                {
                                    var item = callbackResult.Dequeue();
                                    Sprite sp = handle.Result.GetSprite(item.Item1);
                                    if (sp == null)
                                    {
                                        GameDebug.LogError("无法获取图片文件" + item.Item1);
                                    }
                                    item.Item2?.Invoke(sp);
                                    if (isCache && !cache.ContainsKey(key)) cache.Add(key, sp);
                                }

                                if (!altasName.Contains(commonAtlas) && !altasName.Contains(dynamicAltas))
                                {
                                    AssetLoad.Release(handle);
                                }
                            });
                    }
                }
            }
        }

        #region Release

#if UNITY_EDITOR
        public List<IconAsset> iconInfo;


        public void ReleaseText(string channel)
        {
            JsonData result = new JsonData();
            
            IconInfo info = new IconInfo();

            for (int i = 0; i < iconInfo.Count; i++)
            {
                var path = UnityEditor.AssetDatabase.GetAssetPath(iconInfo[i].icon);
                string header = "Assets/" + ConstKey.GetFolder(AssetLoad.AssetFolderType.Bundle) + "/";
                path = path.Remove(0, header.Length);
                
                //IconInfo loader = new IconInfo() {iconKey = iconInfo[i].iconKey, path = path};
                
                JsonData loader = new JsonData();
                loader[nameof(info.k)] = iconInfo[i].icon.name;
                loader[nameof(info.a)] = iconInfo[i].altasName;
                result.Add(loader);
            }

            string json = JsonMapper.ToJson(result);
            
            string abOutPutPath = $"{Application.dataPath}/{ConstKey.GetFolder(AssetLoad.AssetFolderType.Bundle)}/{GetAssetOutPutPath(channel)}";
            using (StreamWriter writer = new StreamWriter(abOutPutPath, false, Encoding.UTF8))
            {
                writer.Write(EncryptionHelper.Xor(json,useKey));
            }
        }
#endif

        #endregion
    }
}