using Module;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teaching
{
    public enum TeachState
    {
        None,
        Ready,
        Teaching,
        Complete,
    }
    public TeachingName teachingName;
    public RunTimeSequence sequence = new RunTimeSequence();//教学步骤队列
    public TeachState teachState = TeachState.None;//教学状态
    protected int teachingIndex = 0;//当前执行的教学步骤序号
    public string path = "UI/GameTeaching/{0}.prefab";//教学文本预制路径
    protected Transform labelPre;//教学文本

    public Teaching(TeachingName tName)
    {
        teachingName = tName;
    }
    /// <summary>
    /// 初始化教学
    /// </summary>
    public virtual void InitTeaching()
    {
        var saveService = DataMgr.Instance.GetSqlService<TeachingSaveData>();
        TeachingSaveData saveData = saveService.Where(ds => ds.teachingName == teachingName.ToString());
        if (saveData != null && saveData.station == 1)
        {
            teachState = TeachState.Complete;
        }
        else{
            AssetLoad.LoadGameObject(string.Format(path, teachingName.ToString()), TeachingMaskUI.Instance.transform, (obj, arg) =>
            {
                if (obj == null)
                {
                    GameDebug.LogErrorFormat("未找到教学提示框:{0}",teachingName.ToString());
                }
                else {
                    labelPre = obj.GetComponent<Transform>();
                    obj.OnActive(false);
                }
            });
        }
        sequence.OnComplete(EndTeaching);
    }
    /// <summary>
    /// 设置教学高亮对象渲染层级
    /// </summary>
    /// <param name="o">教学高亮对象</param>
    public void AddTeachCanvas(GameObject o)
    {
        Canvas c = o.AddComponent<Canvas>();
        c.overrideSorting = true;
        c.sortingOrder = 101;
    }
    /// <summary>
    /// 移除教学对象高亮
    /// </summary>
    /// <param name="o">教学高亮对象</param>
    public void RemoveTeachCanvas(GameObject o)
    {
        GameObject.Destroy(o.GetComponent<Canvas>());
    }

    private Transform partTemp;
    private int childIndex = 0;
    /// <summary>
    /// 设置教学对象到指定父物体
    /// </summary>
    /// <param name="obj">教学对象</param>
    /// <param name="parent">新的父物体</param>
    public void SetTeachParent(GameObject obj,Transform parent)
    {
        partTemp = obj.transform.parent;
        childIndex = obj.transform.GetSiblingIndex();
        obj.transform.SetParent(parent);
        obj.transform.SetSiblingIndex(0);
    }
    /// <summary>
    /// 设置教学对象回到原位置
    /// </summary>
    /// <param name="obj">教学对象</param>
    public void ReturnTeachParent(GameObject obj)
    {
        obj.transform.SetParent(partTemp);
        obj.transform.SetSiblingIndex(childIndex);
    }

    public void SetMaskParent(Transform trans,Transform parent)
    { 
        int index = trans.GetSiblingIndex();
        TeachingMaskUI.Instance.transform.SetParent(parent);
        TeachingMaskUI.Instance.transform.SetSiblingIndex(index);
    }

    public void ReturnMaskParent()
    {
        TeachingMaskUI.Instance.transform.SetParent(TeachingMaskUI.Instance.maskParent);
        TeachingMaskUI.Instance.transform.SetSiblingIndex(TeachingMaskUI.Instance.maskChildIndex);
    }

    /// <summary>
    /// 开始教学
    /// </summary>
    public virtual void StartTeaching(bool pause = true)
    {
        GameDebug.LogFormat("GameTeaching: {0} Ready", teachingName);
        teachState = TeachState.Ready;
        UIController.Instance.canPhysiceback = false;
        if (pause)
            BattleController.Instance.Pause(teachingName.ToString());
        sequence.NextAction();
    }
    /// <summary>
    /// 继续下一步教学
    /// </summary>
    /// <param name="name">当前完成步骤名</param>
    public void NextTeachingStep(string name)
    {
        GameDebug.LogFormat("GameTeachingStep:{0} {1} Complete", teachingName.ToString(), name);
        sequence.NextAction();
    }
    /// <summary>
    /// 教学结束
    /// </summary>
    public virtual void EndTeaching()
    {
        AnalyticsEvent.SendEvent(AnalyticsType.CompleteTeach, teachingName.ToString());//教学完成打点
        GameDebug.LogFormat("GameTeaching: {0} Complete", teachingName);
        teachState = TeachState.Complete;
        EventCenter.Dispatch(EventKey.GameTeachComplete, teachingName);
        BattleController.Instance.Continue(teachingName.ToString());
        UIController.Instance.canPhysiceback = true;
        var data = DataMgr.Instance.GetSqlService<TeachingSaveData>();
        TeachingSaveData saveData = new TeachingSaveData()
        {
            teachingName = teachingName.ToString(),
            station = 1
        };
        data.Insert(saveData);
        //PlayerPrefs.SetInt(TeachingManager.GetTeachingKey(this), 1);
    }

    public bool IsComplete()
    {
        return teachState == TeachState.Complete;;
    }
}
