using System;
using Module;
using UnityEngine;
using UnityEngine.Serialization;

namespace GameProject
{
    public class MapPlayerTrigger : MonoBehaviour , IMapInfo
    {
        public static MapPlayerTrigger currTrigger; 
        
        [FormerlySerializedAs("mapId")] public int _mapId;
        public int areaIndex;
        private string _title;

        public string title
        {
            get
            {
                string _title = MapCtrl.Instance.GetMapAreaData(mapId,areaIndex).areaName;
                return Language.GetContent(_title);
            }
        }

        public void Awake()
        {
            this.gameObject.layer = 26 ; 
            AreaData data = MapCtrl.Instance.GetMapAreaData(mapId,areaIndex);
            if (data != null )
            {
                IMapInfo[] mapInfos = new IMapInfo[]{this} ;
                data.RegisterEvent(mapInfos);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                currTrigger = this;
                MapCtrl.Instance.RefreshCurrentAreaIndex(areaIndex);
                int missionid = BattleController.Instance.missionId;
                // Analytics.SendEvent(AnalyticsType.ArriveTarget, this,missionid);
                EventCenter.Dispatch<IMapInfo>(EventKey.MapInteractionEvent,this);
            }    
        }

        public bool isGet { get; }
        public MapType mapType => MapType.Trigger;
        public int mapId => _mapId;
    }
}