using System;
using System.Collections;
using System.Collections.Generic;
using Module;
using UnityEngine;

namespace HotFix
{
    public interface ICtrlUi
    {
        bool isActive { get; }
        UIModul RefreshModul(UITweenType tweenType, bool isPopup,UIObject from, object[] args);
        void OnStart();
        void Refresh();
        void OnDestroy();
        void Back(UIObject lastUI);
        void OnOpenStart();
        void OnOpenComplete();
        void OnCloseStart();
        void OnCloseComplete();
        V GetOtherView<V>() where V : UIView;

        UIView CreatView(GameObject prefab,bool isPopup);
        void ExitPartAnimator(UITweenType tweenType,Action runNext);
        void EnterPartAnimator(UITweenType tweenType,Action runNext);
    }

    public class UICtrl<ModuleType, ViewType> : ICtrlUi where ViewType : UIView where ModuleType : UIModul, new()
    {
        public ViewType view;
        public ModuleType modul;

        public bool isActive
        {
            get { return view != null; }
        }

        public UIModul RefreshModul(UITweenType tweenType, bool isPopup,UIObject from, object[] args)
        {
            if (modul == null)
            {
                modul = new ModuleType();
            }

            modul.OnRefresh(tweenType, isPopup, from, args);
            return modul;
        }

        public UIView CreatView(GameObject prefab, bool isPopup)
        {
            if (!isActive)
            {
                // GameObject go = HotFixGameObject.Instantiate(prefab, isPopup ? Module.UIComponent.PopupTransform : Module.UIComponent.NormalTransform, references =>
                // {
                //     for (int i = 0; i < references.Length; i++)
                //     {
                //         HotFixMonoBehaviour.CreateMonoBehaviour(references[i]);
                //     }
                // });
                // view = (ViewType) go.GetComponent<ViewReference>().target;
                // view.gameObject.SetActive(false);
                // OnStart();

                GameObject go = ResourceManager.Instantiate(prefab, isPopup ? Module.UIComponent.PopupTransform : Module.UIComponent.NormalTransform);
                view = (ViewType) go.GetScript<UIView>();
                OnStart();
            }

            return view;
        }
        public V GetOtherView<V>() where V : UIView
        {
            return null;
        }
        
        #region EventMethod
        public virtual void OnStart()
        {
        }

        public virtual void Refresh()
        {
        }

        public virtual void OnDestroy()
        {
        }

        public virtual void Back(UIObject lastUI)
        {
            if (UIObject.currentUI.modul.isPopup)
            {
                UIObject.currentUI.Close();
            }
            else
            {
                UIObject.Open(lastUI.uiType, lastUI.modul.tweenType, lastUI.modul.args);
            }
        }

        public virtual void OnOpenStart()
        {
        }

        public virtual void OnOpenComplete()
        {
        }

        public virtual void OnCloseStart()
        {
        }

        public virtual void OnCloseComplete()
        {
        }
        
        public virtual void ExitPartAnimator(UITweenType tweenType,Action runNext)
        {
            runNext?.Invoke();
        }

        public virtual void EnterPartAnimator(UITweenType tweenType,Action runNext)
        {
            runNext?.Invoke();
        }
        
        #endregion
        
        
    }
}