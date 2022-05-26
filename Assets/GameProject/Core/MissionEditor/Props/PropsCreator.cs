using System;
using System.Collections;
using System.Collections.Generic;
using LitJson;
using Module;
using Sirenix.OdinInspector;
using UnityEngine;

// [Serializable]
// public struct RunLogicalFly
// {
//     [HideLabel, HorizontalGroup()] public RunLogicalName logicalName;
//     [HideLabel, HorizontalGroup()] public string fly;
// }


/// <summary>
/// 场景道具生成器，每一个道具对应一个生成器，生成器负责控制道具的初始化逻辑
/// </summary>
public partial class PropsCreator : SerializedMonoBehaviour, IMissionEditor
{
    private static string[] pathNames = new[] {LocalSave.savePath,LocalSave.save2Path};
    
    #region 字段、属性

    /// <summary>
    /// 全局的道具存储
    /// </summary>
    public static List<PropsCreator> editorList = new List<PropsCreator>();
    public static Action<IMissionEditor> onCreatorCreat;

    [LabelText("连接")]
    public List<PropsCreator> link;
    
    [LabelText("保存状态"), SerializeField, ReadOnly]
    private SaveStation saveStation;
    
    [HideInInspector] 
    public PropEntity entity;
    [LabelText("目标Prop"), ReadOnly] 
    public PropsBase props;

    private string lastLogical;
    private string[] lastLogicalArgs;
    private PropsStation lastStation;
    private string lastPropsWriteData;
    private int lastInterCount;

    //是否创建过了
    private bool _isCreated;

    public ProgressOption progressOption => extuil.progress;

    public bool progressIsComplete
    {
        get
        {
            if (BattleController.Instance.ctrlProcedure.currentNode.id == initNodeID)
            {
                if (_isCreated)
                {
                    if (props != null && (entity == null || entity.showProgress))
                    {
                        return !props.isProgressShow;
                    }
                    
                    return true;
                }
                
                return false;
            }
            return true;
        }
    }

    public Vector3 GetTipsPos()
    {
        if (props.progressTipsPoint != null)
        {
            return props.progressTipsPoint.position;
        }

        return transform.position;
    }

    public string localFileName => saveFileName;
    public string localGroup => "Props";
    public string localUid => "Props_" + id;
    
    private EventSender[] _sender;
    private EventReceiver[] _receiver;
    public EventSender[] sender => _sender;
    public EventReceiver[] receiver => _receiver;
    
    public int key => id;

    [Space]
    [OnValueChanged("OnModelChanged")] public int id;
    [LabelText("组")] public string group;
    
    
    [FoldoutGroup("提示相关")]
    [LabelText("交互提示"), ShowIf("@!isTrigger")]
    public string interactiveTips;

    [FoldoutGroup("提示相关")]
    [LabelText("UI提示"), ShowIf("@!isTrigger")]
    public string uiShowTips;

    [Space]
    [FoldoutGroup("模型相关")]
    [LabelText("是否是trigger")] [OnValueChanged("OnModelChanged"), ShowIf("@!staticLoad")]
    public bool isTrigger;

    [FoldoutGroup("模型相关")]
    [OnValueChanged("OnModelChanged"), ShowIf("@!isTrigger")]
    public PropsModelName model;

    // [FoldoutGroup("模型相关")]
    // [OnValueChanged("OnModelChanged"), ShowIf("@!isTrigger") , ValueDropdown("GetModelNames")]
    // public string modelName;
    //
    // [FoldoutGroup("模型相关"),ShowIf("@!isTrigger"),ReadOnly]
    // public string className;
    
    [FoldoutGroup("模型相关")]
    public bool creatHide;
    
    [FoldoutGroup("模型相关")]
    [ShowIf("@isTrigger")] 
    public Bounds bounds;
    
    [FoldoutGroup("模型相关")]
    [LabelText("静态加载"), OnValueChanged("OnLoadTypeChanged"), ShowIf("@!isTrigger")]
    public bool staticLoad;
    
    
    [Space]
    [FoldoutGroup("节点相关")] [LabelText("加载节点")] [ShowIf("@!staticLoad")]
    public int loadNodeID = 1;

    [FoldoutGroup("节点相关")] [LabelText("卸载节点")] public int unloadNodeID = -1;

    [FoldoutGroup("节点相关")] [LabelText("初始化节点")]
    public int initNodeID = 1;

    [FoldoutGroup("节点相关")] [LabelText("不销毁")] public bool unloadUnDestroy = true;
    
    [Space]
    [FoldoutGroup("初始化参数设置")] [HideLabel]
    public PropsInitLogical initLogical;

    [FoldoutGroup("初始化参数设置")] [LabelText("最大初始化次数")]
    public int initLocalCount = -1;

    [FoldoutGroup("初始化参数设置")] [LabelText("剩下交互次数")]
    public int remainInteractiveCount = 1;

    [FoldoutGroup("初始化参数设置")] [LabelText("数量")]
    public int count = 1;

    [Space] [LabelText("是否存档")] public bool save = true;
    [LabelText("存档名称"),ValueDropdown("pathNames")] public string saveFileName = LocalSave.savePath;

    [Space] [LabelText("匹配信息")] public string[] matchInfo;

    [LabelText("场景物品赋值"), OnValueChanged("OnSceneObjectChanged")]
    public GameObject sceneObjectLabel;

    [LabelText("场景物品")] public List<string> sceneObject;
    
    [Space]
    [FoldoutGroup("地图相关")]
    [HideLabel] public CreatorExtuil extuil;
    private bool _isGet;

    public bool isGet
    {
        get
        {
            if (props == null) return _isGet;
            return props.mapIsGet && _isGet;
        }
        set { _isGet = value; }
    }

    public MapType mapType => extuil.mapType;
    public string mapId => extuil.mapId;

    private const string ModelPath = "Props/Scene/{0}.prefab";
    private const string NormalTriggerPath = "Props/Scene/NormalTrigger.prefab";
    
    public bool canAddMap
    {
        get
        {
            if (entity != null)
            {
                return entity.canAddMap;
            }

            return true;
        }
    }

    #endregion
    
    public void InitEvent()
    {
        _sender = CommonTools.CreatSender(eventSenders, this);
        _receiver = CommonTools.CreatReceiver(eventRecivers, this);
    }

    #region MonoEvent

    private void Awake()
    {
        editorList.Add(this);
        lastLogical = initLogical.runLogical.ToString();
        lastLogicalArgs = initLogical.args;

        if (extuil.progress.index != -1)
        {
            BattleController.GetCtrl<ProgressCtrl>().allProgress.Add(this);
        }

        if (!isTrigger)
        {
            entity = PropEntity.entityList.Find(fd => fd.dbData.prefab == model.ToString());
        }
    }

    private void Start()
    {
        if (!@group.IsNullOrEmpty())
        {
            Group.GetGroup(group, this);
        }

        onCreatorCreat?.Invoke(this);
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

        if (editorList.Contains(this))
        {
            editorList.Remove(this);
        }
    }
    
    #endregion
    
    
    #region 初始化prop逻辑

     public async void OnNodeEnter(TaskNode node, Action<PropsBase> callback)
    {
        var tempStation = saveStation;
        saveStation = SaveStation.None;
        if (tempStation == SaveStation.None) //没有状态 正常初始化且不读存档
        {
            if (node.id == unloadNodeID)
            {
                if (props != null)
                {
                    props.RunLogicalOnSelf(RunLogicalName.ForceDestroy);
                    props = null;
                }
                callback?.Invoke(null);
                return;
            }

            bool isLoading = false;
            if (node.id == loadNodeID) //这一步加载模型
            {
                isLoading = true;
                LoadModel(() => { isLoading = false; });
            }

            await Async.WaitUntil(() => !isLoading); //等待模型加载完毕

            if (node.id == initNodeID) //是否需要初始化
            {
                InitPropNew(callback);
            }
            else
            {
                callback?.Invoke(props);
            }
        }
        else if (tempStation == SaveStation.Load || tempStation == SaveStation.Init) //有状态 读取存档
        {
            string data = LocalSave.Read(this);
            if (!data.IsNullOrEmpty())
            {
                PropSave save = JsonMapper.ToObject<PropSave>(data);
                _isGet = save.mapIsGet;
                if (save.initStation == (int) SaveStation.Unload) //卸载
                {
                    if (staticLoad)
                    {
                        props.DestroyThis(true);
                    }

                    callback?.Invoke(props);
                    return;
                }
                else if (save.initStation == (int) SaveStation.UnActive) //隐藏
                {
                    LoadModel(() =>
                    {
                        props.gameObject.OnActive(false);
                        callback?.Invoke(props);
                    });
                    return;
                }

                InitPropFromSave(save, callback);
            }
            else if (tempStation == SaveStation.Init) //读存档但是存档没有找到，那么也走普通初始化
            {
                InitPropNew(callback);
            }
            else //没有找到存档呢！应该是有问题的！
            {
                // GameDebug.LogError($"需要加载但是没有找到存档！ 状态：{tempStation} id:{id}");
                InitPropNew(callback);
            }
        }
        else if (tempStation == SaveStation.Unload)
        {
            UnloadProp(callback);
        }
    }

    public void InitPropNew(Action<PropsBase> callBack)
    {
        LoadModel(() =>
        {
            if (props != null)
            {
                props.Init(initLogical, entity, null);
                callBack?.Invoke(props);
            }
            else
            {
                GameDebug.LogError($"{id}物品未加载");
                callBack?.Invoke(null);
            }
        });
    }

    public void InitPropFromSave(PropSave save, Action<PropsBase> callBack)
    {
        LoadModel(() =>
        {
            props.Init(PropsInitLogical.GetInit(save), entity, save.propsData);
            props.interactiveCount = save.interactiveCount;
            callBack?.Invoke(props);
        });
    }

    public void UnloadProp(Action<PropsBase> callback)
    {
        if (staticLoad)
        {
            if (props == null)
            {
                GameDebug.LogError($"{id}物品prop为null，staticLoad：{staticLoad}");
            }
            else
            {
                props.RunLogicalOnSelf(RunLogicalName.ForceDestroy);
            }
        }

        callback?.Invoke(props);
    }

    #endregion
   


    public string GetWriteDate()
    {
        PropSave save = new PropSave();
        save.lastRunlogicalName = lastLogical;
        save.runlogicalArgs = lastLogicalArgs;
        save.mapIsGet = _isGet;
        if (!receiver.IsNullOrEmpty())
        {
            save.reciverCount = new string[receiver.Length];
            for (int i = 0; i < save.reciverCount.Length; i++)
            {
                save.reciverCount[i] = receiver[i].receiveCount.ToString();
            }
        }

        save.propsData = lastPropsWriteData;
        save.interactiveCount = lastInterCount;
        save.saveStation = (int) lastStation;
        return save.GetWriteStr(props);
    }

    public static string GetPrefabPath(string name)
    {
        return string.Format(ModelPath, name);
    }

    public void LoadModel(Action callback)
    {
        _isCreated = true;
        if (!staticLoad)
        {
            if (props != null)
            {
                callback?.Invoke();
                return;
            }
            if (!isTrigger)
            {
                AssetLoad.LoadGameObject<PropsBase>(GetPrefabPath(model.ToString()), transform, (resu, arg) =>
                {
                    props = resu;
                    props.creator = this;
                    if (creatHide) props.gameObject.OnActive(false);
                    SetActiveLayer();
                    callback?.Invoke();
                });
            }
            else
            {
                AssetLoad.LoadGameObject<NormalTrigger>(NormalTriggerPath, transform, (resu, arg) =>
                {
                    props = resu;
                    props.creator = this;
                    resu.collider.size = bounds.size;
                    resu.collider.center = bounds.center;
                    SetActiveLayer();
                    callback?.Invoke();
                });
            }
        }
        else
        {
            if (props != null)
            {
                SetActiveLayer();
            }
            else
            {
                GameDebug.LogError($"{id} 没有找到prop，检查静态加载相关！！！");
            }
            callback?.Invoke();
        }
    }

    private void SetActiveLayer()
    {
        int unVisableLayer = LayerMask.NameToLayer("UnVisiableProps");
        Transform[] transforms = props.transform.GetComponentsInChildren<Transform>(true);
        for (int i = 0; i < transforms.Length; i++)
        {
            if (transforms[i].gameObject.layer == unVisableLayer)
            {
                transforms[i].gameObject.layer = props.ActiveLayer;
            }
        }
    }


    public void OnEnterBattle(EnterNodeType type)
    {
        if (type == EnterNodeType.FromSave || saveFileName == LocalSave.save2Path)
        {
            if (staticLoad)
            {
                loadNodeID = 1;
            }
            saveStation = BattleController.Instance.ctrlProcedure.GetSaveStation(unloadNodeID, loadNodeID, initNodeID);
        }
    }

    public void Save()
    {
        if (lastLogical != initLogical.runLogical.ToString() && _isCreated)
        {
            LocalSave.Write(this);
        }
    }

    public void EventCallback(int eventID, IEventCallback receiver)
    {
    }

    public void RunLogical(RunLogicalName logical, IEventCallback sender, RunLogicalFlag flag, string senderArg,
        params string[] args)
    {
        if (CommonTools.CommonLogical(logical, args))
        {
            //如果是通用命令的话，直接截胡返回，不改变自身或者其他人的状态
            return;
        }

        if (props == null)
        {
            return;
        }

        if (logical == RunLogicalName.Reset) //重置事件单独处理
        {
            RunLogical(this.initLogical.runLogical, sender, flag, senderArg, this.initLogical.args);
            for (int i = 0; i < receiver.Length; i++)
            {
                receiver[i].ResetValue();
            }

            props.interactiveCount = 0;
            props.gameObject.OnActive(true);
            props.isInit = true;
        }
        else
        {
            //调用PropBase的Run
            props.RunLogical(logical, sender, flag, senderArg, args);
            if (save && (flag & RunLogicalFlag.Save) != 0)
            {
                StartCoroutine(ReadProps(logical, args));
            }
        }
    }

    private IEnumerator ReadProps(RunLogicalName logical, params string[] args)
    {
        yield return new WaitForEndOfFrame(); //这里等到帧尾是为了让RunLogical先走完，然后在存档。
        lastLogical = logical.ToString();
        lastLogicalArgs = args;
        if (props != null)
        {
            lastStation = props.station;
            lastPropsWriteData = props.writeData;
            lastInterCount = props.interactiveCount;
            BattleController.Instance.Save(0);
        }
    }

    public bool TryPredicate(SendEventCondition predicate, string[] sendArg, string[] predicateArgs)
    {
        switch (predicate)
        {
            case SendEventCondition.None:
            case SendEventCondition.Interactive:
                return true;
            case SendEventCondition.AddStation:
            case SendEventCondition.RemoveStation:
                sendArg.ToLower();
                predicateArgs.ToLower();
                return sendArg.IsSame(predicateArgs);
        }

        return false;
    }

    public void TrySendEvent(SendEventCondition predicate, params string[] arg)
    {
        if (!sender.IsNullOrEmpty())
        {
            for (int i = 0; i < sender.Length; i++)
            {
                if (sender[i].predicate == predicate)
                {
                    GameDebug.LogFormat("{0}尝试发送{1}的条件", id, predicate);
                    sender[i].TrySendEvent(arg);
                }
            }
        }
    }

    /// <summary>
    /// 得到连接的物体(后期如有需求可以扩展到怪物也可以连接)
    /// </summary>
    public T GetLinkedProp<T>(int id) where T : PropsBase
    {
        for (int i = 0; i < link.Count; i++)
        {
            if (link[i].id == id && link[i].props != null)
            {
                return (T)link[i].props;
            }
        }
        return null;
    }

    /// <summary>
    /// 得到唯一的连接
    /// </summary>
    public T GetUniqueLinkedProp<T>() where T : PropsBase
    {
        if (link.Count <= 0 || link[0].props == null)
        {
            return null;
        }
        return (T)link[0].props;
    }
    
    
}