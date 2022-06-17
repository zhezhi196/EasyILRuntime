using System;
using System.Collections.Generic;
using Module;
using SecondChapter;
using Sirenix.OdinInspector;
using UnityEngine;
using Object = System.Object;
using Project.Data;

namespace GameProject
{
    public class SceneAreaInteraction : MonoBehaviour
    {
        public int mapId;

        public AreaInteractiveData[] areaInteractiveData;

        [Serializable]
        public struct AreaInteractiveData
        {
            [LabelText("区域Id")]
            public int areaIndex;
            [LabelText("需要交互的物品")]
            public PropsCreator[] needInteractive;
        }
        
        private void Start()
        {
            for (int i = 0; i < areaInteractiveData.Length; i++)
            {
                int areaIndex = areaInteractiveData[i].areaIndex;
                AreaData data = MapCtrl.Instance.GetMapAreaData(mapId,areaIndex);
                PropsCreator[] propsCreators = areaInteractiveData[i].needInteractive;
                if (data != null ) 
                {
                    data.RegisterEvent(propsCreators);
                    data.RefreshCompleteCount(propsCreators.Length);
                }
                else
                {
                    GameDebug.LogErrorFormat("无法找到{0}的地图{1}区域",mapId,areaIndex);
                }
            }
        }

        private void OnDestroy()
        {
            for (int i = 0; i < areaInteractiveData.Length; i++)
            {
                int areaIndex = areaInteractiveData[i].areaIndex;
                AreaData data = MapCtrl.Instance.GetMapAreaData(mapId,areaIndex);
                if (data != null)
                {
                    data.UnRegisterEvent(areaInteractiveData[i].needInteractive);
                }
            }
        }

        private int NeedInteractiveLength(PropsCreator[] propsBase)
        {
            int count = 0;
            for (int i = 0; i < propsBase.Length; i++)
            {
                if (propsBase[i] != null)
                {
                    count ++;
                }
            }
            return count;
        }
        
// #if UNITY_EDITOR
//         [Button("查找物品")]
//         public void SearchProps()
//         {
//             DataMgr.Instance.InitDb();
//             List<PropsBase> result = new List<PropsBase>();
//             
//             var all = transform.GetComponentsInChildren<PropsBase>(true);
//             for (int i = 0; i < all.Length; i++)
//             {
//                 var target = all[i];
//                 PropData da = DataMgr.Instance.GetSqlService<PropData>()
//                     .Where(dd => dd.ID == target.editorData.DataID);
//                 
//                 if (da!=null&&da.notMapObject == 0&& (difficulte.HasFlag(target.editorData.missionDifficult)||target.editorData.missionDifficult.HasFlag(difficulte)))
//                 {
//                     result.Add(all[i]);
//                 }
//             }
//
//             needInteractive = result.ToArray();
//         }
//         [Button("更新数据信息")]
//         public void DeleteDbData()
//         {
//             DataMgr.Instance.InitDb();
//             List<PropsBase> result = new List<PropsBase>();
//
//             for (int i = 0; i < needInteractive.Length; i++)
//             {
//                 var target = needInteractive[i];
//                 PropData da = DataMgr.Instance.GetSqlService<PropData>()
//                     .Where(dd => dd.ID == target.editorData.DataID);
//
//                 if (da != null && da.notMapObject == 0 && (difficulte.HasFlag(target.editorData.missionDifficult) ||
//                                                            target.editorData.missionDifficult.HasFlag(difficulte)))
//                 {
//                     result.Add(needInteractive[i]);
//                 }
//             }
//
//             needInteractive = result.ToArray();
//         }
// #endif

    }
}