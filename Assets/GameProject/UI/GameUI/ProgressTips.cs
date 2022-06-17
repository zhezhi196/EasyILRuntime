﻿using System;
using Module;
using SecondChapter;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class ProgressTips : MonoBehaviour
{
    public Text progressDistance;
    public Image zhishi;
    public Image huangdian;

    // private bool _showTipsButton = true;
    // private bool _showTips = true;
    public RectTransform bg;

    //[ShowInInspector]
    public float safeWidth
    {
        get
        {
            if (simulation) return RootCanvas.Instance.SafeWidth - 200;
            return RootCanvas.Instance.SafeWidth;
        }
    }

    //[ShowInInspector]
    public float width
    {
        get { return RootCanvas.Instance.Width; }
    }

    public bool simulation;
    public ScreenOrientation simulationOrientation;

    public bool showTipsButton(ProgressCtrl ctrl)
    {
        return /*_showTipsButton &&*/ !ctrl.IsAllComplete() && !IsShowTips(ctrl);
    }

    public bool IsShowTips(ProgressCtrl ctrl)
    {
        return /*_showTips && */ctrl.showingTips;
    }

    public bool Refresh()
    {
        ProgressCtrl ctrl = BattleController.GetCtrl<ProgressCtrl>();
        int index = transform.GetSiblingIndex();
        bool shouldShow = ctrl.GetUITipsShouldShow(index);

        if (ctrl.showingTips && shouldShow) //当前有提示
        {
            Vector3 progressPos = ctrl.GetTipPos(index); //通过child的index获取到当前应该在的位置
            float distance = progressPos.Distance(Player.player.transform.position);
            float jiajiao = Vector3.Dot(progressPos - Player.player.transform.position, Player.player.transform.forward);
            bool resultShowTips = distance > LookPoint.circleDistance && IsShowTips(ctrl);
            // if (callback != null)
            // {
            //     callback(false);
            // }

            gameObject.OnActive(resultShowTips);

            if (resultShowTips)
            {
                Vector3 uiPoint = UIController.Instance.Convert3DToUI(Player.player.evCamera, progressPos);

                float halfWidth = bg.rect.width * 0.5f - 15;
                float halfHeight = bg.rect.height * 0.5f - 15;
                // bool isInLiuhai = false;
                // if (simulation)
                // {
                //     isInLiuhai = (simulationOrientation == ScreenOrientation.LandscapeLeft && ((uiPoint.x <= -halfWidth&& jiajiao>=-0.02)||(uiPoint.x >= 0&& jiajiao<0.02))) ||
                //                  (simulationOrientation == ScreenOrientation.LandscapeRight && ((uiPoint.x >= halfWidth&& jiajiao>=-0.02)||(uiPoint.x <= 0&& jiajiao<0.02)));
                // }
                // else
                // {
                //     isInLiuhai = (simulationOrientation == ScreenOrientation.LandscapeLeft && ((uiPoint.x <= -halfWidth&& jiajiao>=-0.02)||(uiPoint.x >= 0&& jiajiao<0.02))) ||
                //                  (simulationOrientation == ScreenOrientation.LandscapeRight && ((uiPoint.x >= halfWidth&& jiajiao>=-0.02)||(uiPoint.x <= 0&& jiajiao<0.02)));
                // }

                bool outScreen = uiPoint.x <= -halfWidth || uiPoint.x >= halfWidth || uiPoint.y >= halfHeight ||
                                 uiPoint.y <= -halfHeight || jiajiao <= 0;
                float radius = bg.rect.width * 0.5f - 50 - liuhaiOffset;
                Vector3 target = progressPos - Player.player.transform.position;
                target = Player.player.transform.InverseTransformDirection(target);
                if (outScreen)
                {
                    transform.localPosition = new Vector2(target.x, 0).normalized * radius; //Vector3.SmoothDamp(transform.localPosition, uiPoint.normalized * radius, ref tempV, 0.05f);
                }
                else
                {
                    transform.localPosition = uiPoint;
                    progressDistance.text = progressPos.Distance(Player.player.transform.position).ToString("F0") + "M";
                }

                zhishi.transform.right = -transform.localPosition;

                if (outScreen)
                {
                    transform.localPosition = transform.localPosition + new Vector3(0, bg.rect.height * 0.2f, 0);
                }

                progressDistance.gameObject.OnActive(!outScreen);
                zhishi.gameObject.OnActive(outScreen);
                
            }
        }
        else
        {
            gameObject.OnActive(false);
            // callback?.Invoke(showTipsButton(ctrl));
        }

        return shouldShow;
    }

    public float liuhaiOffset
    {
        get { return (width - safeWidth) * 0.5f; }
    }

    private Vector3 tempV;

    private void Start()
    {
        //EventCenter.Register<uint, string, IEventCallback>(ConstKey.EventKey, OnGetEvent);
        gameObject.OnActive(false);
    }
}