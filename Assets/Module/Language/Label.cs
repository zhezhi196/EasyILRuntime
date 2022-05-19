using System.Collections.Generic;
using System.IO;
using LitJson;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Module
{
    public enum LabelFlag
    {
        None,
        [LabelText("所有字母大写")]
        AllUper,
        [LabelText("所有字母小写")]
        AllLower,
    }
    [ExecuteInEditMode]
    [RequireComponent(typeof(Text))]
    public class Label : FontSetting
    {
        [OnValueChanged("OnKeyChanged")]
        public string Key;
        public LabelFlag flag;

        public void SetKey(string key, LabelFlag flag = LabelFlag.None)
        {
            this.Key = key;
            this.flag = flag;
            OnLanguageChanged(Language.currentLanguage);
        }
        
        protected override void OnLanguageChanged(SystemLanguage lang)
        {
            base.OnLanguageChanged(lang);
            if (text != null)
            {
                text.text = Language.GetContent(Key, flag);
            }
        }

#if UNITY_EDITOR
        private void OnKeyChanged()
        {
            if (Application.isPlaying) return;
            Language.InitAction(null);
            if (Language.info != null)
            {
                text.text = Language.GetContent(Key);
            }
        }
        [Button]
        private void Refresh()
        {
            Language.info = null;
            OnLanguageChanged(Language.currentLanguage);
        }
#endif
    }
}
