/*
 * 脚本名称：UIViewBase
 * 项目名称：FrameWork
 * 脚本作者：黄哲智
 * 创建时间：2017-12-22 11:01:18
 * 脚本作用：
*/

using DG.Tweening;
using System;
using System.Collections;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace Module
{
    public abstract class UIViewBase : MonoBehaviour
    {
        protected DOTweenAnimation[] m_partTweens;
        [LabelText("数据层")] public UIModel model;
        [LabelText("名称")] public string winName;
        [LabelText("延迟打开")] public float delay = 0;
        [LabelText("动画时长")] public float tweenInterval = 0.4f;
        public UIObject uiConfig;

        [LabelText("动画曲线")]
        public AnimationCurve tweenCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));

        [LabelText("物理返回键")] public bool canPhysicExit = true;
        [LabelText("UI部件动画时间")] public float partAnimationTime = 0.2f;
        public event Action OnWinInitComplete;
        [LabelText("关闭时销毁")] public bool destroyViewOnClose;

        /// <summary>
        /// 只在生成的时候运行一次，相当于start函数
        /// </summary>
        protected virtual void OnChildStart()
        {
        }
        /// <summary>
        /// 当运行open函数的时候，会执行一次这个函数
        /// </summary>
        public virtual void OnOpenStart()
        {
        }
        /// <summary>
        /// 当当前界面开始显示的时候，会执行这个函数
        /// </summary>
        public virtual void OnShowStart()
        {
        }
        /// <summary>
        /// 当打开动画结束后，会执行这个函数
        /// </summary>
        public virtual void OnOpenComplete()
        {
        }
        /// <summary>
        /// 当关闭开始时会执行这个函数
        /// </summary>
        public virtual void OnCloseStart()
        {
        }
        /// <summary>
        /// 当关闭结束时会执行这个函数
        /// </summary>
        public virtual void OnCloseComplete()
        {
        }
        /// <summary>
        /// 每一次这个页面置到最上方，会 运行这个函数，open函数传的参数，会传到这个args里面
        /// </summary>
        /// <param name="args"></param>
        public virtual void Refresh(params object[] args)
        {
        }
        /// <summary>
        /// 当按物理返回键或者返回键，执行这个函数
        /// </summary>
        /// <param name="args"></param>
        public virtual void OnExit(params object[] args)
        {
            if (UIController.Instance.winList.Count < 2) return;
            UIObject uiObject = UIController.Instance.winList[UIController.Instance.winList.Count - 2];

            if (model.isPopup)
            {
                uiObject.viewBase.model.direction = UIOpenDirection.Backward;
                UIController.Instance.Close(winName, model.tweenType);
            }
            else
            {
                UIController.Instance.Open(uiObject.winName, UIModel.Invert(model.tweenType), OpenFlag.Insertion, args.IsNullOrEmpty() ? uiObject.viewBase.model.args : args);
            }

        }

        public void Init(string name)
        {
            this.winName = name;
            model = new UIModel();
            
            OnChildStart();
            OnWinInitComplete?.Invoke();
        }

        public virtual void EnterAnimator(Action finish)
        {
            if (m_partTweens.IsNullOrEmpty())
            {
                m_partTweens = transform.GetComponentsInChildren<DOTweenAnimation>();
            }

            for (int i = 0; i < m_partTweens.Length; i++)
            {
                if (m_partTweens[i].loops != -1)
                {
                    m_partTweens[i].DORestart();
                }
            }
            finish.Invoke();
        }

        public virtual void ExitAnimator(Action finish)
        {
            if (m_partTweens.IsNullOrEmpty())
            {
                m_partTweens = transform.GetComponentsInChildren<DOTweenAnimation>(false);
            }

            for (int i = 0; i < m_partTweens.Length; i++)
            {
                if (m_partTweens[i].loops != -1)
                {
                    m_partTweens[i].DOPlayBackwards();
                }
            }
            finish.Invoke();
        }

        protected virtual void Update()
        {
            if (model != null)
            {
                model.UpdateModel();
            }
        }
    }
}
