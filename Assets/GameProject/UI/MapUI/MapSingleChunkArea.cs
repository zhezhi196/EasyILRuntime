using Module;
using UnityEngine;
using UnityEngine.UI;

namespace GameProject
{

    public class MapSingleChunkArea: MonoBehaviour
    {
        public int mapId;
        public int areaIndex;
        public AreaData area
        {
            get
            {
               return MapCtrl.Instance.GetMapAreaData(mapId,areaIndex);
            }
        }

        public RectTransform pointUI;

        public Image[] mapCompleteStatus;

        public MapUI mapUi;
        public Text title;
        public int offset;
        
        private void Start()
        {
            if (mapUi == null)
                mapUi = transform.GetComponentInParent<MapUI>();

            if (area == null) return;
            if (area.isComplete)
            {
                OnComplete();
            }
            else
            {
                area.onComplete += OnComplete;
            }
        }
        
        private void Update()
        {
            RefreshPlayerPoint();
        }

        private void OnEnable()
        {
            title.gameObject.OnActive(true);
            pointUI.gameObject.SetActive(areaIndex == MapCtrl.Instance.currentAreaIndex);
            title.text = Language.GetContent(area.areaName);
            
            RefreshPlayerPoint();
        }

        private void OnDestroy()
        {
            if (area != null)
            {
                area.onComplete -= OnComplete;
            }
        }

        private void OnComplete()
        {
            for (int i = 0; i < mapCompleteStatus.Length; i++)
            {
                if (area.isComplete)
                {
                    mapCompleteStatus[i].color = mapUi.complete;
                }
            }
        }

        private void RefreshPlayerPoint()
        {
            if (this.pointUI.gameObject.activeSelf)
            {
                Vector3 angele =  (UnityEngine.Quaternion.Euler(0, offset, 0)* Player.player.transform.rotation).eulerAngles;
                pointUI.eulerAngles = new Vector3(0, 0, -angele.y);
                
                
                // pointUI.anchoredPosition = ""
            }
        }
        
   
    }
}