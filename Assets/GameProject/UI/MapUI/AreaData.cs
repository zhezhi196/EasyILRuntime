using System;
using Module;
using UnityEngine;


namespace GameProject
{
    public class AreaData: ILocalSave
    {
        public int mapId;
        public int areaIndex;
        public string areaName;
        public int completeCount;
        private int currentCount;
        private int isTriggered; //房间是否进入过了
        private IMapInfo[] propCreators;
        
        public string localFileName
        {
            get { return LocalSave.savePath; }
        }
        public string localGroup
        {
            get { return "MapArea"; }
        }  
        public string localUid
        {
            get { return $"{mapId}Area{areaIndex}"; }
        }  
  
        public bool isComplete
        {
            get
            {
                return currentCount >= completeCount && isTriggered == 1;
            }
        }
        public event Action onComplete;

        public AreaData( MapCtrl.InitializeAreaData data)
        {
            this.mapId = data.mapId;
            this.areaIndex = data.areaIndex;
            this.areaName = data.areaName;
            ReadData();
        }

        public void RefreshCompleteCount(int completeCount)
        {
            this.completeCount = completeCount;
        }
        
        public void ReadData()
        {
            string data = LocalSave.Read(this);
            if (data != null)
            {
                var split = data.Split(ConstKey.Spite0);
                currentCount = split[0].ToInt();
                isTriggered = split[1].ToInt();
            }
        }

        public string GetWriteDate()
        {
            return currentCount.ToString()+ConstKey.Spite0+isTriggered.ToString();
        } 

        public void OnInteractive(IMapInfo propsCreator)
        {
            if (propsCreator.mapType == MapType.Trigger) //进来的是trigger
            {
                if (isTriggered == 0 && isContainProp(propsCreator))
                {
                    isTriggered = 1;
                    if(isComplete) onComplete?.Invoke();  
                }
            }
            else
            {
                if (isContainProp(propsCreator))
                {
                    currentCount++;
                    if(isComplete) onComplete?.Invoke();
                }
            }
        }

        private bool isContainProp(IMapInfo propsCreator)
        {
            for (int i = 0; i < propCreators.Length; i++)
            {
                if (propCreators[i] != null && propCreators[i] == propsCreator)
                {
                    return true;
                }
            }
            return false;
        }
        
        public void RegisterEvent(IMapInfo[] propsCreators)
        {
            if (this.propCreators == null)
            {
                this.propCreators = propsCreators;
            }
            else
            {
                this.propCreators = ExtendUtil.Add(this.propCreators, propsCreators);
            }
            EventCenter.Register<IMapInfo>(EventKey.MapInteractionEvent, OnInteractive);
        }

        public void UnRegisterEvent(IMapInfo[] propsCreators)
        {
            this.propCreators = null;
            EventCenter.UnRegister<IMapInfo>(EventKey.MapInteractionEvent,OnInteractive);
        }
  
    }
}