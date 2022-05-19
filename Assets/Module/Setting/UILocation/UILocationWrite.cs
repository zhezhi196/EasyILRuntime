using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Module
{
    public class UILocationWrite : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IInitializePotentialDragHandler
    {
        public UILocationFlag flag = UILocationFlag.Alpha | UILocationFlag.Position | UILocationFlag.Scale;
        public string uiName;
        public string itemName;
        private Vector2 offset;
        public Rect uiRec;
        public CanvasGroup canvasGroup;

        public event Action<UILocationWrite> onBeginDrag; 
        public event Action<UILocationWrite> onDrag; 
        public event Action<UILocationWrite> onEndDrag;

        public UILocationWrite[] bindingTar;
        public UILocation resetValue;
        public bool isChanged;
        //
        // public UILocation defaultValue
        // {
        //     get
        //     {
        //         string temp = LocalFileMgr.GetString($"{uiName}_{gameObject.name}_default");
        //         if (!temp.IsNullOrEmpty())
        //         {
        //             return new UILocation(uiName, gameObject.name, temp);
        //         }
        //         else
        //         {
        //             return resetValue;
        //         }
        //     }
        //     set
        //     {
        //         string key = $"{uiName}_{gameObject.name}_default";
        //         if (!LocalFileMgr.ContainKey(key))
        //         {
        //             LocalFileMgr.SetString(key, value.ToString());
        //         }
        //     }
        // }

        private void Awake()
        {
            ILocationSettingUI ui = transform.GetComponentInParent<ILocationSettingUI>();
            uiName = ui.uiName;
            itemName = gameObject.name;
            uiRec = ui.uiRect;
            if (canvasGroup == null)
            {
                canvasGroup = GetComponent<CanvasGroup>();
            }
            resetValue = new UILocation(ui.uiName, gameObject.name, transform.localPosition, transform.localScale.x, canvasGroup.alpha);
            //defaultValue = resetValue;
        }

        private void Start()
        {
            Setting.uiLocationSetting.SetLocation(uiName, itemName, transform.localPosition, transform.localScale.x, canvasGroup != null ? canvasGroup.alpha : 1);
        }

        private void OnEnable()
        {
            ResetValue();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
        }

        public void OnDrag(PointerEventData eventData)
        {
            OnDraggItem(eventData.position);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            OnEndDraggItem(eventData.position);
        }
        
        public virtual void OnBeginDragItem(Vector2 eventData)
        {
            MoveBegin(eventData);
            onBeginDrag?.Invoke(this);
            if (!bindingTar.IsNullOrEmpty())
            {
                for (int i = 0; i < bindingTar.Length; i++)
                {
                    bindingTar[i].MoveBegin(eventData);
                }
            }
        }

        private Vector2 Min(Vector2 v1, Vector2 v2)
        {
            float x = 0;
            if (v1.x > 0 && v2.x > 0) x = Mathf.Min(v1.x, v2.x);
            if (v1.x <= 0 && v2.x <= 0) x = Mathf.Max(v1.x, v2.x);

            float y = 0;
            if (v1.y > 0 && v2.y > 0) y = Mathf.Min(v1.y, v2.y);
            if (v1.y <= 0&& v2.y<=0) y = Mathf.Max(v1.y, v2.y);
            return new Vector2(x, y);
        }

        public virtual void OnDraggItem(Vector2 eventData)
        {
            Vector2 shouldMove = GetMoveDeta(eventData);
            if (!bindingTar.IsNullOrEmpty())
            {
                for (int i = 0; i < bindingTar.Length; i++)
                {
                    Vector2 bindDate = bindingTar[i].GetMoveDeta(eventData);
                    shouldMove = Min(shouldMove, bindDate);
                }
            }

            Move(shouldMove);
            if (!bindingTar.IsNullOrEmpty())
            {
                for (int i = 0; i < bindingTar.Length; i++)
                {
                    bindingTar[i].Move(shouldMove);
                }
            }
            onDrag?.Invoke(this);
        }



        public virtual void OnEndDraggItem(Vector2 eventData)
        {
            MoveEnd(eventData);
            onEndDrag?.Invoke(this);
            if (!bindingTar.IsNullOrEmpty())
            {
                for (int i = 0; i < bindingTar.Length; i++)
                {
                    bindingTar[i].MoveEnd(eventData);
                }
            }
        }
        
        private void MoveBegin(Vector2 eventData)
        {
            offset = transform.localPosition.ToVector2() - eventData;
        }

        public Vector2 GetMoveDeta(Vector2 eventData)
        {
            Vector2 tar = eventData + offset;
            Vector2 min = new Vector2(-uiRec.width * 0.5f, -uiRec.height * 0.5f);
            Vector2 max = -min;
            tar = tar.ClampVector(min, max);
            return tar - new Vector2(transform.localPosition.x, transform.localPosition.y);
        }

        private void Move(Vector3 eventData)
        {
            isChanged = true;
            transform.localPosition += eventData;
            // isChanged = true;
            // Vector3 tar = eventData + offset;
            // Vector3 min = Vector3.zero;
            // Vector3 max = new Vector3(Screen.width, Screen.height);
            // transform.position = tar.Clamp(min, max);
            // return tar.x <= max.x && tar.x >= min.x && tar.y <= max.y && tar.y >= min.y;
        }

        private void MoveEnd(Vector2 eventData)
        {
        }
        public void Save()
        {
            Setting.uiLocationSetting.SetLocation(uiName, itemName, transform.localPosition, transform.localScale.x, canvasGroup != null ? canvasGroup.alpha : 1);
            isChanged = false;
        }

        public void ResetDefault()
        {
            resetValue.Delete();
            Setting.uiLocationSetting.DeleteLocation(uiName, itemName);
            // var tar = defaultValue;
            //
            // if ((flag & UILocationFlag.Position) != 0)
            // {
            //     transform.position = tar.position;
            // }
            //
            // if ((flag & UILocationFlag.Scale) != 0)
            // {
            //     transform.localScale = tar.scale * Vector2.one;
            // }
            //
            // if ((flag & UILocationFlag.Alpha) != 0)
            // {
            //     if (canvasGroup != null)
            //     {
            //         canvasGroup.alpha = tar.alpha;
            //     }
            // }
            //
            // isChanged = true;
        }

        public void ResetValue()
        {
            var tar = Setting.uiLocationSetting.GetPositon(uiName, itemName);
            if (!tar.hasKey)
            {
                tar = resetValue;
            }
            
            if ((flag & UILocationFlag.Position) != 0)
            {
                transform.localPosition = new Vector3(tar.position.x, tar.position.y, transform.localPosition.z);
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

            isChanged = false;
        }

        public void OnInitializePotentialDrag(PointerEventData eventData)
        {
            OnBeginDragItem(eventData.position);
        }
        
        
        [Button]
        private void EditorInit()
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }
    }
}