using Module;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackPlayerCtrl : BattleSystem
{
    private string prefabPath = "Agent/Player/Player.prefab";
    public Player player;
    public RunTimeAction loadPlayer;
    public RunTimeAction openGameUi;
    public RunTimeAction bronPlayer;
    public RunTimeAction bronAnim;
    public override void StartBattle(EnterNodeType enterType)
    {
        //加载玩家预制体
        loadPlayer = new RunTimeAction(() =>
        {
            //加载角色预制体
            AssetLoad.LoadGameObject<Player>(prefabPath, BattleController.Instance.heroRoot, (obj, arg) =>
            {
                player = obj;
                player.gameObject.OnActive(false);
                loadPlayer = null;
                player.Init();
                player.weaponManager.defualtWeapon.Init(player, false);//初始化武器武器控制器
                CameraCtrl.AddCamera(player.cameraCtrl.pCamera);
                CameraCtrl.AddCamera(player.cameraCtrl.evCamera);
                BattleController.Instance.NextFinishAction("LoadPlayer");
            });
        });
        openGameUi = new RunTimeAction(() =>
        {
            UIController.Instance.Open("BlackGameUI", UITweenType.Fade);
            BattleController.Instance.NextFinishAction("openGameUi");
            openGameUi = null;
        });
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
                AudioManager.listener.enabled = false;
                EventCenter.Dispatch(EventKey.OnPlayerComplete);
                player.characterController.enabled = true;
                BattleController.Instance.NextFinishAction("BronPlayer");
                bronPlayer = null;
            });
            bronAnim = new RunTimeAction(() =>
            {
                BattleController.Instance.ctrlProcedure.currentNode.nodeParent.playerCreator.TrySendEvent(SendEventCondition.PlayerBorn);
                TimelineController timeline = BattleController.GetCtrl<TimelineCtrl>().GetReviveAnimtion(player.transform.position);
                if (timeline != null)
                {
                    player.AddStation(Player.Station.Story);
                    player.timelineModel.Show(PlayerTimelineModel.ShowModel.None);
                    player.currentWeapon.TakeBack();
                    player.transform.position = timeline.playerPoint.position;
                    player.transform.rotation = timeline.playerPoint.rotation;
                    player._cameraFllowTrans = player.timelineModel.camerTrans;
                    timeline.Play(player, null, () =>
                    {
                        player.timelineModel.Hide();
                        player.currentWeapon.TakeOut();
                        player.RemoveStation(Player.Station.Story);
                        player._cameraFllowTrans = player.currentWeapon.cameraPoint;
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
        }
    }
}
