using Module;
using UnityEngine;
using UnityEngine.UI;

namespace GameProject
{
    public class MapUI : UIViewBase
    {
        public static bool TryOpenMap()
        {
            // var entity = PropEntity.propsModels;
            int currMapIndex = GetCurrMapIndex();
            // foreach (KeyValuePair<PropsData,PropEntity> keyValuePair in entity)
            // {
            //     MapEntity mapEntity = keyValuePair.Value as MapEntity;
            //     if (mapEntity != null && mapEntity.mapIndex == currMapIndex)
            //     {
            //         if (mapEntity.count > 0)
            //         {
                        UIController.Instance.Open("MapUI", UITweenType.None, currMapIndex);
                        return true;
            //         }
            //         else
            //         {
            //             UIController.Instance.Open("MapUI", UITweenType.None, -1);
            //             return false;
            //         }
            //     }
            // }
            // UIController.Instance.Open("MapUI", UITweenType.None, -1);
            // return false;
        }
        public Color runing;
        public Color complete;
        public GameObject[] area;
        public UIBtnBase back;
        public Text title;
        public Text titleContain;
        public TouchScale touchScale;
        public Text currentTitle;

        public GameObject yizhi;
        public GameObject weizhi;
        protected override void OnChildStart()
        {
            back.AddListener(OnBack);
        }

        private void OnEnable()
        {
            currentTitle.text = MapPlayerTrigger.currTrigger.title;

        }

        private static int GetCurrMapIndex()
        {
            string currNodeIndex = BattleController.Instance.ctrlProcedure.currentNode.mapID;
            return (!string.IsNullOrEmpty(currNodeIndex)) && MapCtrl.Instance.mapData.ContainsKey(currNodeIndex.ToInt()) ? currNodeIndex.ToInt() : -1;
        }
        
        private void OnBack()
        {
            OnExit();
        }

        public override void OnOpenStart()
        {
            BattleController.Instance.Pause(winName);
        }

        public override void OnCloseStart()
        {
            BattleController.Instance.Continue(winName);
        }

        private int index;
        public override void Refresh(params object[] args)
        {
            index= args[0].ToInt();
            if (index == -1)
            {
                weizhi.gameObject.OnActive(true);
                yizhi.gameObject.OnActive(false);
            }
            else
            {
                weizhi.gameObject.OnActive(false);
                yizhi.gameObject.OnActive(true);
                currentTitle.gameObject.OnActive(GetCurrMapIndex() == index);
                for (int i = 1; i <= area.Length; i++)
                {
                    area[i-1].OnActive(i == index);
                }
                string mapName = MapCtrl.Instance.mapData.ContainsKey(index) ? MapCtrl.Instance.mapData[index].mapName : "2443";
                title.text = Language.GetContent(mapName);
                // titleContain.gameObject.OnActive(currentArea.node.Contains(BattleController.Instance.ctrlProcedure.currentNode.mapID));
                touchScale.ResetScale();
            }
        }
    }
}