using System.Collections.Generic;
using System.IO;
using LitJson;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace Module
{
    [ExecuteInEditMode]
    public class Label : MonoBehaviour
    {
        public int Key = -1;
        public Text text;
        public void SetKey(int key)
        {
            this.Key = key;
            OnLanguageChanged(Language.currentLanguage);
        }
        
        void Awake()
        {
            text = GetComponent<Text>();
            Language.onLanguageChanged += OnLanguageChanged;
            OnLanguageChanged(Language.currentLanguage);
        }

        private void OnLanguageChanged(SystemLanguage lang)
        {
            if (text != null)
            {
                text.text = Language.GetContent(Key);
            }
        }
        private void OnDestroy()
        {
            Language.onLanguageChanged -= OnLanguageChanged;
        }

#if UNITY_EDITOR
        public static Dictionary<int, LangInfo> language = new Dictionary<int, LangInfo>();
        [Button("刷新语言配置")]
        public static void Init()
        {
            using (StreamReader reader=new StreamReader($"{Application.dataPath}/Bundles/{string.Format(ConstKey.JsonConfigPath,"LanguageInfo.json")}"))
            {
                LangInfo[] target = JsonMapper.ToObject<LangInfo[]>(reader.ReadToEnd());
                language.Clear();
                for (int i = 0; i < target.Length; i++)
                {
                    LangInfo temp = target[i];
                    language.Add(temp.ID, temp);
                }
            }

        }
        void Update()
        {
            if (Application.isPlaying) return;

            if (!language.ContainsKey(Key))
            {
                text.text = "文本丢失了";
                return;
            }

            switch (Language.currentLanguage)
            {
                case SystemLanguage.Chinese:
                    text.text = language[Key].CN;
                    break;
                case SystemLanguage.English:
                    text.text = language[Key].EN;
                    break;
            }
        }
#endif
    }



}
