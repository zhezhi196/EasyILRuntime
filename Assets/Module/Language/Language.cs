using FrameWork;
using System;
using System.Collections.Generic;
using Sirenix.Utilities;
using UnityEngine;

namespace Module
{
    public static class Language
    {
        private static bool isInit;
        private static Dictionary<int, Dictionary<SystemLanguage,string>> language = new Dictionary<int, Dictionary<SystemLanguage,string>>();
        public static event Action<SystemLanguage> onLanguageChanged;
        private static SystemLanguage m_currLang = SystemLanguage.Unknown;
        private static string localKey="LanguageSetting";

        public static SystemLanguage currentLanguage
        {
            get
            {
                if (LocalFileMgr.ContainKey(localKey))
                {
                    return (SystemLanguage) LocalFileMgr.GetInt(localKey);
                }
                else
                {
                    if (m_currLang == SystemLanguage.Unknown)
                    {
                        ChangeLanguage(Application.systemLanguage);
                    }

                    return m_currLang;
                }
            }
        }

        private static void Init()
        {
            LangInfo[] lang = DataMgr.Instance.GetDataArray<LangInfo>(false);
            if (!lang.IsNullOrEmpty())
            {
                for (int i = 0; i < lang.Length; i++)
                {
                    LangInfo temp = lang[i];
                    if (temp.ID != -1 && !language.ContainsKey(temp.ID))
                    {
                        Dictionary<SystemLanguage, string> lanContent = new Dictionary<SystemLanguage, string>();
                        lanContent.Add(SystemLanguage.ChineseSimplified, temp.CN);
                        lanContent.Add(SystemLanguage.Chinese, temp.CN);
                        lanContent.Add(SystemLanguage.English, temp.EN);
                        lanContent.Add(SystemLanguage.Russian, temp.Ru); 
                        lanContent.Add(SystemLanguage.Spanish, temp.Sp);
                        lanContent.Add(SystemLanguage.Portuguese, temp.Po);
                        language.Add(temp.ID, lanContent);
                    }
                }

                isInit = true;
            }
        }

        public static string GetContent(int ID)
        {
            if (ID == 0) return "文本缺失";
            if (!isInit) Init();
            Dictionary<SystemLanguage, string> content;
            if (!language.TryGetValue(ID, out content)) return "文本缺失 ID:" + ID;
            string result = null;
            if (!content.TryGetValue(currentLanguage, out result)) return "缺少语言:"+currentLanguage;
            return result;
        }

        public static void ChangeLanguage(SystemLanguage lang)
        {
            m_currLang = lang;
            LocalFileMgr.SetInt(localKey, (int) lang);
            onLanguageChanged?.Invoke(lang);
        }
    }
}