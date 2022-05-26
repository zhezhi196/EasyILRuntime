using System;
using System.Collections.Generic;
using Module;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

public class MonsterCreator : SerializedMonoBehaviour, IMissionEditor, IEventReceiver, IEventSender
{
    public enum MonsterActiveState
    {
        Null,
        UnActive,
        Normal
    }
    public static List<MonsterCreator> creatList = new List<MonsterCreator>();

    [ReadOnly]
    public IMonster monster;
    private SaveStation saveStation;
    private string lastLogical;
    private string[] lastLogicalArgs;
    private EventReceiver[] _receiver;
    private EventSender[] _sender;
    
    [OnValueChanged("OnModelChanged")]
    public int id;
    [LabelText("标签"), OnValueChanged("OnModelChanged"),TextArea]
    private string _mark;
    [LabelText("加载节点"),OnValueChanged("LoadIdChanged")] public int loadNodeID = 1;
    [LabelText("卸载节点")] public int unloadNodeID = 2;
    [OnValueChanged("OnModelChanged"),LabelText("模型")] public MonsterModelName modeName;
    [LabelText("加载隐藏")] public bool loadHide;
    [LabelText("是否存档")] public bool save = true;
    [LabelText("不能寻路")] public bool notNav;
    [HideLabel] public ProgressOption progress;
    [BoxGroup("掉落物品id"),HideLabel] public int dropId;
    [LabelText("掉落匹配"),BoxGroup("掉落"),] public string[] matchInfo;

    [LabelText("阶段"), ListDrawerSettings(AddCopiesLastElement = true, CustomRemoveIndexFunction = "RemoveLevel", CustomAddFunction = "ReadLevel")]
    public List<MonsterLeveEditor> levelEditor = new List<MonsterLeveEditor>();

    [LabelText("事件发送")] public List<EventSenderEditor> eventSenders;
    [LabelText("事件接受")] public List<EventReciverEditor> eventRecivers;
    [HideLabel]
    public CreatorExtuil extuil;
    public ProgressOption progressOption => progress;

    public bool progressIsComplete
    {
        get
        {
            if (BattleController.Instance.ctrlProcedure.currentNode.id == loadNodeID)
            {
                if (monster != null)
                {
                    return !monster.isProgressComplete;
                }
                else
                {
                    return true;
                }
            }
            
            return true;
        }
    }

    public Vector3 GetTipsPos()
    {
        if (monster is AttackMonster att)
        {
            return att.excPoint.position;
        }

        return monster.transform.position;
    }

    public string localFileName => LocalSave.savePath;
    public string localGroup => "Monster";

    public string localUid => "MonsterSave_" + id;
    public int key => id;
    public EventReceiver[] receiver => _receiver;
    public EventSender[] sender => _sender;
    public bool isGet { get; set; }
    public MapType mapType => extuil.mapType;
    public string mapId => extuil.mapId;

    private void Awake()
    {
        creatList.Add(this);
        if (progress.index != -1)
        {
            BattleController.GetCtrl<ProgressCtrl>().allProgress.Add(this);
        }
    }


    private void OnDestroy()
    {
        if (!receiver.IsNullOrEmpty())
        {
            for (int i = 0; i < receiver.Length; i++)
            {
                receiver[i].Dispose();
            }
        }
        creatList.Remove(this);
    }

    public void OnEnterBattle(EnterNodeType type)
    {
        if (type == EnterNodeType.FromSave)
        {
            saveStation = BattleController.Instance.ctrlProcedure.GetSaveStation(unloadNodeID, loadNodeID, loadNodeID);
        }
    }

    public void OnNodeEnter(TaskNode node, Action<IMonster> callback)
    {
        var tempStation = saveStation;
        saveStation = SaveStation.None;
        if (tempStation == SaveStation.None)
        {
            if (node.id == unloadNodeID)
            {
                if (monster != null)
                {
                    RunLogical(RunLogicalName.Destroy, this, RunLogicalFlag.None, null);
                }

                callback?.Invoke(null);
            }
            else if (node.id == loadNodeID)
            {
                if (monster == null)
                {
                    LoadModel(() =>
                    {
                        if (loadHide)
                        {
                            monster.transform.gameObject.OnActive(false);
                        }
                        else
                        {
                            monster.Born();
                        }

                        callback?.Invoke(monster);
                    });
                }
                else
                {
                    GameDebug.LogError("Monster is exited");
                }
            }
            else
            {
                callback?.Invoke(monster);
            }
        }
        else if (tempStation == SaveStation.Load || tempStation == SaveStation.Init)
        {
            string data = LocalSave.Read(this);
            string[] spite = data.Split(ConstKey.Spite0);

            initStation = (InitStation) spite[0].ToInt();
            MonsterActiveState activeState = (MonsterActiveState) spite[1].ToInt();

            if (initStation == InitStation.OnlyCreat || initStation == InitStation.Normal)
            {
                if (activeState != MonsterActiveState.Null)
                {
                    LoadModel(() =>
                    {
                        monster.initStation = initStation;
                        if (activeState == MonsterActiveState.Normal)
                        {
                            monster.Born();
                        }
                        else if (activeState == MonsterActiveState.UnActive)
                        {
                            monster.transform.gameObject.OnActive(false);
                        }

                        callback?.Invoke(monster);
                    });
                }
                else
                {
                    callback?.Invoke(monster);
                }
            }
            //这里已经死了
            else if (initStation > InitStation.Normal)
            {
                LoadModel(() =>
                {
                    callback?.Invoke(monster);
                    if (monster is Monster mons)
                    {
                        if (mons.deadDestroy)
                        {
                            AssetLoad.Destroy(mons.transform.gameObject);
                        }
                        else
                        {
                            mons.initStation = initStation;
                            mons.transform.gameObject.OnActive(false);
                        }
                    }
                });
            }
            else
            {
                callback?.Invoke(monster);
            }
        }
        else if (tempStation == SaveStation.Unload)
        {
            //isCreat = true;
            //到这里说明这个怪卸载了
            callback?.Invoke(null);
        }
    }

    public string GetWriteDate()
    {
        MonsterActiveState MonsterSt = MonsterActiveState.Normal;

        if (monster == null)
        {
            MonsterSt = MonsterActiveState.Null;
        }
        else
        {
            if (loadHide)
            {
                MonsterSt = MonsterActiveState.UnActive;
            }
        }

        return (int) initStation + ConstKey.Spite0.ToString() + (int) MonsterSt;
    }

    public void ReceiveEvent()
    {
        _receiver = CommonTools.CreatReceiver(eventRecivers, this);
        _sender = CommonTools.CreatSender(eventSenders, this);
    }

    public void LoadModel(Action callback)
    {
        if (monster == null)
        {
            AssetLoad.LoadGameObject($"Monster/{modeName}/{modeName}.prefab", transform, (resu, arg) =>
            {
                monster = resu.GetComponent<IMonster>();
                monster.OnCreat(this);
                //isCreat = true;
                callback?.Invoke();
            });
        }
    }

    public void EventCallback(int eventID, IEventCallback receiver)
    {
    }

    public void RunLogical(RunLogicalName logical, IEventCallback sender, RunLogicalFlag flag, string senderArg, params string[] args)
    {
        if (monster != null)
        {
            if (!CommonTools.CommonLogical(logical, args))
            {
                switch (logical)
                {
                    case RunLogicalName.Reset:
                        monster.ResetToBorn();
                        break;
                    case RunLogicalName.Dead:
                        if (monster is AttackMonster att)
                        {
                            att.SwitchStation(0);
                            att.OnHurt(att.monsterParts[0], new Damage() {damage = Int32.MaxValue,weapon = WeaponType.Knife});
                        }

                        break;
                    case RunLogicalName.SwitchBt:
                        if (args[0] == "Chase" && monster is AttackMonster attack)
                        {
                            attack.Chase();
                        }
                        //todo 切行为树
                        //monster.GetAgentCtrl<SimpleBehaviorCtrl>().SwitchBehavior(args[0]);
                        break;
                    case RunLogicalName.Destroy:
                    case RunLogicalName.ForceDestroy:
                        MonsterCtrl ctrl = BattleController.GetCtrl<MonsterCtrl>();
                        if (ctrl != null)
                        {
                            ctrl.TryRemoveMonster(monster);
                        }

                        AssetLoad.Destroy(monster.transform.gameObject);
                        break;
                    case RunLogicalName.Show:
                        monster.gameObject.OnActive(true);
                        monster.Born();
                        break;
                    case RunLogicalName.StopAttack:
                        monster.StopAttack();
                        break;
                }
            }
        }
    }
    
    public void TrySendEvent(SendEventCondition predicate, params string[] arg)
    {
        if (!sender.IsNullOrEmpty())
        {
            GameDebug.LogFormat("{0}尝试发送{1}的条件", id, predicate);
            for (int i = 0; i < sender.Length; i++)
            {
                if (sender[i].predicate == predicate)
                {
                    sender[i].TrySendEvent(arg);
                }
            }
        }
    }

    public bool TryPredicate(SendEventCondition predicate, string[] sendArg, string[] predicateArgs)
    {
        switch (predicate)
        {
            case SendEventCondition.None:
                return true;
            case SendEventCondition.AddStation:
            case SendEventCondition.RemoveStation:
                for (int i = 0; i < sendArg.Length; i++)
                {
                    sendArg[i] = sendArg[i].ToLower();
                }

                for (int i = 0; i < predicateArgs.Length; i++)
                {
                    predicateArgs[i] = predicateArgs[i].ToLower();
                }
                return sendArg.IsSame(predicateArgs);
            case SendEventCondition.Dead:
                return monster != null && !monster.isAlive;
        }

        return false;
    }

    public void Save()
    {
        LocalSave.Write(this);
    }

    #region Editor

    public InitStation initStation;
#if UNITY_EDITOR

    private Mesh[] skin;
    private IMonster m;

    private void LoadIdChanged()
    {
        unloadNodeID = loadNodeID + 1;
    }

    private void CheckProgress()
    {
        NodeParent parent = transform.GetComponentInParent<NodeParent>();
        string[] spite = parent.prefab.Split('/');
        string path = parent.prefab.Substring(0,parent.prefab.Length - spite.Last().Length-1);
        MissionGraph graph = UnityEditor.AssetDatabase.LoadAssetAtPath<MissionGraph>(path + ".asset");
        graph.CheckProgress(progress.index);
    }
    

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            if (monster == null)
            {
                Gizmos.color = Color.red;
                if (skin.IsNullOrEmpty() || m == null)
                {
                    var goo=UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>($"Assets/{ConstKey.GetFolder(AssetLoad.AssetFolderType.Bundle)}/Monster/{modeName}/{modeName}.prefab");
                    m = goo.GetComponent<IMonster>();
                    if (m != null)
                    {
                        var skinn = m.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>(true);
                        if (!skinn.IsNullOrEmpty())
                        {
                            skin = new Mesh[skinn.Length];
                            for (int i = 0; i < skin.Length; i++)
                            {
                                skin[i] = skinn[i].sharedMesh;
                            }
                        }
                        else
                        {
                            MeshFilter[] fitter = m.gameObject.GetComponentsInChildren<MeshFilter>(true);
                            if (fitter != null)
                            {
                                skin = new Mesh[fitter.Length];
                                for (int i = 0; i < fitter.Length; i++)
                                {
                                    skin[i] = fitter[i].sharedMesh;
                                }
                            }
                        }
                    }
                }

                if (skin != null)
                {
                    for (int i = 0; i < skin.Length; i++)
                    {
                        Gizmos.DrawMesh(skin[i], transform.position,  transform.rotation);
                    }
                }


                // if (!Application.isPlaying)
                // {
                //     if (m != null && m.eye != null)
                //     {
                //         Gizmos.color = Color.green;
                //         DrawTools.DrawCameraView(transform, m.eye.viewAngle, m.eye.aspect, m.eye.viewDistance);
                //     }
                // }
            }

            if (!levelEditor.IsNullOrEmpty())
            {
                for (int i = 0; i < levelEditor.Count; i++)
                {
                    if (!levelEditor[i].patrolPoint.IsNullOrEmpty())
                    {
                        for (int j = 0; j < levelEditor[i].patrolPoint.Count; j++)
                        {
                            if (levelEditor[i].patrolPoint[j] == null)
                            {
                                levelEditor[i].patrolPoint.RemoveAt(j);
                                return;
                            }

                            Gizmos.color = new Color(0.3767355f, 0.9622642f, 0.9507972f);
                            if (levelEditor[i].patrolPoint[j].gameObject != null)
                            {
                                Gizmos.DrawCube(levelEditor[i].patrolPoint[j].position, Vector3.one * 0.3f);
                                Gizmos.DrawLine(levelEditor[i].patrolPoint[j].position,
                                    levelEditor[i].patrolPoint[(j + 1) % levelEditor[i].patrolPoint.Count].position);
                            }
                        }
                    }

                    if (!levelEditor[i].escapePoint.IsNullOrEmpty())
                    {
                        for (int j = 0; j < levelEditor[i].escapePoint.Count; j++)
                        {
                            if (levelEditor[i].escapePoint[j] == null)
                            {
                                levelEditor[i].escapePoint.RemoveAt(j);
                                return;
                            }

                            Gizmos.color = Color.yellow;
                            if (levelEditor[i].escapePoint[j].gameObject != null)
                            {
                                Gizmos.DrawCube(levelEditor[i].escapePoint[j].position, Vector3.one * 0.3f);
                                var po = levelEditor[i].escapePoint[j];
                                Gizmos.DrawRay(po.position, po.forward );
                            }
                        }
                    }
                }
            }

        }
    }

    private void RemoveLevel(int index)
    {
        var tar = levelEditor[index];
        for (int i = 0; i < tar.patrolPoint.Count; i++)
        {
            if (tar.patrolPoint[i] != null)
            {
                DestroyImmediate(tar.patrolPoint[i].gameObject);
            }
        }
        
        for (int i = 0; i < tar.escapePoint.Count; i++)
        {
            if (tar.escapePoint[i] != null)
            {
                DestroyImmediate(tar.escapePoint[i].gameObject);
            }
        }

        levelEditor.RemoveAt(index);
    }
    
    [Button("重新刷新editor")]
    public void OnModelChanged()
    {
        gameObject.name = id + " " + _mark + " " + modeName;
        skin = null;
        var temp = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>($"Assets/{ConstKey.GetFolder(AssetLoad.AssetFolderType.Bundle)}/Monster/{modeName}/{modeName}.prefab");
        IMonster monster = temp.GetComponent<IMonster>();
        for (int i = 0; i < levelEditor.Count; i++)
        {
            levelEditor[i].skills.Clear();
            if (i < monster.defaultLevels.Count)
            {
                for (int j = 0; j < monster.defaultLevels[i].skills.Count; j++)
                {
                    levelEditor[i].skills.Add(monster.defaultLevels[i].skills[j]);
                }

                // levelEditor[i].viewAngle = monster.defaultLevel[i].viewAngle;
                // levelEditor[i].viewDistance = monster.defaultLevel[i].viewDistance;
                // levelEditor[i].hearRange = monster.defaultLevel[i].hearRange;
            }
        }
        
        ReadAllLevel(false);
    }

    public void ReadAllLevel(bool recreat)
    {
        var temp = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>($"Assets/{ConstKey.GetFolder(AssetLoad.AssetFolderType.Bundle)}/Monster/{modeName}/{modeName}.prefab");
        Monster mm = temp.GetComponent<Monster>();
        if (recreat)
        {
            levelEditor = new List<MonsterLeveEditor>();

            for (int i = 0; i < mm.levelEditor.Count; i++)
            {
                var leve = CopyData(new MonsterLeveEditor(), mm.levelEditor[i]);
                this.levelEditor.Add(leve);
            }
        }
        else
        {
            for (int i = 0; i < levelEditor.Count; i++)
            {
                MonsterLeveEditor monsterData = mm.defaultLevels[Mathf.Clamp(i, 0, mm.defaultLevels.Count - 1)];
                CopyData(levelEditor[i], monsterData);
            }
        }
    }

    private MonsterLeveEditor CopyData(MonsterLeveEditor leve, MonsterLeveEditor prefabData)
    {
        if (leve == null) leve = new MonsterLeveEditor();
        leve.creator = this;
        leve.camp = prefabData.camp;
        leve.skills = new List<Skill>(prefabData.skills);
        leve.animatorController = prefabData.animatorController;
        //leve.partEditor = new List<MonsterLeveEditor.MonsterPartEditor>(prefabData.partEditor);
        // leve.viewAngle = prefabData.viewAngle;
        // leve.viewDistance = prefabData.viewDistance;
        // leve.hearRange = prefabData.hearRange;
        //leve.partEditor = prefabData.partEditor;
        MonsterConfig config = AssetDatabase.LoadAssetAtPath<MonsterConfig>("Assets/Config/MonsterConfig.asset");
        NodeParent node = transform.GetComponentInParent<NodeParent>();
        var info = config.config.Find(fd => fd.normalId == prefabData.dataId);

        if (info != null)
        {
            var info2 = info.missionConfig.Find(fd => fd.missionId.ToString() == node.node.graph.name);
            if (info2 != null)
            {
                leve.dataId = info2.targetId;
                return leve;
            }
        }

        leve.dataId = prefabData.dataId;
        return leve;
    }

    private void ReadLevel()
    {
        var temp = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>($"Assets/{ConstKey.GetFolder(AssetLoad.AssetFolderType.Bundle)}/Monster/{modeName}/{modeName}.prefab");
        IMonster monster = temp.GetComponent<IMonster>();
        MonsterLeveEditor prefabLevelEditor = monster.defaultLevels[Mathf.Clamp(levelEditor.Count, 0, monster.defaultLevels.Count-1)];
        MonsterLeveEditor tar = CopyData(new MonsterLeveEditor(), prefabLevelEditor);
        levelEditor.Add(tar);
    }
    [Button]
    public void ReReadLevelWithConfig()
    {
        MonsterConfig config = AssetDatabase.LoadAssetAtPath<MonsterConfig>("Assets/Config/MonsterConfig.asset");
        NodeParent node = transform.GetComponentInParent<NodeParent>();
        var temp = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>($"Assets/{ConstKey.GetFolder(AssetLoad.AssetFolderType.Bundle)}/Monster/{modeName}/{modeName}.prefab");
        IMonster monster = temp.GetComponent<IMonster>();
        var info = config.config.Find(fd => fd.normalId == monster.defaultLevels[0].dataId);

        if (info != null)
        {
            var info2 = info.missionConfig.Find(fd => fd.missionId.ToString() == node.node.graph.name);
            if (info2 != null)
            {
                if (levelEditor.Count > 0)
                {
                    levelEditor[0].dataId = info2.targetId;
                }
            }
        }
    }

#endif

    #endregion

}