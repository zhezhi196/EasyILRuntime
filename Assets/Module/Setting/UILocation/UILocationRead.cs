using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Module
{
    public class UILocationRead : MonoBehaviour
    {
        public UILocationFlag flag = UILocationFlag.Alpha | UILocationFlag.Position | UILocationFlag.Scale;

        public string uiName;
        public string itemName;
        public CanvasGroup canvasGroup;

        private void Awake()
        {
            uiName = transform.GetComponentInParent<ILocaltionReadingUI>()?.uiName;
            itemName = gameObject.name;
            canvasGroup = transform.GetComponent<CanvasGroup>();
        }

        public void ResetValue()
        {
            var tar = Setting.uiLocationSetting.GetPositon(uiName, itemName);
            if (tar.hasKey)
            {
                if ((flag & UILocationFlag.Position) != 0)
                {
                    transform.localPosition = tar.position;
                }

                if ((flag & UILocationFlag.Scale) != 0)
                {
                    transform.localScale = tar.scale * Vector2.one;
                }

                if ((flag & UILocationFlag.Alpha) != 0)
                {
                    if (canvasGroup != null)
                    {
                        canvasGroup.alpha = tar.alpha;
                    }
                }
            }
        }

        private void OnEnable()
        {
            ResetValue();
        }
        
        [Button]
        private void EditorInit()
        {
            canvasGroup = gameObject.AddOrGetComponent<CanvasGroup>();
        }
        
    }
}