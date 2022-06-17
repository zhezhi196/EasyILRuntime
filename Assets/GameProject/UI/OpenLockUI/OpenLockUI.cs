using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Module;
using Project.Data;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

[Serializable]
public enum OpenOperationStage
{
    ArrowMoving,
    ArrowStop,
    Animating,
    LackLockPick, //开锁器不足
}

public class OpenLockUI : UIViewBase
{
    private const string OpenLockArgsPath = "TinyGame/OpenLock/{0}.asset";
    public OpenLockArgs lockParam;

    public Button backBtn;
    public Button openBtn;
    public OpenLockUILight lightComponent;
    public RectTransform greenArea;
    public RectTransform arrow;

    public Transform lockIn;
    public Transform lockPick;
    public Transform lockPickRoot;

    public CanvasGroup root;

    public Button successPage;
    public Text lockPickNumText;

    public Sprite lockPickSprite;
    public Sprite adSprite;
    
    private float processBarWidth;
    private int _curIndex = 0;
    [SerializeField, ReadOnly] 
    private OpenOperationStage _state;
    private Vector2 _arrowPosCache = new Vector2(0,0);
    
    [SerializeField, ReadOnly]
    private float _arrowPosNormalized = 0; //箭头的位置 0-1
    private int _speedDirection = 1; // 1 -1
    private float _speedScale = 0.07f;

    [SerializeField]
    private float _curSpeed = 0;

    private DoorLockPickProp _prop;

    private const int lockPickId = 20031;
    private AdsData adData;

    protected override void OnChildStart()
    {
        backBtn.onClick.AddListener(() =>
        {
            successPage.gameObject.OnActive(false);
            UIController.Instance.Back();
        });
        openBtn.onClick.AddListener(OnClickStop);
        successPage.onClick.AddListener(() =>
        {
            successPage.gameObject.OnActive(false);
            UIController.Instance.Back();
        });
        processBarWidth = ((RectTransform) greenArea.parent).sizeDelta.x;
        _arrowPosCache.y = arrow.anchoredPosition.y;
        // lockPickRoot = lockPick.parent;

        adData = DataMgr.Instance.GetSqlService<AdsData>().WhereID(30701);
        base.OnChildStart();
    }
    

    public override void Refresh(params object[] args)
    {
        root.alpha = 0;
        _prop = (DoorLockPickProp)args[0];
        var lockArgsId = (int)args[1];
        AssetLoad.PreloadAsset<OpenLockArgs>(string.Format(OpenLockArgsPath, lockArgsId), (async) =>
        {

            lockParam = async.Result;
            RefreshPrivate();
            root.DOFade(1, 0.2f).SetUpdate(true);
        });
        
        base.Refresh(args);
    }

    private void RefreshPrivate()
    {
        ResetLock();
        lightComponent.OnRefresh(lockParam.lockNum);
        if (!HaveEnoughLockPick())
        {
            RefreshCurStage(false,null);
            lockPickRoot.gameObject.OnActive(false);
            _state = OpenOperationStage.LackLockPick;
            OpenWatchAd();
        }
        else
        {
            RefreshCurStage(true,null);
            lockPickRoot.gameObject.OnActive(true);
            PlayShakeAnim(true);
            PlayBrokenAnim(true);
            PlayBrokenAnim(false);
        }

        lockPickNumText.text = BattleController.GetCtrl<BagPackCtrl>().GetBagItemNum(lockPickId).ToString();
    }

    private Tween greenAreaAnim;
    private void RefreshCurStage(bool anim , Action refreshOver)
    {
        //重置宽度和箭头位置
        arrow.anchoredPosition = new Vector2(0, arrow.anchoredPosition.y);
        _arrowPosNormalized = 0;
        var pos = lockParam.areas[_curIndex].pos; // [0-1]
        _curSpeed = lockParam.areas[_curIndex].moveSpeed;

        if (greenAreaAnim != null)
        {
            greenAreaAnim.Kill();
        }

        greenArea.sizeDelta = new Vector2(0, greenArea.sizeDelta.y);
        greenArea.anchoredPosition = new Vector3(((pos.y + pos.x)/2) * processBarWidth, greenArea.anchoredPosition.y);
        if (anim)
        {
            greenAreaAnim = greenArea.DOSizeDelta(new Vector2((pos.y - pos.x) * processBarWidth, greenArea.sizeDelta.y), 0.5f).SetUpdate(true).OnComplete(() =>
            {
                _state = OpenOperationStage.ArrowMoving;
                refreshOver?.Invoke();
            });
        }
        else
        {
            greenArea.sizeDelta = new Vector2((pos.y - pos.x) * processBarWidth, greenArea.sizeDelta.y);
            _state = OpenOperationStage.ArrowMoving;
            refreshOver?.Invoke();
        }
    }

    private void Update()
    {
        // openBtn.gameObject.OnActive(false);
        if (_state != OpenOperationStage.ArrowMoving)
        {
            return;
        }
        // openBtn.gameObject.OnActive(true);
        
        //箭头位置计算
        _arrowPosNormalized = Mathf.Clamp01(_arrowPosNormalized + _speedDirection * Time.unscaledDeltaTime * _speedScale * _curSpeed);

        //方向变化
        if (_arrowPosNormalized == 0 || _arrowPosNormalized == 1)
        {
            _speedDirection *= -1;
        }
        
        //箭头位置赋值
        _arrowPosCache.x = processBarWidth * _arrowPosNormalized;
        arrow.anchoredPosition = _arrowPosCache;
    }

    private const string FreezeKey = "OpenLockAnimating";
    private void OnClickStop()
    {
        if (_state != OpenOperationStage.ArrowMoving)
        {
            return;
        }
        
        _state = OpenOperationStage.Animating;
        UICommpont.FreezeUI(FreezeKey);
        
        var curPos = lockParam.areas[_curIndex].pos;
        if (_arrowPosNormalized >= curPos.x && _arrowPosNormalized <= curPos.y)
        {
            OnHit();
        }
        else
        {
            GameDebug.Log("未命中！");
            _state = OpenOperationStage.Animating;
            //播放断裂动画
            PlayShakeAnim(false);
            PlayBrokenAnim(true);
            greenArea.sizeDelta = new Vector2(0, greenArea.sizeDelta.y);
            lightComponent.Failed(() =>
            {
                PlayShakeAnim(true);
                PlayBrokenAnim(false);
                if (HaveEnoughLockPick())
                {
                    UseLockPick();
                    RefreshPrivate();
                }
                else
                {
                    RefreshPrivate();
                    _state = OpenOperationStage.LackLockPick;
                }
                UICommpont.UnFreezeUI(FreezeKey);
            });
        }
    }

    private void OnOpen()
    {
        _prop.UnlockSuccess();
        var group = successPage.gameObject.GetComponent<CanvasGroup>();
        group.gameObject.OnActive(true);
        group.alpha = 0;
        group.DOFade(1, 0.4f).SetUpdate(true);
    }

    /// <summary>
    /// 是否拥有足够的开锁器
    /// </summary>
    /// <returns></returns>
    private bool HaveEnoughLockPick()
    {
        return BattleController.GetCtrl<BagPackCtrl>().GetBagItemNum(lockPickId) > 0;
    }

    private void UseLockPick()
    {
        BattleController.GetCtrl<BagPackCtrl>().ConsumeItem(lockPickId,1);
    }


    private void OpenWatchAd()
    {
        //打开观看广告的界面
        CommonPopup.Popup(Language.GetContent("701"),Language.GetContent("745"),lockPickSprite,new PupupOption()
        {
            title = Language.GetContent("707"),
            action = ()=>{
                UIController.Instance.Back();
            }
        },new PupupOption()
        {
            title = Language.GetContent("746"),
            subIcon = adSprite,
            action = () =>
            {
                CommonPopup.Close();
                OnWatchAd();
            }
        });
    }
    
    private void OnWatchAd()
    {
        GameDebug.Log("看广告成功！");
        //弹出获得开锁器的弹框
        var count = int.Parse(adData.rewardCount);
        PropEntity.GetEntity(lockPickId).GetReward(count, 0);
        //title Language.GetContent("701")
        CommonPopup.Popup("", string.Format(Language.GetContent("734") , count), lockPickSprite,
            new PupupOption()
            {
                title = Language.GetContent("702"),
                action = () =>
                {
                    RefreshPrivate();
                }
            });
        //刷新最后失败之前进度，还是重置到开始进度？
    }


    private void OnHit()
    {
        GameDebug.Log("命中！");
        PlayShakeAnim(false);
        RotateLock(() =>
        {
            //播放进度条撑满的动画
            StartCoroutine(GreenAreaFullAnim(() =>
            {
                lightComponent.Success(_curIndex + 1, () =>
                {
                    greenAreaAnim = greenArea.DOSizeDelta(new Vector2(0, greenArea.sizeDelta.y), 0.3f).SetUpdate(true).OnComplete(() =>
                    {
                        PlayShakeAnim(true);
                        _curIndex++;
                        if (_curIndex == lockParam.lockNum) //打开了！
                        {
                            PlayShakeAnim(false);
                            GameDebug.Log("打开了！");
                            OnOpen();
                        }
                        else
                        {
                            RefreshCurStage(true, null);
                        }
                        UICommpont.UnFreezeUI(FreezeKey);
                    });
                });
            }));
        });
    }

    private IEnumerator GreenAreaFullAnim(Action callback)
    {
        bool left = true;
        bool right = true;
        while (left || right)
        {
            yield return 0;
            if (greenArea.offsetMin.x <= 0)
            {
                left = false;
                greenArea.offsetMin = new Vector2(0, greenArea.offsetMin.y);
            }
            else
            {
                greenArea.offsetMin = new Vector2(greenArea.offsetMin.x - Time.unscaledDeltaTime * 1200f,greenArea.offsetMin.y);
            }
            if (greenArea.offsetMax.x >= processBarWidth + 20)
            {
                right = false;
                greenArea.offsetMax = new Vector2(processBarWidth + 20, greenArea.offsetMax.y);
            }
            else
            {
                greenArea.offsetMax = new Vector2(greenArea.offsetMax.x + Time.unscaledDeltaTime * 1200f,greenArea.offsetMax.y);  
            }
        }  
        
        callback?.Invoke();
    }

    private void RotateLock(Action callback)
    {
        var angle = new Vector3(0, 0, (180 / lockParam.lockNum) * (_curIndex + 1));
        lockIn.DOLocalRotate(-angle, 1).OnComplete(() =>
        {
            callback?.Invoke();
        }).SetUpdate(true);
        lockPickRoot.DOLocalRotate(-angle, 1).SetUpdate(true);
    }
    
    private void ResetLock()
    {
        lockIn.transform.rotation = Quaternion.Euler(0,0,0);
        lockPick.transform.localRotation = Quaternion.Euler(0,0,0);
        lockPickRoot.transform.rotation = Quaternion.Euler(0,0,0);
        _curIndex = 0;
        _state = OpenOperationStage.ArrowStop;
        _arrowPosNormalized = 0;
    }

    private void PlayBrokenAnim(bool play)
    {
        var anims = lockPick.transform.GetChild(0).GetComponents<DOTweenAnimation>();
        for (int i = 0; i < anims.Length; i++)
        {
            if (play)
            {
                anims[i].DORestart();
            }
            else
            {
                anims[i].DOPause();
            }
        }
    }

    private void PlayShakeAnim(bool play)
    {
        var anims = lockPickRoot.GetChild(0).GetComponents<DOTweenAnimation>();
        for (int i = 0; i < anims.Length; i++)
        {
            if (play)
            {
                anims[i].DOPlay();
            }
            else
            {
                anims[i].DOPause();
            }

        }
    }
}