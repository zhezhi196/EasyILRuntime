using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
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
[ExecuteInEditMode]
public partial class PropsCreator : SerializedMonoBehaviour, IMissionEditor
{
    private static string[] pathNames = new[] {LocalSave.savePath,LocalSave.save2Path};
    private SaveStation _saveStation;
    #region 字段、属性

    /// <summary>
    /// 全局的道具存储
    /// </summary>
    public static List<PropsCreator> editorList = new List<PropsCreator>();
    public static Action<IMissionEditor> onCreatorCreat;

    [LabelText("连接")]
    public List<PropsCreator> link;

    [LabelText("保存状态"), SerializeField, ReadOnly]
    public SaveStation saveStation
    {
        get
        {
            if (props == null) return SaveStation.Unload;
            if (!props.gameObject.activeInHierarchy) return SaveStation.UnActive;
            return _saveStation;
        }
        set
        {
            _saveStation = value;
        }
    }
    
    
    [HideInInspector] 
    public PropEntity entity;
    [LabelText("目标Prop"), ReadOnly] 
    public PropsBase props;

    private PropSave propSave;

    [HideInInspector]
    public string lastLogical;
    private string[] lastLogicalArgs;
    private PropsStation lastStation;
    private string lastPropsWriteData;
    private int lastInterCount;

    //是否创建过了
    private bool _isCreated;

    // public ProgressOption progressOption => extuil.progress;

    public bool progressIsComplete
    {
        get
        {
            //如果道具状态是卸载或者不激活的话，就返回成功
            if (saveStation != SaveStation.Init && saveStation != SaveStation.None)
            {
                return true;
            }
            
            //如果已经初始化了，那么在判断propbase的状态
            if (saveStation == SaveStation.Init && _isCreated)
            {
                return props.progressIsComplete;
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
    [ReadOnly]public int idBackUp;
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
    public int mapId => extuil.mapId;

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
        idBackUp = id;

        if (!Application.isPlaying)
        {
            return;
        }
        
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
        if (!Application.isPlaying)
        {
            return;
        }
        
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
    /// <summary>
    /// 节点进入，初始化物品。分为三种情况，新游戏进入无存档，继续游戏有存档，游戏内切节点有存档
    /// </summary>
    /// <param name="node"></param>
    /// <param name="callback"></param>
    /// <param name="enterType"></param>
    public async void OnNodeEnter(TaskNode node, Action<PropsBase> callback , EnterNodeType enterType)
    {
        if (id == 1)
        {
            
        }
        if (enterType == EnterNodeType.Restart || saveStation == SaveStation.None) //没有状态 正常初始化且不读存档
        {
            if (node.id == unloadNodeID)
            {
                _saveStation = SaveStation.Unload;
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
                _saveStation = SaveStation.Load;
                LoadModel(() => { isLoading = false; });
            }

            await Async.WaitUntil(() => !isLoading); //等待模型加载完毕

            if (node.id == initNodeID) //是否需要初始化
            {
                _saveStation = SaveStation.Init;
                InitPropNew(callback);
            }
            else
            {
                callback?.Invoke(props);
            }
        }
        else
        {
            if (enterType == EnterNodeType.FromSave)
            {
                if (propSave != null)
                {
                    _isGet = propSave.mapIsGet;
                    InitPropFromSave(propSave, callback);
                }
                else
                {
                    GameDebug.LogError($"没有存档且SaveStation不是none : {id}");
                    callback?.Invoke(props);
                }
            }
            else
            {
                //游戏内切节点，SaveStation处理一下
                if (node.id == unloadNodeID)
                {
                    _saveStation = SaveStation.Unload;
                }
                else if (node.id == initNodeID)
                {
                    _saveStation = SaveStation.Init;
                }
                else if (node.id == loadNodeID)
                {
                    _saveStation = SaveStation.Load;
                }
                else //如果都不是，维持上一个节点的样子，不用动
                {
                    callback?.Invoke(props);
                    return;
                }

                //如果状态发生了变化，就根据新的状态初始化当前道具
                if (_saveStation == SaveStation.Load)
                {
                    LoadModel(() =>
                    {
                        callback?.Invoke(props);
                    });
                }
                else if (_saveStation == SaveStation.Init)
                {
                    InitPropNew(callback);
                }
                else if (_saveStation == SaveStation.Unload)
                {
                    UnloadProp(callback);
                }
                else if(_saveStation == SaveStation.UnActive)
                {
                    LoadModel(() =>
                    {
                        props.gameObject.OnActive(false);
                        callback?.Invoke(props);
                    });
                    return;
                }
            }
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
            //有可能props在Init的时候forceDestroy了
            if (props != null)
            {
                props.interactiveCount = save.interactiveCount;
            }
            callBack?.Invoke(props);
        });
    }

    public void UnloadProp(Action<PropsBase> callback)
    {
        if (staticLoad)
        {
            if (props != null)
            {
                props.RunLogicalOnSelf(RunLogicalName.ForceDestroy);
                // GameDebug.LogError($"{id}物品prop为null，staticLoad：{staticLoad}");
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
        return save.GetWriteStr(this);
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
            if (props != null) //已经加载的了，返回
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
            
            string data = LocalSave.Read(this);
            if (data.IsNullOrEmpty())
            {
                saveStation = SaveStation.None; //没有存档，就没有存档状态
                propSave = null;
            }
            else
            {
                propSave = JsonMapper.ToObject<PropSave>(data); //有的话读取存档状态
                saveStation = (SaveStation)propSave.initStation;
            }
            // saveStation = BattleController.Instance.ctrlProcedure.GetSaveStation(unloadNodeID, loadNodeID, initNodeID);
        }
    }

    public void Save()
    {
        if (!BattleController.GetCtrl<PropsCtrl>().IsLoadingNode())
        {
            LocalSave.Write(this);    
        }
        
        // if (lastLogical != initLogical.runLogical.ToString() && _isCreated)
        // {
        //     LocalSave.Write(this);
        // }
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

            GameDebug.Log($"{id}收到了重置的消息，重置了部分属性");
            props.interactiveCount = 0;
            props.gameObject.OnActive(true);
            props.isInit = true;
        }
        else
        {
            //调用PropBase的Run
            props.RunLogical(logical, sender, flag, senderArg, args);
            //记录状态
            FlushData(logical, args);
            if (save && (flag & RunLogicalFlag.Save) != 0)
            {
                BattleController.Instance.Save(0);
                // ReadProps(logical, args);
                // StartCoroutine(ReadProps(logical, args)); 
            }
            // else
            // {
            //
            //     // StartCoroutine(FlushDataCO(logical, args)); //道具每次状态改变都要flush
            // }
        }
    }

    private void FlushData(RunLogicalName logical, params string[] args)
    {
        lastLogical = logical.ToString();
        lastLogicalArgs = args;
        if (props != null)
        {
            lastStation = props.station;
            lastPropsWriteData = props.writeData;
            lastInterCount = props.interactiveCount;
        }
    }
    //
    // private IEnumerator FlushDataCO(RunLogicalName logical, params string[] args)
    // {
    //     yield return new WaitForEndOfFrame(); //这里等到帧尾是为了让RunLogical先走完，然后在存档。
    //     lastLogical = logical.ToString();
    //     lastLogicalArgs = args;
    //     if (props != null)
    //     {
    //         lastStation = props.station;
    //         lastPropsWriteData = props.writeData;
    //         lastInterCount = props.interactiveCount;
    //     }
    // }

    // private void ReadProps(RunLogicalName logical, params string[] args)
    // {
    //     // yield return FlushDataCO(logical , args);
    //     BattleController.Instance.Save(0);
    // }

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


    public SaveStation GetTrueSaveStation()
    {
        return _saveStation;
    }
    
    
}