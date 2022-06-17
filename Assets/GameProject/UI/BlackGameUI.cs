using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Module;
using System;

namespace ProjectUI
{
    public class BlackGameUI : UIViewBase
    {
        [Header("移动控制")]
        public GameObject leftStick;//移动
        public UIScrollBase leftJoyStick;
        public RectTransform viewPivot;//摇杆背景
        public RectTransform pot;//摇杆点
        public UIScrollBase rightJoyStick;
        private float radius;//点移动半径
        public UIBtnBase setBtn;
        private List<UIInteractionNode> interNodes = new List<UIInteractionNode>();
        public Transform handNode;
        protected override void OnChildStart()
        {
            //-----移动旋转-----
            leftJoyStick.AddDragDown(StartMove);
            leftJoyStick.AddDrag(RoleMove);
            leftJoyStick.AddDragUp(StopMove);
            radius = pot.sizeDelta.x * 0.5f;//计算点移动半径
            rightJoyStick.AddDragDown(OnStartRotate);
            rightJoyStick.AddDrag(OnDragUI);
            rightJoyStick.AddDragUp(OnStopRotate);
            setBtn.AddListener(OnSetBtn);
            PropsCtrl.RegisterLookPointEvent(ShowHand);
        }

        private void OnDestroy()
        {
            PropsCtrl.UnRegisterLookPointEvent(ShowHand);
        }

        private void LateUpdate()
        {
            for (int i = 0; i < interNodes.Count; i++)
            {
                interNodes[i].UpdateNode(handNode);
            }
        }

        public void StartMove(Vector2 arg1, Vector2 arg2)
        {
            Player.player?.StartMove();
        }

        public void RoleMove(Vector2 arg1, Vector2 arg2, float arg3)
        {
            if (pot.anchoredPosition.magnitude > radius)
                pot.anchoredPosition = pot.anchoredPosition.normalized * radius;
            {//移动输入刷新
                Vector2 dir = pot.anchoredPosition / radius;
                if (dir.sqrMagnitude <= 0.001f)
                {
                    dir = Vector2.zero;
                }
                if (Mathf.Abs(dir.x) > 0.5f)
                {
                    dir.x = dir.x > 0 ? 1f : -1f;
                }
                else
                {
                    dir.x = dir.x * 2f;
                }
                if (Mathf.Abs(dir.y) > 0.5f)
                {
                    dir.y = dir.y > 0 ? 1f : -1f;
                }
                else
                {
                    dir.y = dir.y * 2f;
                }
                Player.player?.Movement(dir);
            }
        }

        public void StopMove(Vector2 arg1, Vector2 arg2)
        {
            Player.player?.StopMove();
           
        }

        private void OnStartRotate(Vector2 arg1, Vector2 arg2)
        {
            Player.player?.StartRotate();
        }

        private void OnDragUI(Vector2 arg1, Vector2 arg2, float arg3)
        {
            Player.player?.Rotate(arg2);
        }

        private void OnStopRotate(Vector2 arg1, Vector2 arg2)
        {
            Player.player?.StopRotate();
        }
        private void OnSetBtn()
        {
            UIController.Instance.Open("BattleSettingUI", UITweenType.None, 2);
        }

        private const string UIInteractionNodeStr = "UIInteractionNode";
        private void ShowHand(bool show, LookPoint lookPoint, Action callback)
        {
            if (show)
            {
                LoadPrefab<UIInteractionNode>(UIInteractionNodeStr, null, node =>
                {
                    node.transform.SetParent(handNode);
                    node.transform.localScale = Vector3.one;
                    node.lookPoint = lookPoint;
                    lookPoint.uiNode = node;
                    interNodes.Add(node);
                    callback?.Invoke();
                });
            }
            else
            {
                interNodes.Remove(lookPoint.uiNode);
            }
        }
    }
}
