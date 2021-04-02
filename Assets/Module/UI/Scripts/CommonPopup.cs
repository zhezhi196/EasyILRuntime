using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Module
{
    public struct PupupOption
    {
        public string title;
        public UnityAction action;

        public PupupOption(UnityAction action,string title)
        {
            this.action = action;
            this.title = title;
        }
    }

    public class CommonPopup : MonoBehaviour
    {
        [Serializable]
        public struct PopupCollect
        {
            public Button button;
            public Text content;
        }

        public PopupCollect[] buttons;
        public Text title;
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

        public static void Popup(string title, string content, params PupupOption[] option)
        {
            if (option.Length < 1)
            {
                option = new PupupOption[1];
                option[0] = new PupupOption(() => Close(), "Confirm");
            }
            Instance.gameObject.OnActive(true);
            Instance.title.text = title;
            Instance.content.text = content;
            for (int i = 0; i < Instance.buttons.Length; i++)
            {
                if (i < option.Length)
                {
                    Instance.buttons[i].button.gameObject.OnActive(true);

                    Instance.buttons[i].button.onClick.AddListener(option[i].action);
                    Instance.buttons[i].content.text = option[i].title;
                }
                else
                {
                    Instance.buttons[i].button.gameObject.OnActive(false);
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
        }
    }
}