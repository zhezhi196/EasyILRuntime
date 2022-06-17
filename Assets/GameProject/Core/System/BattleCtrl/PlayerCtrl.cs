using Module;
using System;
using System.Text;
using UnityEngine;

/// <summary>
/// 存档的读取,PlayerCtrl在startbattle读取
/// 武器的存档,在weapon.init读取
/// player的存档,在bronPlayer,player.showplayer读取
/// </summary>
public class PlayerCtrl : BattleSystem,ILocalSave
{
    private string prefabPath = "Agent/Player/Player.prefab";
    public Player player;
    public RunTimeAction loadPlayer;
    
    public RunTimeAction bronPlayer;
    public RunTimeAction bronAnim;
    public RunTimeAction openGameUi;
    public RunTimeAction endTimeline;
    public RunTimeAction endAction;
    public int fireCount = 0;
    public int deathCount = 0;
    public override void StartBattle(EnterNodeType enterType)
    {
        //if (enterType == EnterNodeType.FromSave)
        //{
        //    ReadData();
        //}
        //加载玩家预制体
        loadPlayer = new RunTimeAction(() =>
        {
            //加载角色预制体
            AssetLoad.LoadGameObject<Player>(prefabPath, BattleController.Instance.heroRoot, (obj, arg) =>
            {
                if (enterType == EnterNodeType.Restart)//开始新游戏添加一个注射器
                    BattleController.GetCtrl<BagPackCtrl>().PutToBag(PropEntity.GetEntity(20032), 1, -1, null, 0);

                player = obj;
                player.gameObject.OnActive(false);
                loadPlayer = null;
                player.Init();
                //武器升级,皮肤存档,初始化
                for (int i = 0; i < player.weaponManager.weaponList.Count; i++)
                {
                    player.weaponManager.weaponList[i].Init(player, enterType == EnterNodeType.FromSave);
                    if (WeaponManager.weaponAllEntitys.ContainsKey(player.weaponManager.weaponList[i].weaponID)
                        && WeaponManager.weaponAllEntitys[player.weaponManager.weaponList[i].weaponID].isGet)
                    {
                        player.weaponManager.ownWeapon.Add(player.weaponManager.weaponList[i]);
                        player.weaponManager.AddToSolt(player.weaponManager.weaponList[i]);
                        player.weaponManager.weaponList[i].level = player.weaponManager.weaponList[i].entity.level;
                    }
                }
                player.weaponManager.defualtWeapon.Init(player, false);//初始化武器武器控制器
                CameraCtrl.AddCamera(player.cameraCtrl.pCamera);
                CameraCtrl.AddCamera(player.cameraCtrl.evCamera);
                BattleController.Instance.NextFinishAction("LoadPlayer");
            });
        });
        openGameUi = new RunTimeAction(() =>
        {
            UIController.Instance.Open("GameUI", UITweenType.Fade);
            BattleController.Instance.NextFinishAction("openGameUi");
            openGameUi = null;
        });
        EventCenter.Register<int>(EventKey.OnGetWeapon, OnGetWeapon);
        EventCenter.Register<int, int>(EventKey.OnWeaponUpgrade, OnWeaponUpgrade);
    }

    public override void OnNodeEnter(NodeBase node, EnterNodeType enterType)
    {
        if (enterType != EnterNodeType.NextNode && enterType != EnterNodeType.SkipNode)
        {
            //游戏开始,播放动画之类的操作
            bronPlayer = new RunTimeAction(() =>
            {
                //开始游戏到复活点
                player.characterController.enabled = false;
                player.transform.position = (node as TaskNode).nodeParent.playerCreator.transform.position;
                player.transform.eulerAngles = (node as TaskNode).nodeParent.playerCreator.transform.eulerAngles;
                player.gameObject.OnActive(true);
                player.ShowPlayer(enterType);//会读取存档
                // AudioPlay.defaultListener.enabled = false;
                AudioManager.listener.enabled = false;
                EventCenter.Dispatch(EventKey.OnPlayerComplete);
                player.characterController.enabled = true;
                if (player.hp <= 0)
                {
                    player.AddStation(Player.Station.Death);
                    player.WaitAlive();
                }
                BattleController.Instance.NextFinishAction("BronPlayer");
                bronPlayer = null;
            });
            bronAnim = new RunTimeAction(() =>
            {
                BattleController.Instance.ctrlProcedure.currentNode.nodeParent.playerCreator.TrySendEvent(SendEventCondition.PlayerBorn);
                TimelineController timeline = BattleController.GetCtrl<TimelineCtrl>().GetReviveAnimtion(player.transform.position);
                if (timeline != null)
                {
                    //cameraRoot.DOLocalRotate(Vector3.zero, 0.2f).SetId(gameObject);
                    player.AddStation(Player.Station.Story);
                    player.timelineModel.Show(PlayerTimelineModel.ShowModel.None);
                    player.currentWeapon.TakeBack();
                    player.transform.position = timeline.playerPoint.position;
                    player.transform.rotation = timeline.playerPoint.rotation;
                    player._cameraFllowTrans = player.timelineModel.camerTrans;
                    timeline.Play(player, null, () =>
                    {
                        //player.characterController.enabled = true;
                        player.timelineModel.Hide();
                        player.currentWeapon.TakeOut();
                        player.RemoveStation(Player.Station.Story);
                        player._cameraFllowTrans = player.currentWeapon.cameraPoint;
                        //inTimeline = false;
                        BattleController.Instance.NextFinishAction("BronAnim");
                        bronAnim = null;
                    });
                }
                else
                {
                    //模型切换
                    player.currentWeapon.TakeOut();
                    player.timelineModel.Hide();
                    player._cameraFllowTrans = player.currentWeapon.cameraPoint;
                    BattleController.Instance.NextFinishAction("BronAnim");
                    bronAnim = null;
                }
                GamePlay.Instance.firstGame = false;
            });
            //if (GamePlay.Instance.firstGame)
            //{
            //    GamePlay.Instance.firstGame = false;
            //}
        }
    }

    public override void ExitBattle(OutGameStation station)
    {
        EventCenter.UnRegister<int>(EventKey.OnGetWeapon, OnGetWeapon);
        EventCenter.UnRegister<int, int>(EventKey.OnWeaponUpgrade, OnWeaponUpgrade);
    }
    public override void Save()
    {
        for (int i = 0; i < player.weaponManager.ownWeapon.Count; i++)
        {
            player.weaponManager.ownWeapon[i].Save();
        }
        player.Save();
        //LocalSave.Write(this);
    }
    public override void OnTaskResult(bool result)
    {
        endAction = new RunTimeAction(() =>
        {
            RunTimeSequence endSequence = new RunTimeSequence();
            endSequence.OnComplete(() =>
            {
                BattleController.Instance.NextFinishAction("EndAction");
                endAction = null;
            });
            if (result)
            {
                //结局动画延迟0.5秒
                endSequence.Add(new RunTimeAction(async () =>
                {
                    await Async.WaitforSecondsRealTime(1f);
                    BlackMaskChange.Instance.Black();
                    UIController.Instance.Get("GameUI").viewBase.gameObject.OnActive(false);
                    player.gameObject.OnActive(false);
                    AudioPlay.defaultListener.enabled = true;
                    endSequence.NextAction();
                }));

                //游戏胜利结局动画
                endSequence.Add(new RunTimeAction(() =>
                {
                    BattleController.GetCtrl<TimelineCtrl>().GetEndTimeline(BattleController.Instance.ctrlProcedure.mission.difficulte, (timeline) =>
                    {
                        if (timeline == null)
                        {
                            endSequence.NextAction();
                        }
                        else
                        {
                            timeline.Play(null, null, () =>
                            {
                                endSequence.NextAction();
                            });
                        }
                    });
                }));

                //if (BattleController.Instance.ctrlProcedure.mission.difficulte == GameDifficulte.Nightmare)
                //{
                //    //加载场景
                //    endSequence.Add(new RunTimeAction(() =>
                //    {
                //        Loading.Open("", "EndAnim");
                //        GameScene.Load("EndScene", () =>
                //        {
                //            endSequence.NextAction();
                //        });
                //    }));
                //    //播放动画
                //    endSequence.Add(new RunTimeAction(() =>
                //    {
                //        TimelineController timeline = GameObject.Find("EndTimeline").GetComponent<TimelineController>();
                //        if (timeline != null)
                //        {
                //            AudioPlay.PlayBackGroundMusic("BGM_aiji");
                //            timeline.Play(null, null, () =>
                //            {
                //                endSequence.NextAction();
                //            });
                //        }
                //        else
                //        {
                //            endSequence.NextAction();
                //        }

                //        Loading.Close("", "EndAnim");
                //    }));
                //}
            }

            //结算ui界面
            endSequence.Add(new RunTimeAction(() =>
            {
                if (result)
                {
                    UIController.Instance.Open("SuccessUI", UITweenType.Fade, new System.Action(() =>
                    {
                        endSequence.NextAction();
                    }));
                }
                else
                {
                    UIController.Instance.Open("FailUI", UITweenType.Fade, new System.Action(() =>
                    {
                        endSequence.NextAction();
                    }));
                }
                BlackMaskChange.Instance.Close();
            }));
            endSequence.NextAction();
        });
    }

    public async void DragMoveTest()
    {
        GameObject obj = new GameObject();
        obj.transform.position = player.CenterPostion+ new Vector3(0,0.5f,0);
        await Async.WaitForEndOfFrame();
        player.StartDragMove(obj.transform,null);
        float t = 0f;
        while (t <= 1)
        {
            t += TimeHelper.deltaTime;
            obj.transform.position += player.transform.forward * 10f * TimeHelper.deltaTime;
            await Async.WaitForEndOfFrame();
        }
        GameObject.DestroyImmediate(obj);
        player.EndDragMove();
    }

    private void OnGetWeapon(int id)
    {
        player.GetWeapon(id);
    }
    private void OnWeaponUpgrade(int id, int up)
    {
        player.WeaponUpgrade(id, up);
    }

    public string localFileName => LocalSave.savePath;
    public string localGroup { get; }
    public string localUid { get; }
    public string GetWriteDate()
    {
        return "";
    }
}