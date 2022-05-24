using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;
using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector;
using UnityEditor.Animations;

/// <summary>
/// 动画工具
/// </summary>
public class AnimationEventTools : Editor
{
    [MenuItem("Assets/AnimToos/清除动画事件")]
    static void ClearSelectClipEvent()
    {
        if (Selection.objects.Length > 0)
        {
            for (int k = 0; k < Selection.objects.Length; k++)
            {
                AnimationClip clip = Selection.objects[k] as AnimationClip;
                string clipName = "";
                if (clip != null)
                {
                    clipName = clip.name;
                }
                //找到当前的animationClip的路径
                string assetPath = AssetDatabase.GetAssetPath(Selection.objects[k]);
                ClearEvent(assetPath, clipName);
            }
        }
    }

    private static void ClearEvent(string assetPath,string clipName)
    {
        ModelImporter modelImporter = AssetImporter.GetAtPath(assetPath) as ModelImporter;
        //读取文件序列化数据 实际就是meta里的数据
        SerializedObject serializedObject = new SerializedObject(modelImporter);
        //找到所有animation
        SerializedProperty clipAnimations = serializedObject.FindProperty("m_ClipAnimations");
        if (clipAnimations == null || clipAnimations.arraySize == 0)
        {
            modelImporter.clipAnimations = modelImporter.defaultClipAnimations;
            serializedObject = new SerializedObject(modelImporter);
            clipAnimations = serializedObject.FindProperty("m_ClipAnimations");
        }
        for (int i = 0; i < clipAnimations.arraySize; i++)
        {
            SerializedProperty clipAnimationProperty = clipAnimations.GetArrayElementAtIndex(i);
            if (string.IsNullOrEmpty(clipName) || clipAnimationProperty.displayName == clipName)
            {
                //找到其events
                SerializedProperty eventsProperty = clipAnimationProperty.FindPropertyRelative("events");
                if (eventsProperty.arraySize <= 0)
                {
                    Debug.LogFormat("动画{0}的Events为空", clipAnimationProperty.displayName);
                    continue;
                }
                //清空事件
                eventsProperty.ClearArray();
                //应用
                serializedObject.ApplyModifiedProperties();
                //重新读取
                AssetDatabase.ImportAsset(assetPath);
                Debug.LogFormat("已清除动画{0}的Events", clipAnimationProperty.displayName);
            }
        }
        //最终刷新一下AssetDatabase
        AssetDatabase.Refresh();
    }

    public static void PlayAudioClip()
    {
        if (Selection.objects.Length <= 0)
        {
            Debug.Log("未选择任何资源");
            return;
        }
        AudioClip clip = Selection.objects[0] as AudioClip;
        if (clip == null)
        {
            Debug.Log("请选择AudioClip");
            return;
        }
        
        Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;

        Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
        MethodInfo method = audioUtilClass.GetMethod(
            "PlayClip",
            BindingFlags.Static | BindingFlags.Public,
            null,
            new Type[] { typeof(AudioClip), typeof(int), typeof(bool) },
            null
        );

        Debug.Log(method);
        method.Invoke(
            null,
            new object[] { clip, 0, false }
        );
    }
}
/// <summary>
/// 动画事件编辑窗口
/// </summary>
public class AnimtionEvnentEditoWindow : OdinEditorWindow
{
    static EditorWindow window;
    [MenuItem("Tools/裴亚龙专用/动画事件编辑器")]
    public static void OpenWindow()
    {
        window = GetWindow<AnimtionEvnentEditoWindow>("动画事件编辑器");
        window.Show();
    }
    public GameObject previewModel;
    //---------------左侧Group----------------
    private Animator previewAnim;
    [HorizontalGroup("Group", 0.3f)]
    [Button("刷新")]
    [BoxGroup("Group/动画"), GUIColor(0.565f, 0.933f, 0.565f)]
    private void Refesh()
    {
        if (previewModel != null)
        {
            previewAnim = previewModel.GetComponent<Animator>();
            animationClips = previewAnim.runtimeAnimatorController.animationClips;
            amsDic.Clear();
            stateNames = new List<string>();
            AnimatorStateMachine stateMachine = ((AnimatorController)previewAnim.runtimeAnimatorController).layers[0].stateMachine;
            GetAllState(stateMachine);
        }
        else
        {
            animationClips = null;
            stateNames.Clear();
            previewState = "";
            previewClip = null;
            Debug.LogError("未指定Animator");
        }
        OnDisable();
        OnEnable();
    }
    private void GetAllState(AnimatorStateMachine stateMachine)
    {
        for (int i = 0; i < stateMachine.states.Length; i++)
        {
            if (stateMachine.states[i].state.motion is AnimationClip)
            {
                if (amsDic.ContainsKey(stateMachine.states[i].state.name))
                {
                    Debug.Log("重名state:" + stateMachine.states[i].state.name);
                }
                else
                {
                    amsDic.Add(stateMachine.states[i].state.name, stateMachine.states[i].state);
                    stateNames.Add(stateMachine.states[i].state.name);
                }
            }
        }
        for (int i = 0; i < stateMachine.stateMachines.Length; i++)
        {
            GetAllState(stateMachine.stateMachines[i].stateMachine);
        }
    }

    [Title("可编辑动画", "@stateNames.Count", Bold = false)]
    [BoxGroup("Group/动画")]
    [ValueDropdown("stateNames"), HideLabel, OnValueChanged("RefeshClipName")]
    public string previewState;//预览状态
    private void RefeshClipName()
    {
        if (!string.IsNullOrEmpty(previewState))
        {
            if (amsDic.Count <= 0)
            {
                Refesh();
                if (amsDic.Count <= 0)
                {
                    Debug.LogError("没有找到选择的动画");
                    return;
                }
            }
            previewClipName = amsDic[previewState].motion.name;
            for (int i = 0; i < animationClips.Length; i++)
            {
                if (animationClips[i].name == previewClipName)
                {
                    previewClip = animationClips[i];
                    break;
                }
            }
            value = 0;
        }
    }
    [BoxGroup("Group/动画")]
    [Title("所有动画列表", "@animationClips.Length", Bold = false), ReadOnly]
    public AnimationClip[] animationClips;//Controller所有动画
    private List<string> stateNames = new List<string>();//可预览state的名字列表
    private Dictionary<string, AnimatorState> amsDic = new Dictionary<string, AnimatorState>();//Motion是AnimationClip的State

    //--------------右侧Group预览区------------------
    [HorizontalGroup("Group/Editor")]
    [VerticalGroup("Group/Editor/Event")]
    [BoxGroup("Group/Editor/Event/动画预览", CenterLabel = true)]
    [HideLabel, ReadOnly, Title("预览动画名", Bold = false, HorizontalLine = false), GUIColor(1, 1, 0)]
    public string previewClipName = "";//预览状态动画名称
    private AnimationClip previewClip;//预览动画
    [BoxGroup("Group/Editor/Event/动画预览")]
    [Button("预览"), DisableIf("isPreview")]
    private void PreviewClip()
    {
        if (!string.IsNullOrEmpty(previewState))
        {
            if (Application.isPlaying)
            {
                Debug.Log("游戏内预览");
                previewAnim.Play(previewState, 0);
            }
            else
            {
                value = 0;
                startPreviewTime = EditorApplication.timeSinceStartup;
                animEvents = new List<AnimationEvent>(previewClip.events);
                isPreview = true;
                Debug.Log("游戏外预览");
            }
            Debug.Log("预览动画:" + previewState);
        }
        else
        {
            Debug.LogError("预览动画:未选择预览动画");
        }
    }
    bool isPreview = false;
    double startPreviewTime = 0;
    List<AnimationEvent> animEvents;
    [BoxGroup("Group/Editor/Event/动画预览")]
    [Range(0, 1), HideLabel, OnValueChanged("UpdateAnimtion"), DisableIf("isPreview")]
    public float value = 0;
    private void UpdateAnimtion()
    {
        if (previewClip != null)
            previewClip.SampleAnimation(previewModel, value * previewClip.length);
    }
    //------------------右侧Group编辑区---------------
    [BoxGroup("Group/Editor/Event/事件编辑", CenterLabel = true)]
    [HideLabel, ReadOnly, Title("当前编辑动画名", Bold = false, HorizontalLine = false), GUIColor(1, 1, 0)]
    public string editorClipName = "";//编辑状态动画名称
    [Title("事件列表", Bold = false)]
    [BoxGroup("Group/Editor/Event/事件编辑")]
    [TableList, LabelText("动画事件")]
    public List<EventData> eventDatas = new List<EventData>();
    [TitleGroup("Group/Editor/Event/事件编辑/更新", BoldTitle = false)]
    [ButtonGroup("Group/Editor/Event/事件编辑/更新/Buttons"), LabelText("获取预览动画事件")]
    public void UpdateEventFromClip()
    {
        if (previewClip == null)
        {
            Debug.LogError("未选择预览动画");
            return;
        }
        editorClipName = previewClip.name;
        eventDatas = new List<EventData>();
        for (int i = 0; i < previewClip.events.Length; i++)
        {
            EventData data = new EventData();
            data.eventName = previewClip.events[i].functionName;
            data.eventTime = previewClip.events[i].time / previewClip.length;
            data.floatArg = previewClip.events[i].floatParameter;
            data.intArg = previewClip.events[i].intParameter;
            data.strArg = previewClip.events[i].stringParameter;
            eventDatas.Add(data);
        }
    }
    [ButtonGroup("Group/Editor/Event/事件编辑/更新/Buttons"), LabelText("更新事件到Clip")]
    public void UpdateEvents2Clip()
    {
        SetAnimationEvent();
    }
    [ButtonGroup("Group/Editor/Event/事件编辑/更新/Buttons"), LabelText("丢弃修改"), GUIColor(0.941f, 0.502f, 0.502f)]
    public void DiscardChanges()
    {
        eventDatas = new List<EventData>();
        UpdateEventFromClip();
    }
    
    public class EventData
    {
        public bool editor = false;
        [EnableIf("editor"),ValueDropdown("eventNames")]
        public string eventName;
        [EnableIf("editor")]
        public float eventTime;
        [EnableIf("editor")]
        public string strArg;
        [EnableIf("editor")]
        public int intArg;
        [EnableIf("editor")]
        public float floatArg;
        private static List<string> eventNames = new List<string>() { "Reload", "TakeBack", "TakeOut", "Attack", "ToAim", "ToNoAim", "PlayAudio", "SkillEvent"};
    }

    Rect rect = new Rect(0, 0, 500, 50);
    private void DrawfraemLine()
    {
        rect = new Rect(window.position.width * 0.3f, 50, window.position.width * 0.7f, 50);
        Handles.BeginGUI();
        Handles.color = new Color(0, 1, 0, 0.4f);
        float step = 8;
        int Index = 0;
        for (float i = rect.x + 2; i < rect.width; i += step)
        {
            if (Index % 5 == 0)
            {
                Handles.DrawLine(new Vector3(i, rect.y + rect.height - 20), new Vector3(i, rect.y + rect.height - 5));
                string str = Index.ToString();
                if (str.Length > 2)
                {
                    GUI.Label(new Rect(i - 15, rect.y + 12, 30, 12), str);
                }
                else if (str.Length > 1)
                {
                    GUI.Label(new Rect(i - 10, rect.y + 12, 20, 12), str);
                }
                else
                {
                    GUI.Label(new Rect(i - 5, rect.y + 12, 12, 12), str);
                }

            }
            else
            {
                Handles.DrawLine(new Vector3(i, rect.y + rect.height - 15), new Vector3(i, rect.y + rect.height - 10));
            }
            Index++;

        }

        Handles.EndGUI();
    }

    bool isAddUpdate = false;
    protected override void OnEnable()
    {
        base.OnEnable();
        if (!isAddUpdate)
        {
            isAddUpdate = true;
            EditorApplication.update += MUpdate;
            Debug.Log("AddUpdate");
        }
    }

    private void OnDisable()
    {
        if (isAddUpdate)
        {
            isAddUpdate = false;
            EditorApplication.update -= MUpdate;
            Debug.Log("RemoveUpdate");
        }
    }

    private void MUpdate()
    {
        if (isPreview)//editor预览动画
        {
            float t = (float)(EditorApplication.timeSinceStartup - startPreviewTime);
            if (t <= previewClip.length)
            {
                t = Mathf.Clamp(t,0, previewClip.length);
                previewClip.SampleAnimation(previewModel, t);
                for (int i = animEvents.Count-1; i >=0; i--)
                {
                    if (animEvents[i].time < t)
                    {
                        Debug.Log("触发动画事件:" + animEvents[i].functionName);
                        animEvents.Remove(animEvents[i]);
                    }
                }
            }
            else {
                value = 1;
                UpdateAnimtion();
                isPreview = false;
            }
        }
    }
    //更新动画Events到meta文件
    private void SetAnimationEvent()
    {
        string assetPath = AssetDatabase.GetAssetPath(previewClip);
        ModelImporter modelImporter = AssetImporter.GetAtPath(assetPath) as ModelImporter;
        //读取文件序列化数据 实际就是meta里的数据
        SerializedObject serializedObject = new SerializedObject(modelImporter);
        //找到所有animation
        SerializedProperty clipAnimations = serializedObject.FindProperty("m_ClipAnimations");
        if (clipAnimations == null || clipAnimations.arraySize == 0)
        {
            modelImporter.clipAnimations = modelImporter.defaultClipAnimations;
            serializedObject = new SerializedObject(modelImporter);
            clipAnimations = serializedObject.FindProperty("m_ClipAnimations");
        }
        for (int i = 0; i < clipAnimations.arraySize; i++)
        {
            SerializedProperty clipAnimationProperty = clipAnimations.GetArrayElementAtIndex(i);
            if (clipAnimationProperty.displayName == editorClipName)
            {
                //找到其events
                SerializedProperty eventsProperty = clipAnimationProperty.FindPropertyRelative("events");
                //清空事件
                eventsProperty.ClearArray();
                //重新写入
                for (int j = 0; j < eventDatas.Count; j++)
                {
                    eventsProperty.InsertArrayElementAtIndex(eventsProperty.arraySize);
                    SerializedProperty eventProperty = eventsProperty.GetArrayElementAtIndex(j);
                    //数据中的时间是相对于总时间的0到1的小数 而不是以秒为单位的时间 所以要转换一下
                    eventProperty.FindPropertyRelative("time").floatValue = eventDatas[j].eventTime;
                    eventProperty.FindPropertyRelative("functionName").stringValue = eventDatas[j].eventName;
                    eventProperty.FindPropertyRelative("floatParameter").floatValue = eventDatas[j].floatArg;
                    eventProperty.FindPropertyRelative("intParameter").intValue = eventDatas[j].intArg;
                    eventProperty.FindPropertyRelative("data").stringValue = eventDatas[j].strArg;
                }
                //应用
                serializedObject.ApplyModifiedProperties();
                //重新读取
                AssetDatabase.ImportAsset(assetPath);
            }
        }
        //最终刷新一下AssetDatabase
        AssetDatabase.Refresh();
        Debug.LogFormat("已更新动画{0}的Events", editorClipName);
    }
}
