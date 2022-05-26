using System;
using Sirenix.OdinInspector;

[Flags]
public enum MapType
{
    Main = 1,
    Door = 2,
}
[Serializable]
public struct CreatorExtuil
{
    [HideLabel] 
    public ProgressOption progress;
    [LabelText("地图类型")]
    public MapType mapType;
    [LabelText("地图ID")] 
    public string mapId;
    [LabelText("区域id")]
    public string areaId;

}