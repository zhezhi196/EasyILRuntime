using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Module
{
    [RequireComponent(typeof(Text))]
    [ExecuteInEditMode]
    public class FontSetting : MonoBehaviour
    {
        public Text text;
        [ReadOnly]
        public SystemLanguage textLanguage;

        protected virtual async void Awake()
        {
            if (text == null) text = GetComponent<Text>();
            if (Language.info == null)
            {
                await Async.WaitUntil(()=>Language.info!=null);
            }
            OnLanguageChanged(Language.currentLanguage);
        }
        
        protected virtual void OnEnable()
        {
            Language.onLanguageChanged += OnLanguageChanged;
            if (textLanguage != Language.currentLanguage)
            {
                OnLanguageChanged(Language.currentLanguage);
            }
        }

        protected virtual void OnDisable()
        {
            Language.onLanguageChanged -= OnLanguageChanged;
        }

        protected virtual void OnLanguageChanged(SystemLanguage obj)
        {
            if (FontInfo.info == null) return;
            for (int i = 0; i < FontInfo.info.Length; i++)
            {
                var tar = FontInfo.info[i];
                if (tar.lan == obj)
                {
                    if (tar.font != null)
                    {
                        text.font = tar.font;
                    }
                    break;
                }
            }

            textLanguage = obj;
        }
    }
}