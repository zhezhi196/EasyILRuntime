using System.Collections.Generic;
using Module;
using UnityEngine;

internal class PlayerCtrl : BattleSystem
{
    public Player player;
    public List<IProtection> baseMent = new List<IProtection>();

    public RunTimeAction creatPlayer;
    public RunTimeAction creatBase;
    public RunTimeAction bornAll;

    public override void RestartBattle(MissionGraph editorGraph)
    {
        bornAll = new RunTimeAction(() =>
        {
            player.Born();
            for (int i = 0; i < baseMent.Count; i++)
            {
                baseMent[i].Born();
            }

            BattleController.Instance.NextFinishAction("bornAll");
            bornAll = null;
        });
    }

    public override void OnNodeEnter(TaskNode node)
    {
        if (player == null)
        {
            creatPlayer = new RunTimeAction(() =>
            {
                AssetLoad.LoadGameObject<Player>("Agent/Player/Player.prefab", BattleController.Instance.root, (p, arg) =>
                    {
                        player = p;
                        player.Creat(node.editorPrefab.playerCreator.Find(fd => fd.playerType == PlayerType.Player),
                            () =>
                            {
                                AssetLoad.LoadGameObject<PlayerCamera>("Agent/Player/Camera.prefab",
                                    BattleController.Instance.root, (go, arg) =>
                                    {
                                        player.camera = go;
                                        creatPlayer = null;
                                        BattleController.Instance.NextFinishAction("creatPlayer");
                                    });
                            });
                    });
            });
        }

        if (baseMent.IsNullOrEmpty())
        {
            creatBase = new RunTimeAction(() =>
            {
                Voter voter = new Voter(node.editorPrefab.playerCreator.Length, () =>
                {
                    creatBase = null;
                    BattleController.Instance.NextFinishAction("creatBase");
                });
                for (int i = 0; i < node.editorPrefab.playerCreator.Length; i++)
                {
                    var creator = node.editorPrefab.playerCreator[i];
                    if (creator.playerType != PlayerType.Player)
                    {
                        AssetLoad.LoadGameObject($"Base/{creator.playerType}/{creator.playerType}.prefab",
                            BattleController.Instance.root, (bas, arg) =>
                            {
                                IProtection baseMent = bas.GetComponent<IProtection>();
                                this.baseMent.Add(baseMent);
                                baseMent.Creat(creator);
                                voter.Add();
                            });
                    }
                    else
                    {
                        voter.Add();
                    }
                }
            });
        }
        else
        {
            if (Player.baseMent is ProtectedHuman wife)
            {
                PlayerCreator wifeCreator = node.editorPrefab.playerCreator.Find(fd => fd.playerType == PlayerType.HumanBase);
                if (wifeCreator != null)
                {
                    wife.Move(wifeCreator.transform.position);
                }
            }
        }
    }
}