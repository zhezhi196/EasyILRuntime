using SDK;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Module
{
    public class PhoneInputField : InputField
    {
        public override void OnPointerDown(PointerEventData eventData)
        {
#if UNITY_EDITOR
            base.OnPointerDown(eventData);
#else
            SDKMgr.GetInstance().MyCommon.OpenInput(textComponent.text, OnInputValueChanged, OnKeybordShowChange);
#endif
        }

        private void OnKeybordShowChange(bool obj)
        {
            if (!obj)
            {
                DeactivateInputField();
            }
        }

        private void OnInputValueChanged(string obj)
        {
            this.textComponent.text = obj;
        }
    }
}