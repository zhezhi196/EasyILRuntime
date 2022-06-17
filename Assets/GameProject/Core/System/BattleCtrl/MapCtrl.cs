using System.Collections.Generic;
using GameProject;
using UnityEngine;
using Module;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using Project.Data;

/// <summary>
/// 游戏ui地图管理
/// 地图初始化,存档
/// 地图更新
/// 获取地图探索遮罩
/// </summary>
public class MapCtrl : BattleSystem,ILocalSave
{
    public string localFileName => LocalSave.savePath;
    public string localGroup { get; }
    public string localUid { get; }
    public string GetWriteDate()
    {
        // throw new NotImplementedException();
        return string.Empty;
    }

    private int CurrentAreaIndex;

    public int currentAreaIndex
    {
        get { return CurrentAreaIndex; }
    }
    
    
    private static MapCtrl _instance;
    public static MapCtrl Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new MapCtrl();
            }
            return _instance;
        }
    }
    private static Dictionary<int ,InitializeMapData> MapInitData = new Dictionary<int ,InitializeMapData>();
    public Dictionary<int ,InitializeMapData> mapData
    {
        get {
            return MapInitData;
        }
    }

    public void RefreshCurrentAreaIndex(int areaIndex)
    {
        this.CurrentAreaIndex = areaIndex;
        Debug.Log("记录当前触发区域 in MapCtrl"+areaIndex);
    }
     
    public override void StartBattle(EnterNodeType enterType)
    {
        InitMapData();
    }

    private void InitMapData()
    {
        GameObject battleController = GameObject.Find("BattleController");
        PropsCreator[] mapCreator = battleController.GetComponentsInChildren<PropsCreator>();
        foreach ( PropsCreator propCreator in mapCreator)
        {
            // 地图  场景中配置的id从1开始
            int mapId = propCreator.mapId;
            //  当前地图未初始化
            if (mapId > 0 && !MapInitData.ContainsKey(mapId))
            {
                // 初始化单个地图
                InitializeMapData initializeMapData = new InitializeMapData();
                initializeMapData.areaData = new Dictionary<int ,AreaData>();

                // 获得地图sql数据表
                var sqlData = DataMgr.Instance.GetSqlService<MapData>();
                var sqlDataTableList = sqlData.tableList;
                for (int i = 0; i < sqlDataTableList.Count;i++)
                {
                    if (mapId == sqlDataTableList[i].mapId)
                    {
                        // 获得地图中 全部区域名称
                        string areaName = sqlDataTableList[i].areaName;
                        int areaCount = sqlDataTableList[i].areaCount;
                        string[] singeAreaName = areaName.Split('_');
                        //初始化 单元区域
                        for (int j = 0; j < areaCount; j++)
                        {
                            InitializeAreaData initializeAreaData = new InitializeAreaData();
                            int _AreaIndex = j + 1;
                            initializeAreaData.mapId = mapId;
                            initializeAreaData.areaIndex =_AreaIndex;
                            initializeAreaData.areaName = j < singeAreaName.Length ? singeAreaName[j] : "区域名称未配置" ;
                            AreaData areaData = new AreaData(initializeAreaData);

                            // 单个区域的信息 总合进地图的区域单元中
                            initializeMapData.areaData.Add(_AreaIndex,areaData);
                        }
                        initializeMapData.mapId = mapId;
                        initializeMapData.mapName = i < sqlDataTableList.Count ? sqlDataTableList[i].mapName : "地图名称未配置" ;
                        MapInitData.Add(mapId,initializeMapData);
                        break;
                    }
                }
            }
        }
    }

    // 整张图内容
    public struct InitializeMapData
    {
        // 地图id
        public int mapId;
        
        // 地图名字
        public string mapName;
        
        //地图单格数据
        public Dictionary<int ,AreaData> areaData ;
    }
    
    // 单个区域模块
    public struct InitializeAreaData 
    {
        //地图id 
        public int mapId;
        // 区域id
        public int areaIndex;
        // 区域名称
        public string areaName;
    }

    public AreaData GetMapAreaData(int mapId, int areaIndex)
    {
        if (mapData.ContainsKey(mapId)== false)
        {
            Debug.LogError("当前地图id未配置 == : "+mapId + "      场景PropsCreato脚本上");
            return null;
        }
        if (mapData[mapId].areaData.ContainsKey(areaIndex) == false)
        {
            Debug.LogError($"当前{mapId}地图区域{areaIndex}未配置");
            return null;
        }
        return mapData[mapId].areaData[areaIndex];
    }
    
    public override void Save()
    {
        foreach (var data in mapData)
        {
            var value = data.Value;
            int count = value.areaData.Count;
            
            for (int j = 1; j <=count; j++)
            {
                LocalSave.Write(value.areaData[j]);
            }
        }
    }
    
}