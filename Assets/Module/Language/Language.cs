using System;
using System.Collections.Generic;
using LitJson;
using UnityEngine;

namespace Module
{
    public static class Language
    {
        public static Dictionary<string, LangInfo[]> info;
        public const string useKey = "langguage";
        private static SystemLanguage m_currLang = SystemLanguage.Unknown;

        public static SystemLanguage currentLanguage
        {
            get
            {
                if (LocalFileMgr.ContainKey(ConstKey.languageLocalKey))
                {
                    return (SystemLanguage) LocalFileMgr.GetInt(ConstKey.languageLocalKey);
                }
                else
                {
                    if (m_currLang == SystemLanguage.Unknown)
                    {
                        if (Channel.isChina)
                        {
                            ChangeLanguage(SystemLanguage.Chinese);
                        }
                        else
                        {
                            ChangeLanguage(SystemLanguage.English);
                        }
                    }

                    return m_currLang;
                }
            }
        }
        
        public static string AssetOutPutPath
        {
            get { return $"{ConstKey.GetFolder(AssetLoad.AssetFolderType.Config)}/lang.txt"; }
        }

        public static event Action<SystemLanguage> onLanguageChanged;

        public static AsyncLoadProcess Init(AsyncLoadProcess process)
        {
            process.IsDone = false;
            InitAction(() => process.SetComplete());
            return process;
        }

        public static void InitAction(Action callback)
        {
            if (info != null)
            {
                callback?.Invoke();
                return;
            }
#if UNITY_EDITOR
            var json = UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>($"Assets/{ConstKey.GetFolder(AssetLoad.AssetFolderType.Bundle)}/{AssetOutPutPath}").text;
            info = JsonMapper.ToObject<Dictionary<string, LangInfo[]>>(EncryptionHelper.Xor(json,useKey));
            callback?.Invoke();
#else
            AssetLoad.PreloadAsset<TextAsset>(AssetOutPutPath, handle =>
            {
                info = JsonMapper.ToObject<Dictionary<string, LangInfo[]>>(EncryptionHelper.Xor(handle.Result.text, useKey));
                callback?.Invoke();
            });
#endif
        }

        public static string GetID(string content,SystemLanguage language)
        {
            if (info == null)
            {
                InitAction(null);
            }
            int index = -1;

            foreach (KeyValuePair<string,LangInfo[]> keyValuePair in info)
            {
                if (index == -1)
                {
                    for (int i = 0; i < keyValuePair.Value.Length; i++)
                    {
                        if (keyValuePair.Value[i].l == language)
                        {
                            index = i;
                            if (keyValuePair.Value[i].c == content) return keyValuePair.Key;
                            break;
                        }
                    }
                }
                else
                {
                    if (keyValuePair.Value[index].c == content) return keyValuePair.Key;
                }
            }

            return null;
        }


        public static string GetContent(string ID, LabelFlag flag = LabelFlag.None)
        {
            if (info == null)
            {
                InitAction(null);
            }
            if (ID.IsNullOrEmpty()) return "文本缺失";
            LangInfo[] content = null;
            string idString = ID;
            if (!info.TryGetValue(idString, out content)) return "文本缺失 ID:" + ID;
            for (int i = 0; i < content.Length; i++)
            {
                if (content[i].l == currentLanguage)
                {
                    if (flag == LabelFlag.AllLower)
                    {
                        return content[i].c.ToLower();
                    }
                    else if (flag == LabelFlag.AllUper)
                    {
                        return content[i].c.ToUpper();
                    }
                    else if (flag == LabelFlag.None)
                    {
                        return content[i].c;
                    }
                }
            }
            return "文本缺失 ID:" + ID;
        }

        public static void ChangeLanguage(SystemLanguage lang)
        {
            m_currLang = lang;
            LocalFileMgr.SetInt(ConstKey.languageLocalKey, (int) lang);
            onLanguageChanged?.Invoke(lang);
        }
    }
}