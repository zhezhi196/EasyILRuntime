using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Module;
using DG.Tweening;
using UnityEngine.UI;
namespace ProjectUI
{
    public class GameUI : UIViewBase, ILocaltionReadingUI
    {
        [Header("移动控制")]
        public GameObject leftStick;//移动
        public UIScrollBase leftJoyStick;
        public RectTransform viewPivot;//摇杆背景
        public RectTransform pot;//摇杆点
        public GameObject runRoot;
        //public Image runBG;
        public RectTransform runRt;//跑步判定区域
        private float radius;//点移动半径
        private bool moveHide = false;//持续移动,隐藏ui参数
        private float hideTime = 0.5f;//持续移动,隐藏ui参数
        private bool isRun = false;
        public UIScrollBase rightJoyStick;
        public CanvasGroup hideGroup;
        [Header("交互按钮")]
        public UIScrollBase attackBtn;
        public UIBtnBase attackBtn1;
        public UIBtnBase aimBtn;
        public Sprite[] aimImg;
        public UIBtnBase reloadBtn;
        public UIBtnBase crouchBtn;
        private bool showCrouchBtn = true;//下蹲按钮是否可以显示
        public GameObject gameCtrlGroup;
        public Sprite[] crouchImg;
        public TopBtnsGroup topBtnsGroup;
        public Transform handNode;
        public UIBtnBase hideBtn;
        public UIBtnBase dodgeBtn;
        public CanvasGroup uiCanvas;
        [Header("武器")]
        public UIBtnBase weaponBtn;
        public Image weaponTip;
        public Image weaponIcon;
        public Text bulletCount;
        public Text maxBulletCount;
        public WeaponGiftSlot weaponGiftSlot;
        public UIBtnBase weaponSelectMask;
        public Transform weaponBgTrans;
        private List<UIInteractionNode> interNodes = new List<UIInteractionNode>();
        private bool isLoadInternodes;
        [Header("效果")]
        public Image strengthCircle;
        public Color cNormalColor = Color.white;
        public Color cWeakColor = Color.red;

        public GameUIPlayerState playerStateUI;
#if LOG_ENABLE
        public Text labdelText;
#endif
        protected override void OnChildStart()
        {
#if LOG_ENABLE
            labdelText.gameObject.OnActive(true);
#endif
            //-----移动旋转-----
            leftJoyStick.AddDragDown(StartMove);
            leftJoyStick.AddDrag(RoleMove);
            leftJoyStick.AddDragUp(StopMove);
            radius = pot.sizeDelta.x * 0.5f;//计算点移动半径
            rightJoyStick.AddDragDown(OnStartRotate);
            rightJoyStick.AddDrag(OnDragUI);
            rightJoyStick.AddDragUp(OnStopRotate);
            //------交互按钮-----
            attackBtn.AddDragPrepare(AttackScrollDown);
            attackBtn1.AddPointDown(AttackBtnDown);
            attackBtn.AddPointUp(AttackScrollUp);
            attackBtn1.AddPointUp(AttackBtnUp);
            attackBtn.AddDrag(AttackScrollDrag);
            reloadBtn.AddListener(ReloadBtnDown);
            //aimBtn.AddListener(AimBtnDown);
            crouchBtn.AddListener(CrouchBtnDown);
            weaponBtn.AddListener(WeaponBtnDown);
            weaponSelectMask.AddListener(CloseWeaponSelect);
            weaponBgTrans.localScale = Tools.GetScreenScale();
           // showCrouchBtn = TeachingManager.CompleteTeaching(TeachingName.CrouchTeaching);
            crouchBtn.OnActive(showCrouchBtn);
            hideBtn.AddListener(HideBtnDown);
            dodgeBtn.AddListener(DodgeBtnDown);
            //------事件-------
            Player.player.onAddStation += OnPlayerAddStation;
            Player.player.onRemoveStation += OnPlayerRemoveStation;
            //Player.player.weaponManager.OnGetNewWeapon += OnGetNewWeapon;
            PropsCtrl.RegisterLookPointEvent(ShowHand);
            BulletEntity.onBulletCountChanged += OnBulletChange;
            EventCenter.Register<Weapon, int>(EventKey.WeaponBulletChange, OnWeaponBulletChange);
            //EventCenter.Register<TeachingName>(EventKey.GameTeachStart, OnTeachingStart);
        }

        public override void Refresh(params object[] args)
        {
            if (Player.player.currentWeapon.weaponType == WeaponType.Empty)
            {
                weaponBtn.OnActive(false);
                attackBtn.OnActive(false);
                attackBtn1.OnActive(false);
                reloadBtn.OnActive(false);
                //aimBtn.OnActive(false);
            }
            else if (Player.player.currentWeapon.weaponType == WeaponType.MeleeWeapon)
            {
                bulletCount.text = "1";
                maxBulletCount.text = "1";
                SpriteLoader.LoadIcon(Player.player.currentWeapon.entity.weaponData.icon, (s) =>
                {
                    weaponIcon.sprite = s;
                    weaponBtn.OnActive(true);
                });
                attackBtn.OnActive(true);
                attackBtn1.OnActive(true);
                reloadBtn.OnActive(false);
                //aimBtn.OnActive(false);
            }
            else
            {
                bulletCount.text = Player.player.currentWeapon.bulletCount.ToString();
                reloadBtn.OnActive(true);
                //aimBtn.OnActive(true);
                maxBulletCount.text = Player.player.currentWeapon.bullet.bagCount.ToString();
                SpriteLoader.LoadIcon(Player.player.currentWeapon.entity.weaponData.icon, (s) =>
                {
                    weaponIcon.sprite = s;
                    weaponBtn.OnActive(true);
                });
                attackBtn.OnActive(true);
                attackBtn1.OnActive(true);
                ////每次回到gameui判断当前武器是否需要自动换弹,解决游戏暂停时换弹问题
                //if (Player.player.currentWeapon.bulletCount <= 0 && Player.player.currentWeapon.CanReload)
                //{
                //    Player.player.ReloadBtnDown();
                //}
            }

            //if (Player.player.weaponManager.meleeWeapon != null && Player.player.weaponManager.meleeWeapon.activeGife != null)
            //{
            //    SpriteLoader.LoadIcon(Player.player.weaponManager.meleeWeapon.activeGife.dbData.icon, (s) =>
            //    {
            //        meleeBtn.image.sprite = s;
            //        meleeBtn.OnActive(TeachingManager.TeachingIsStart(TeachingName.MeleeAttack));
            //    });
            //}
            //playerStateUI.Refesh();
        }

        private void OnDestroy()
        {
            PropsCtrl.UnRegisterLookPointEvent(ShowHand);
            BulletEntity.onBulletCountChanged -= OnBulletChange;
            EventCenter.UnRegister<Weapon, int>(EventKey.WeaponBulletChange, OnWeaponBulletChange);
            //EventCenter.UnRegister<TeachingName>(EventKey.GameTeachStart, OnTeachingStart);
        }

        #region 事件

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

        private void OnBulletChange(BulletEntity b, int count)
        {
            if (b.weapon == Player.player.currentWeapon)
            {
                maxBulletCount.text = b.bagCount.ToString();
            }
        }

        private void OnWeaponBulletChange(Weapon weapon, int change)
        {
            if (weapon == Player.player.currentWeapon)
            {
                bulletCount.text = weapon.bulletCount.ToString();
            }
        }

        private void OnPlayerAddStation(Player.Station station)
        {
            switch (station)
            {
                //case Player.Station.MeleeAttack:
                //    weaponBtn.OnActive(false);
                //    reloadBtn.OnActive(false);
                //    //aimBtn.OnActive(false);
                //    attackBtn.OnActive(false);
                //    attackBtn1.OnActive(false);
                //    //meleeBtn.OnActive(false);
                //    break;
                case Player.Station.WeaponChanging:
                    weaponBtn.OnActive(false);
                    reloadBtn.OnActive(false);
                    //aimBtn.OnActive(false);
                    break;
                case Player.Station.Aim:
                    //runRoot.OnActive(false);
                    //runRt.localScale = Vector3.zero;
                    //aimBtn.image.sprite = aimImg[1];
                    AddAim();
                    if (strengthCircle.transform.parent.gameObject.activeSelf)
                        strengthCircle.transform.parent.gameObject.OnActive(false);
                    break;
                case Player.Station.Weak:
                    strengthCircle.transform.parent.gameObject.OnActive(true);
                    strengthCircle.color = cWeakColor;
                    break;
                case Player.Station.Running:
                    strengthCircle.transform.parent.gameObject.OnActive(true);
                    runRt.localScale = new Vector3(2.5f, 1.5f, 1f);
                    break;
                case Player.Station.Ass:
                case Player.Station.Excute:
                case Player.Station.Story:
                case Player.Station.Death:
                    gameCtrlGroup.OnActive(false);
                    break;
                case Player.Station.Reloading:
                case Player.Station.BowReloading:
                case Player.Station.ShotGunReload:
                    runRoot.OnActive(false);
                    runRt.localScale = Vector3.zero;
                    break;
                case Player.Station.Hiding:
                    hideBtn.OnActive(true);
                    break;
                case Player.Station.Dodge:
                    uiCanvas.blocksRaycasts = false;
                    break;
                default:
                    break;
            }
        }

        private void OnPlayerRemoveStation(Player.Station station)
        {
            switch (station)
            {
                case Player.Station.Ass:
                case Player.Station.Excute:
                case Player.Station.Story:
                case Player.Station.Death:
                    gameCtrlGroup.OnActive(true);
                    break;
                //case Player.Station.MeleeAttack:
                //    if (Player.player.currentWeapon.weaponType != WeaponType.Empty)
                //    {
                //        attackBtn.OnActive(true);
                //        attackBtn1.OnActive(true);
                //    }
                //    //meleeBtn.OnActive(true);
                //    break;
                case Player.Station.WeaponChanging:
                    if (Player.player.currentWeapon.weaponType == WeaponType.Empty)
                        return;
                    if (Player.player.currentWeapon.weaponType == WeaponType.MeleeWeapon)
                    {
                        bulletCount.text = "1";
                        maxBulletCount.text = "1";
                    }
                    else
                    {
                        bulletCount.text = Player.player.currentWeapon.bulletCount.ToString();
                        reloadBtn.OnActive(true);
                        //aimBtn.OnActive(true);
                        maxBulletCount.text = Player.player.currentWeapon.bullet.bagCount.ToString();
                    }
                    SpriteLoader.LoadIcon(Player.player.currentWeapon.entity.weaponData.icon, (s) =>
                    {
                        weaponIcon.sprite = s;
                        weaponBtn.OnActive(true);
                    });
                    attackBtn.OnActive(true);
                    attackBtn1.OnActive(true);
                    break;
                case Player.Station.Aim:
                    //aimBtn.image.sprite = aimImg[0];
                    if (Player.player.ContainStation(Player.Station.Weak))
                    {
                        strengthCircle.transform.parent.gameObject.OnActive(true);
                    }
                    break;
                case Player.Station.Weak:
                    strengthCircle.transform.parent.gameObject.OnActive(false);
                    strengthCircle.color = cNormalColor;
                    break;
                case Player.Station.Running:
                    strengthCircle.transform.parent.gameObject.OnActive(false);
                    runRt.localScale = Vector3.one;
                    break;
                case Player.Station.Hiding:
                    hideBtn.OnActive(false);
                    break;
                case Player.Station.Dodge:
                    uiCanvas.blocksRaycasts = true;
                    break;
                default:
                    break;
            }
        }

        private void OnGetNewWeapon(int id)
        {
            weaponTip.gameObject.OnActive(true);
        }

        //private void OnTeachingStart(TeachingName teachingName)
        //{
        //    if (teachingName == TeachingName.CrouchTeaching)
        //    {
        //        showCrouchBtn = true;
        //    }
        //}
        #endregion

        #region 移动旋转
        public void StartMove(Vector2 arg1, Vector2 arg2)
        {
            //BattleController.Instance.Continue();//解决游戏暂定bug,待测试
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
            if (arg3 > hideTime && !moveHide)
            {
                moveHide = true;
                hideGroup.DOFade(0, 0.2f).SetId(this.GetInstanceID());
            }
            UpdateRunState(arg1);
        }

        public void StopMove(Vector2 arg1, Vector2 arg2)
        {
            Player.player?.StopMove();
            if (moveHide)
            {
                moveHide = false;
                if (hideGroup.alpha > 0)
                {
                    DOTween.Kill(this.GetInstanceID());
                }
                hideGroup.alpha = 1;
            }
            if (isRun)
            {
                isRun = !isRun;
                Player.player.IsRun = isRun;
            }
            runRoot.OnActive(false);
        }
        bool isAddAim = false;
        private void UpdateRunState(Vector2 arg1)
        {
            bool inArea = RectTransformUtility.RectangleContainsScreenPoint(runRt, arg1, CameraCtrl.Instance.uiCamera);
            //判定是否在跑动区域
            if (Player.player.NotRun == runRt.localScale.x > 0.01f)
            {
                runRoot.OnActive(!Player.player.NotRun);
                runRt.localScale = Player.player.NotRun ? Vector3.zero : Vector3.one;
            }
            if (inArea)
            {
                if (isAddAim)//添加瞄准，打断跑步
                {
                    if (Player.player.IsRun)
                        Player.player.IsRun = false;
                    return;
                }
                if (isRun != inArea)
                {
                    isRun = !isRun;
                    Player.player.IsRun = isRun;
                }
            }
            else {
                if (isAddAim)//退出跑步区域，重置addaim状态
                    isAddAim = false;
                if (isRun)
                {
                    isRun = false;
                    Player.player.IsRun = false;
                }
            }

            //if (isRun != RectTransformUtility.RectangleContainsScreenPoint(runRt, arg1, CameraCtrl.Instance.uiCamera))
            //{
            //    isRun = !isRun;
            //    Player.player.IsRun = isRun;
            //}
        }

        private void AddAim()
        {
            isAddAim = true;
        }

        private void OnStartRotate(Vector2 arg1, Vector2 arg2)
        {
            //BattleController.Instance.Continue();//解决游戏暂定bug,待测试
            rotateScrollDown = true;
            if (!attackScrollDown)
                Player.player?.StartRotate();
        }

        private void OnDragUI(Vector2 arg1, Vector2 arg2, float arg3)
        {
            Player.player?.Rotate(arg2);
        }

        private void OnStopRotate(Vector2 arg1, Vector2 arg2)
        {
            rotateScrollDown = false;
            if (!attackScrollDown)
                Player.player?.StopRotate();
        }
        #endregion

        #region 交互按钮
        private bool attackBtnDown = false;
        private bool rotateScrollDown = false;
        private bool attackScrollDown = false;
        private void AttackBtnDown(ButtonTriggerType triggerType)
        {
            if (!attackBtnDown)
            {
                attackBtnDown = true;
                Player.player.AttackBtnDown();
            }
        }

        private void AttackBtnUp(ButtonTriggerType triggerType)
        {
            if (attackBtnDown)
            {
                attackBtnDown = false;
                Player.player.AttackBtnUp();
            }
        }
        private void AttackScrollDown(Vector2 pos)
        {
            attackScrollDown = true;
            if (!rotateScrollDown)
                Player.player?.StartRotate();
            if (!attackBtnDown)
            {
                attackBtnDown = true;
                Player.player.AttackBtnDown();
            }
        }
        private void AttackScrollDrag(Vector2 arg1, Vector2 arg2, float arg3)
        {
            Player.player?.Rotate(arg2);
        }
        private void AttackScrollUp(Vector2 pos)
        {
            attackScrollDown = false;
            if (!rotateScrollDown)
                Player.player?.StartRotate();
            if (attackBtnDown)
            {
                attackBtnDown = false;
                Player.player.AttackBtnUp();
            }
        }

        private void AimBtnDown()
        {
            Player.player.AimBtnDown();
        }

        private void ReloadBtnDown()
        {
            Player.player.ReloadBtnDown();
        }

        private void CrouchBtnDown()
        {
            Player.player.StandOrSquat();
        }

        private void MeleeBtnDown(ButtonTriggerType triggerType)
        {
            Player.player.MeleeAttackBtnDown();
        }

        private void MeleeBtnUp(ButtonTriggerType triggerType)
        {
            Player.player.MeleeAttackBtnUp();
        }

        public void WeaponBtnDown()
        {
            BattleController.Instance.Pause(winName);
            weaponGiftSlot.Show();
            //weaponTip.gameObject.OnActive(false);
            //AudioPlay.Play("zhuangpei_tanchu");
        }

        public void HideBtnDown()
        {
            Player.player.ExitHideProp();
        }
        public void DodgeBtnDown()
        {
            Player.player.Dodge();
        }
        public void CloseWeaponSelect()
        {
            Refresh();
            BattleController.Instance.Continue(winName);
            weaponGiftSlot.Close();
        }

        public override void OnExit(params object[] args)
        {
            BattleController.Instance.Pause("GameUIExit");
            CommonPopup.Popup(Language.GetContent("701"), Language.GetContent("712"), null,
                new PupupOption(() =>
                {
                    BattleController.Instance.Save();
                    BattleController.Instance.ExitBattle(OutGameStation.Break);
                }, Language.GetContent("713")),
                new PupupOption(() => {
                    BattleController.Instance.Continue("GameUIExit");
                }, Language.GetContent("703")));
        }

        protected override void Update()
        {
            base.Update();
            strengthCircle.fillAmount = Player.player.strength / Player.player.playerAtt.strength;
#if LOG_ENABLE
            labdelText.text = string.Format("生命{0}\n能量{1}\n体力{2}", Player.player.hp, Player.player.energy, Player.player.strength);
#endif
        }

        private void LateUpdate()
        {
            for (int i = 0; i < interNodes.Count; i++)
            {
                interNodes[i].UpdateNode(handNode);
            }
            if (showCrouchBtn)
            {
                if (crouchBtn.gameObject.activeSelf != Player.player.canStand)
                {
                    crouchBtn.gameObject.OnActive(Player.player.canStand);
                }
                if (crouchBtn.image.sprite != crouchImg[Player.player.isSquat.ToInt()])
                {
                    crouchBtn.image.sprite = crouchImg[Player.player.isSquat.ToInt()];
                }
            }
        }

        public string uiName => "GameUI";
        
        public void PlayFade(Action callBack)
        {
            uiCanvas.DOFade(1, 0.5f).OnComplete(() =>
            {
                callBack?.Invoke();
            });
        }
        
    }
    #endregion
    

}
