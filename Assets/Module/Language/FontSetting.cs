using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Module
{
    [Serializable]
    public class FontInfo
    {
        [HorizontalGroup,HideLabel]
        public SystemLanguage lan;
        [HorizontalGroup,HideLabel]
        public UnityEngine.Font font;
    }

    [RequireComponent(typeof(Text))]
    [ExecuteInEditMode]
    public class FontSetting : MonoBehaviour
    {
#if UNITY_EDITOR

        [UnityEditor.MenuItem("Tools/AddFontSetting")]
        public static void AddFontSetting()
        {
            Object[] select = UnityEditor.Selection.objects;
            
            for (int i = 0; i < select.Length; i++)
            {
                GameObject go = select[i] as GameObject;
                if (go != null)
                {
                    Set1(go);
                }
            }
        }
#endif
        private static void Set1(GameObject go)
        {
#if UNITY_EDITOR
            Text[] text = go.transform.GetComponentsInChildren<Text>();
            for (int i = 0; i < text.Length; i++)
            {
                FontSetting setting = text[i].gameObject.AddOrGetComponent<FontSetting>();
                setting.text = text[i];
                setting.currLan = Language.currentLanguage;
            }
#endif
        }

        public Text text;

        public SystemLanguage currLan;

        private void Awake()
        {
            if (text == null)
            {
                text = GetComponent<Text>();
            }
            
            Set1(gameObject);
        }

        private void Start()
        {
            OnLanguageChanged(Language.currentLanguage);
        }

        private void OnEnable()
        {
            Language.onLanguageChanged += OnLanguageChanged;

            if (currLan != Language.currentLanguage)
            {
                OnLanguageChanged(Language.currentLanguage);
            }
        }

        private void OnDisable()
        {
            Language.onLanguageChanged -= OnLanguageChanged;
        }

        private void OnLanguageChanged(SystemLanguage obj)
        {
            if (Font.Instance == null) return;
            for (int i = 0; i < Font.Instance.info.Length; i++)
            {
                var tar = Font.Instance.info[i];
                if (tar.lan == obj)
                {
                    if (tar.font != null)
                    {
                        text.font = tar.font;
                    }
                    else
                    {
                        text.font = Font.arial;
                    }
                    break;
                }
            }

            currLan = obj;
        }
    }
}