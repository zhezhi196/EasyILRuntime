using System;
using System.Collections.Generic;
using Module;
using UnityEngine;

public class PropsCtrl: BattleSystem
{
    public RunTimeAction loadPros = null;
    public List<PropsBase> exitProps = new List<PropsBase>();
    public List<PropEntity> propPopupedMarkList = new List<PropEntity>();


    
    public override void OnNodeEnter(NodeBase node, EnterNodeType enterType)
    {
        loadPros = new RunTimeAction(() =>
        {
            if (node is TaskNode task)
            {
                Voter v = new Voter(PropsCreator.editorList.Count, () =>
                {
                    BattleController.Instance.NextFinishAction("loadPros");
                    loadPros = null;
                });
                exitProps.Clear();
                // Dictionary<PropsCreator,bool> ssss = new Dictionary<PropsCreator, bool>();
                for (int i = 0; i < PropsCreator.editorList.Count; i++)
                {

                    var creator = PropsCreator.editorList[i];
                    // ssss.Add(creator , false);
                    // if (creator.id == 316)
                    // {
                    //     
                    // }
                    creator.OnNodeEnter(task, props =>
                    {
                        if (props != null && props.isActive)
                        {
                            exitProps.Add(props);
                        }

                        // ssss[creator] = true;
                        v.Add();

                        // if (v.count == 151)
                        // {
                        //     foreach (var b in ssss)
                        //     {
                        //         if (b.Value == false)
                        //         {
                        //             GameDebug.Log(b.Key.id);
                        //         }
                        //     }
                        // }
                        // GameDebug.Log($"{v.count} {v.maxCount}");
                    });
                }
            }
        });
    }

    public override void Save()
    {
        for (int i = 0; i < PropsCreator.editorList.Count; i++)
        {
            PropsCreator.editorList[i].Save();
        }
    }

    public override void OnUpdate()
    {
        PropsBase.UpdateTime();
    }



    #region 道具的交互点逻辑
    
    public static Action<bool, LookPoint,Action> onShowLookPoint;
    
    public static void RegisterLookPointEvent(Action<bool, LookPoint,Action> e)
    {
        onShowLookPoint += e;
    }
    
    public static void UnRegisterLookPointEvent(Action<bool, LookPoint,Action> e)
    {
        if (onShowLookPoint != null) onShowLookPoint -= e;
    }

    public static void OnShowLookPoint(bool show , LookPoint point , Action callBack)
    {
        onShowLookPoint?.Invoke(show , point , callBack);
    }

    #endregion

}