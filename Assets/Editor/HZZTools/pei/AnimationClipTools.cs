using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector;
using System.Linq;
using System.Reflection;

public class AnimationClipTools : OdinEditorWindow
{
    static EditorWindow window;
    [MenuItem("Tools/裴亚龙专用/动画编辑工具")]
    public static void OpenWindow()
    {
        window = GetWindow<AnimationClipTools>("动画事件编辑器");
        window.Show();
    }
    [OnValueChanged("OnChangeBaseTrans")]
    public Transform baseTrans;
    [ReadOnly]
    public Vector3 basePos;
    [ReadOnly]
    public Vector3 baseAngle;
    public bool useCustom = false;
    [ShowIf("useCustom")]
    public Vector3 customAngle;
    public AnimationClip clip;
    [OnValueChanged("OnTargetChange")]
    public Transform targetTrans;
    [ReadOnly]
    public string targetName;
    //public bool pos =true;
    public bool angle = true;
    [FolderPath]
    public string clipPath;

    private void OnClipChange()
    {
        if (clip != null)
            clipPath = AssetDatabase.GetAssetPath(clip);
    }

    private void OnChangeBaseTrans()
    {
        if (baseTrans != null)
        {
            basePos = baseTrans.localPosition;
            baseAngle = GetInspectorRotationValueMethod(baseTrans);
        }
    }

    private void OnTargetChange()
    {
        targetName = targetTrans.name.ToLower();
    }

    private const int DecimalAccuracy = 10000;
    [Button("更改动画")]
    private void ChangeAnim()
    {
        var curveDatas = AnimationUtility.GetAllCurves(clip, true);

        var newClip = new AnimationClip();
        EditorUtility.CopySerialized(clip, newClip);
        newClip.name = clip.name;
        newClip.ClearCurves();
        //记录原始旋转值,初始坐标,初始旋转值
        Quaternion[] quaternions = null;
        Quaternion firstQuaternion = Quaternion.identity;
        Vector3 startAngle = Vector3.zero;
        Vector3 firstPos = Vector3.zero;
        foreach (var dt in curveDatas)
        {
            var nodeName = dt.path.ToLower().Split('/').Last();
            if (nodeName == targetName)
            {
                //初始位置
                if (dt.propertyName.ToLower().Contains("position"))
                {
                    var keys = dt.curve.keys;
                    if (dt.propertyName.ToLower().Contains("x"))
                    {
                        firstPos.x = keys[0].value;
                    }
                    if (dt.propertyName.ToLower().Contains("y"))
                    {
                        firstPos.y = keys[0].value;
                    }
                    if (dt.propertyName.ToLower().Contains("z"))
                    {
                        firstPos.z = keys[0].value;
                    }
                }
                //原始旋转,初始旋转
                if (dt.propertyName.ToLower().Contains("rotation")&& angle)
                {
                    if (quaternions == null)
                    {
                        quaternions = new Quaternion[dt.curve.keys.Length];
                    }
                    //Debug.Log(dt.propertyName);
                    var keys = dt.curve.keys;
                    //Debug.Log(keys.Length);
                    for (var i = 0; i < keys.Length; i++)
                    {
                        if (dt.propertyName.ToLower().Contains("x"))
                        {
                            quaternions[i].x = keys[i].value;
                            if (i ==0)
                            {
                                firstQuaternion.x = keys[i].value;
                            }
                        }
                        if (dt.propertyName.ToLower().Contains("y"))
                        {
                            quaternions[i].y = keys[i].value;
                            if (i == 0)
                            {
                                firstQuaternion.y = keys[i].value;
                            }
                        }
                        if (dt.propertyName.ToLower().Contains("z"))
                        {
                            quaternions[i].z = keys[i].value;
                            if (i == 0)
                            {
                                firstQuaternion.z = keys[i].value;
                            }
                        }
                        if (dt.propertyName.ToLower().Contains("w"))
                        {
                            quaternions[i].w = keys[i].value;
                            if (i == 0)
                            {
                                firstQuaternion.w = keys[i].value;
                            }
                        }
                    }
                }
            }
        }
        startAngle = firstQuaternion.eulerAngles;
        foreach (var dt in curveDatas)
        {
            if (dt.propertyName.ToLower().Contains("scale"))
            {
                continue;
            }
            var nodeName = dt.path.ToLower().Split('/').Last();
            var keys = dt.curve.keys;
            if (nodeName == targetName)
            {
                //对坐标进行偏移
                if (dt.propertyName.ToLower().Contains("position"))
                {
                    for (var i = 0; i < keys.Length; i++)
                    {
                        keys[i].time = Mathf.Round(keys[i].time * DecimalAccuracy) / DecimalAccuracy;
                        float baseValue = 0;
                        if (dt.propertyName.ToLower().Contains("x"))
                        {
                            baseValue =firstPos.x- basePos.x;
                        }
                        if (dt.propertyName.ToLower().Contains("y"))
                        {
                            baseValue = firstPos.y - basePos.y;
                        }
                        if (dt.propertyName.ToLower().Contains("z"))
                        {
                            baseValue = firstPos.z - basePos.z;
                        }
                        keys[i].value = Mathf.Round((keys[i].value - baseValue) * DecimalAccuracy) / DecimalAccuracy;
                        keys[i].outTangent = Mathf.Round(keys[i].outTangent * DecimalAccuracy) / DecimalAccuracy;
                        keys[i].inTangent = Mathf.Round(keys[i].inTangent * DecimalAccuracy) / DecimalAccuracy;
                    }
                }//对旋转进行偏移
                else if (dt.propertyName.ToLower().Contains("rotation")&& angle)
                {
                    for (var i = 0; i < keys.Length; i++)
                    {
                        keys[i].time = Mathf.Round(keys[i].time * DecimalAccuracy) / DecimalAccuracy;
                        float newValue = 0;
                        Vector3 oldAngle = quaternions[i].eulerAngles;
                        Debug.Log(oldAngle);
                        Vector3 newAngle = oldAngle- startAngle - (useCustom?customAngle:baseAngle);
                        Debug.Log("startAngle" + startAngle);
                        Debug.Log("newAngle1" + newAngle);
                        Quaternion newValueQ = Quaternion.Euler(newAngle);
                        Debug.Log("newValueQ" + newValueQ);
                        Debug.Log("newAngle2" + newValueQ.eulerAngles);
                        if (dt.propertyName.ToLower().Contains("x"))
                        {
                            newValue = newValueQ.x;
                        }
                        if (dt.propertyName.ToLower().Contains("y"))
                        {
                            newValue = newValueQ.y;
                        }
                        if (dt.propertyName.ToLower().Contains("z"))
                        {
                            newValue = newValueQ.z;
                        }
                        if (dt.propertyName.ToLower().Contains("w"))
                        {
                            newValue = newValueQ.z;
                        }
                        keys[i].value = Mathf.Round(newValue * DecimalAccuracy) / DecimalAccuracy;
                        keys[i].outTangent = Mathf.Round(keys[i].outTangent * DecimalAccuracy) / DecimalAccuracy;
                        keys[i].inTangent = Mathf.Round(keys[i].inTangent * DecimalAccuracy) / DecimalAccuracy;
                    }
                }
                else
                {
                    for (var i = 0; i < keys.Length; i++)
                    {
                        keys[i].time = Mathf.Round(keys[i].time * DecimalAccuracy) / DecimalAccuracy;
                        keys[i].value = Mathf.Round(keys[i].value * DecimalAccuracy) / DecimalAccuracy;
                        keys[i].outTangent = Mathf.Round(keys[i].outTangent * DecimalAccuracy) / DecimalAccuracy;
                        keys[i].inTangent = Mathf.Round(keys[i].inTangent * DecimalAccuracy) / DecimalAccuracy;
                    }
                }
            }
            else {
                for (var i = 0; i < keys.Length; i++)
                {
                    keys[i].time = Mathf.Round(keys[i].time * DecimalAccuracy) / DecimalAccuracy;
                    keys[i].value = Mathf.Round(keys[i].value * DecimalAccuracy) / DecimalAccuracy;
                    keys[i].outTangent = Mathf.Round(keys[i].outTangent * DecimalAccuracy) / DecimalAccuracy;
                    keys[i].inTangent = Mathf.Round(keys[i].inTangent * DecimalAccuracy) / DecimalAccuracy;
                }
            }

            dt.curve.keys = keys;
            //设置新数据
            newClip.SetCurve(dt.path, dt.type, dt.propertyName, dt.curve);
        }
        AssetDatabase.CreateAsset(newClip, clipPath + @"/" + newClip.name + ".anim");
        AssetDatabase.Refresh();
    }
    /// <summary>
    /// 动画默认不导出Scale序列帧，除非该节点包含scale关键词(加scale关键词表示该节点需要进行scale变换)
    /// </summary>
    /// <param name="dt"></param>
    /// <returns></returns>
    private static bool IsFilterCurveData(AnimationClipCurveData dt, string nodeName)
    {
        if (dt.propertyName.ToLower().Contains("scale") && !nodeName.Contains("scale"))
            return true;
        return false;
    }
    //获取到旋转的正确数值
    public Vector3 GetInspectorRotationValueMethod(Transform transform)
    {
        // 获取原生值
        System.Type transformType = transform.GetType();
        PropertyInfo m_propertyInfo_rotationOrder = transformType.GetProperty("rotationOrder", BindingFlags.Instance | BindingFlags.NonPublic);
        object m_OldRotationOrder = m_propertyInfo_rotationOrder.GetValue(transform, null);
        MethodInfo m_methodInfo_GetLocalEulerAngles = transformType.GetMethod("GetLocalEulerAngles", BindingFlags.Instance | BindingFlags.NonPublic);
        object value = m_methodInfo_GetLocalEulerAngles.Invoke(transform, new object[] { m_OldRotationOrder });
        string temp = value.ToString();
        //将字符串第一个和最后一个去掉
        temp = temp.Remove(0, 1);
        temp = temp.Remove(temp.Length - 1, 1);
        //用‘，’号分割
        string[] tempVector3;
        tempVector3 = temp.Split(',');
        //将分割好的数据传给Vector3
        Vector3 vector3 = new Vector3(float.Parse(tempVector3[0]), float.Parse(tempVector3[1]), float.Parse(tempVector3[2]));
        return vector3;
    }
}
