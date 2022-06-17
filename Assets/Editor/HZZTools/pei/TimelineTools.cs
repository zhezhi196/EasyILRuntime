using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.Events;
using UnityEditor.Events;

public class CreatTimelineAsset : OdinEditorWindow
{
    static CreatTimelineAsset window;
    [MenuItem("Tools/裴亚龙专用/Timeline编辑工具")]
    public static void OpenWindow()
    {
        window = GetWindow<CreatTimelineAsset>("创建Timeline资源");
        window.Show();
    }
    private const string assetPath = "Assets/Bundles/Timeline/";
    [LabelText("资源名")]
    public string assetName;
    [LabelText("角色模型")]
    public GameObject playerModel;
    [LabelText("资源类型")]
    public TimeLineType type;
    [ HideIf("type",TimeLineType.Story),LabelText("怪物类型")]
    public GameObject monsterModel;
    [ShowIf("type",TimeLineType.Story)]
    public string key;
    [Button("创建资源")]
    private void Create()
    {
        if (string.IsNullOrEmpty(assetName))
        {
            if (EditorUtility.DisplayDialog("警告", "资源名为空", "确定"))
            { 
            
            }
            return;
        }
        GameObject root = new GameObject(assetName);//根节点
        GameObject playerPoint = new GameObject("PlayerPoint");//玩家点
        playerPoint.transform.SetParent(root.transform);
        PlayableDirector playable =  root.AddComponent<PlayableDirector>();
        TimelineController timeline;//timeline组件
        //创建asset
        TimelineAsset asset = CreateInstance<TimelineAsset>();
        asset.name = assetName + "Asset";
        AssetDatabase.CreateAsset(asset, string.Format(assetPath + "{0}.playable", asset.name));
        if (type != TimeLineType.Story)//暗杀处决挣脱timeline
        {
            timeline = root.AddComponent<AssExcuteTimeline>();
            asset.CreateTrack(typeof(AnimationTrack), null, "PlayerAnim");
            asset.CreateTrack(typeof(AnimationTrack), null, "MonsterAnim");
            SetPlayerPoint(root, playerPoint);
        }
        else {
            timeline = root.AddComponent<StoryTimeline>();
        }
        asset.CreateTrack(typeof(AudioTrack), null, "Audio");
        //绑定timeline资源
        timeline.playerPoint = playerPoint.transform;
        timeline.playable = playable;
        timeline.type = type;
        playable.playableAsset = asset;
        //Signal,添加OnComplete回调
        asset.CreateMarkerTrack();
        TrackAsset track = asset.markerTrack;//创建markerTrack
        SignalEmitter signalEmitter = track.CreateMarker<SignalEmitter>(1f);//添加信号发射器
        SignalReceiver signal = root.AddComponent<SignalReceiver>();//添加信号接收器
        SignalAsset signalAsset = AssetDatabase.LoadAssetAtPath(assetPath+ "TimelineSignal/OnComplete.signal", typeof(SignalAsset)) as SignalAsset;//加载信号资源
        UnityEvent callBack = new UnityEvent();
        UnityEventTools.AddPersistentListener(callBack, timeline.OnComplete);//创建回调Event
        signal.AddReaction(signalAsset, callBack);//信号接收器绑定事件
        signalEmitter.asset = signalAsset;//发射器绑定事件
        //保存资源
        string prefabPath = string.Format(assetPath + "{0}.prefab", root.name);
        PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
        GameObject.DestroyImmediate(root);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        //获取预制体到场景
        LoadPrefabToScene(prefabPath);
        assetName = "";
    }
    //设置PlayerPoint
    private void SetPlayerPoint(GameObject root,GameObject playerPoint)
    {
        //怪物杀死玩家，柜子抓住玩家，找玩家处决点
        if (type == TimeLineType.KillPlayer || type == TimeLineType.GetOut)
        {
            if (playerModel != null)
            {
                playerModel.transform.position = Vector3.zero;
                playerModel.transform.eulerAngles = Vector3.zero;
                root.transform.position = Vector3.zero;
                root.transform.eulerAngles = Vector3.zero;
                Transform[] trans = playerModel.GetComponentsInChildren<Transform>();
                if (trans.Length > 0)
                {
                    for (int i = 0; i < trans.Length; i++)
                    {
                        if (trans[i].name == "chu_jue")
                        {
                            playerPoint.transform.position = trans[i].position;
                            playerPoint.transform.localEulerAngles = new Vector3(0, 180, 0);
                            break;
                        }
                    }
                }
            }
        }
        //怪物暗杀处决动画
        else
        {
            if (monsterModel != null)
            {
                root.transform.position = monsterModel.transform.position;
                root.transform.rotation = monsterModel.transform.rotation;
                if (type == TimeLineType.Ass)//暗杀动画找怪物暗杀点
                {
                    Transform[] trans = monsterModel.GetComponentsInChildren<Transform>();
                    if (trans.Length > 0)
                    {
                        for (int i = 0; i < trans.Length; i++)
                        {
                            if (trans[i].name == "ansha" || trans[i].name == "an_sha")
                            {
                                playerPoint.transform.position = trans[i].position;
                                break;
                            }
                        }
                    }
                }
                else//处决，找怪物处决点
                {
                    Transform[] trans = monsterModel.GetComponentsInChildren<Transform>();
                    if (trans.Length > 0)
                    {
                        for (int i = 0; i < trans.Length; i++)
                        {
                            if (trans[i].name == "chujue" || trans[i].name == "chu_jue")
                            {
                                playerPoint.transform.position = trans[i].position;
                                playerPoint.transform.localEulerAngles = new Vector3(0, 180, 0);
                                break;
                            }
                        }
                    }
                }
            }
        }
    }
    //加载创建的预制体到场景中
    private void LoadPrefabToScene(string path)
    {
        GameObject Target = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
        GameObject prefabAsset = PrefabUtility.InstantiatePrefab(Target) as GameObject;
        prefabAsset.transform.SetAsLastSibling();
        PlayableDirector prefabPlayable = prefabAsset.GetComponent<PlayableDirector>();
        TimelineController controller = prefabAsset.GetComponent<TimelineController>();
        if (monsterModel != null)
        {
            if (type == TimeLineType.KillPlayer || type == TimeLineType.GetOut)
            {
                monsterModel.transform.position = controller.playerPoint.transform.position;
                monsterModel.transform.rotation = controller.playerPoint.transform.rotation;
            }
            prefabPlayable.SetGenericBinding((prefabPlayable.playableAsset as TimelineAsset).GetOutputTrack(2), monsterModel.GetComponent<Animator>());
        }
        if (playerModel != null)
        {
            if (type != TimeLineType.KillPlayer && type != TimeLineType.GetOut)
            {
                playerModel.transform.position = controller.playerPoint.transform.position;
                playerModel.transform.rotation = controller.playerPoint.transform.rotation;
            }
            prefabPlayable.SetGenericBinding((prefabPlayable.playableAsset as TimelineAsset).GetOutputTrack(1), playerModel.GetComponent<Animator>());
        }
        Selection.activeGameObject = prefabAsset;
    }
}
