using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Module
{
    public struct PupupOption
    {
        public string title;
        public UnityAction action;
        public Sprite subIcon;

        public PupupOption(UnityAction action, string title, Sprite subIcon = null)
        {
            this.action = action;
            this.title = title;
            this.subIcon = subIcon;
        }
    }

    public class CommonPopup : MonoBehaviour
    {
        [Serializable]
        public struct PopupCollect
        {
            public Button button;
            public Text content;
            public Image subIcon;
        }

        public PopupCollect[] buttons;
        public Text title;
        public Image icon;
        public Text content;


        #region instance

        private static CommonPopup instance;

        private static CommonPopup Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = UICommpont.UICanvas.transform.Find("CommonPopup").GetComponent<CommonPopup>();
                }

                return instance;
            }
        }

        #endregion

        public static bool isPopup;
        public static void Popup(string title, string content, Sprite icon, params PupupOption[] option)
        {
            if (option.Length < 1)
            {
                option = new PupupOption[1];
                option[0] = new PupupOption(() => Close(), Language.GetContent(407));
            }

            Instance.gameObject.OnActive(true);
            Instance.title.text = title;
            Instance.content.text = content;
            for (int i = 0; i < Instance.buttons.Length; i++)
            {
                if (i < option.Length)
                {
                    ;
                    Instance.buttons[i].button.gameObject.OnActive(true);
                    Instance.buttons[i].button.onClick.AddListener(option[i].action);
                    Instance.buttons[i].button.onClick.AddListener(()=>AudioPlay.PlayOneShot("tongyongButton").SetIgnorePause(true));
                    Instance.buttons[i].content.text = option[i].title;
                    Instance.buttons[i].subIcon.gameObject.OnActive(option[i].subIcon != null);
                    Instance.buttons[i].subIcon.sprite = option[i].subIcon;
                }
                else
                {
                    Instance.buttons[i].button.gameObject.OnActive(false);
                }
            }

            Instance.icon.gameObject.OnActive(icon != null);
            Instance.icon.sprite = icon;
            if (icon != null)
            {
                Instance.content.alignment = TextAnchor.UpperCenter;
            }
            else
            {
                Instance.content.alignment = TextAnchor.MiddleCenter;
            }
            
            Instance.Sort();
            isPopup = true;
        }

        private void Sort()
        {
            StartCoroutine(SortEn());
        }

        private IEnumerator SortEn()
        {
            yield return new WaitForSecondsRealtime(0.2f);
            for (int i = 0; i < buttons.Length; i++)
            {
                LayoutGroup lay = buttons[i].button.GetComponent<LayoutGroup>();
                if (lay != null)
                {
                    lay.enabled = true;
                }
            }
            
            yield return new WaitForSecondsRealtime(0.2f);
            for (int i = 0; i < buttons.Length; i++)
            {
                LayoutGroup lay = buttons[i].button.GetComponent<LayoutGroup>();
                if (lay != null)
                {
                    lay.enabled = false;
                }
            }
        }

        public static void Close()
        {
            Instance.gameObject.OnActive(false);
            for (int i = 0; i < Instance.buttons.Length; i++)
            {
                Instance.buttons[i].button.onClick.RemoveAllListeners();
            }
            isPopup = false;
        }
    }
}