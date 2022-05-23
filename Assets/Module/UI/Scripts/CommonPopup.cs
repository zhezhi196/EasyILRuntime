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
            public Text noIconContent;
            public Text content;
            public Image subIcon;
        }

        public PopupCollect[] buttons;
        public GameObject titleUnder;
        public Text title;
        public Image icon;
        public Text content;
        //public Button selectPanle;



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
        public static Action<bool> onPopup;
        
        public static void RefreshContent(string s)
        {
            Instance.content.text = s;
        }

        public static void Popup(string title, string content, Sprite icon, params PupupOption[] option)
        {
            if (option.Length < 1)
            {
                option = new PupupOption[1];
                option[0] = new PupupOption(null, Language.GetContent("702"));
            }

            var canvas = Instance.gameObject.GetComponent<Canvas>();
            if (canvas != null)
            {
                AssetLoad.Destroy(canvas);
            }
            
            var ray = Instance.gameObject.GetComponent<GraphicRaycaster>();
            if (ray != null)
            {
                AssetLoad.Destroy(ray);
            }
            
            onPopup?.Invoke(true);
            Instance.gameObject.OnActive(true);
            Instance.title.text = title;
            Instance.titleUnder.OnActive(!string.IsNullOrEmpty(title));
            Instance.content.text = content;
            //Instance.selectPanle.gameObject.OnActive(false);
            for (int i = 0; i < Instance.buttons.Length; i++)
            {
                if (i < option.Length)
                {
                    Instance.buttons[i].button.gameObject.OnActive(true);
                    var callback = option[i].action;
                    Instance.buttons[i].button.onClick.AddListener(() =>
                    {
                        Close();
                        callback?.Invoke();
                    });
                    //Instance.buttons[i].button.onClick.AddListener(()=>AudioPlay.PlayOneShot(Config.globleConfig.commonButtonAudio).SetIgnorePause(true));
                    if (option[i].subIcon != null)
                    {
                        Instance.buttons[i].content.transform.parent.gameObject.OnActive(true);
                        Instance.buttons[i].noIconContent.gameObject.OnActive(false);
                        Instance.buttons[i].content.text = option[i].title;
                    }
                    else
                    {
                        Instance.buttons[i].content.transform.parent.gameObject.OnActive(false);
                        Instance.buttons[i].noIconContent.gameObject.OnActive(true);
                        Instance.buttons[i].noIconContent.text = option[i].title;
                    }
                }
                else
                {
                    Instance.buttons[i].button.gameObject.OnActive(false);
                }
            }

            Instance.icon.gameObject.OnActive(icon != null);
            Instance.icon.sprite = icon;
            Instance.icon.SetNativeSize();
            if (icon != null)
            {
                Instance.content.alignment = TextAnchor.UpperCenter;
            }
            else
            {
                Instance.content.alignment = TextAnchor.MiddleCenter;
            }
            
            isPopup = true;
        }

        public static void PopupNoClose(string title, string content, Sprite icon, params PupupOption[] option)
        {
            if (option.Length < 1)
            {
                option = new PupupOption[1];
                option[0] = new PupupOption(null, Language.GetContent("702"));
            }
            onPopup?.Invoke(true);
            Instance.gameObject.OnActive(true);
            Instance.title.text = title;
            Instance.content.text = content;
            //Instance.selectPanle.gameObject.OnActive(false);
            for (int i = 0; i < Instance.buttons.Length; i++)
            {
                if (i < option.Length)
                {
                    Instance.buttons[i].button.gameObject.OnActive(true);
                    var callback = option[i].action;
                    Instance.buttons[i].button.onClick.AddListener(() =>
                    {
                        //Close();
                        callback?.Invoke();
                    });
                    Instance.buttons[i].button.onClick.AddListener(() => AudioManager.PlayUI(Config.globleConfig.commonButtonAudio));//AudioPlay.PlayOneShot(Config.globleConfig.commonButtonAudio).SetIgnorePause(true));
                    if (option[i].subIcon != null)
                    {
                        Instance.buttons[i].content.transform.parent.gameObject.OnActive(true);
                        Instance.buttons[i].noIconContent.gameObject.OnActive(false);
                        Instance.buttons[i].content.text = option[i].title;
                    }
                    else
                    {
                        Instance.buttons[i].content.transform.parent.gameObject.OnActive(false);
                        Instance.buttons[i].noIconContent.gameObject.OnActive(true);
                        Instance.buttons[i].noIconContent.text = option[i].title;
                    }
                }
                else
                {
                    Instance.buttons[i].button.gameObject.OnActive(false);
                }
            }

            Instance.icon.gameObject.OnActive(icon != null);
            Instance.icon.sprite = icon;
            Instance.icon.SetNativeSize();
            if (icon != null)
            {
                Instance.content.alignment = TextAnchor.UpperCenter;
            }
            else
            {
                Instance.content.alignment = TextAnchor.MiddleCenter;
            }

            isPopup = true;
        }

        public static void AddContentAction(UnityAction cal)
        {
            //Instance.selectPanle.gameObject.OnActive(true);
            //Instance.selectPanle.onClick.RemoveAllListeners();
            //Instance.selectPanle.onClick.AddListener(cal);
        }

        public static void Close()
        {
            onPopup?.Invoke(false);
            Instance.gameObject.OnActive(false);
            for (int i = 0; i < Instance.buttons.Length; i++)
            {
                Instance.buttons[i].button.onClick.RemoveAllListeners();
            }
            isPopup = false;
        }

        public static void UpdateContent(string c)
        {
            if (instance.gameObject.activeInHierarchy)
            {
                Instance.content.text = c;
            }
        }
    }
}