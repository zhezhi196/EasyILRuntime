using System;
using Sirenix.OdinInspector;

[Flags]
public enum MapType
{
    Main = 1,
    Door = 2,
    Prop = 3,
    Trigger = 4,
}
[Serializable]
public struct CreatorExtuil
{
    [HideLabel] 
    public ProgressOption progress;
    [LabelText("地图类型")]
    public MapType mapType;
    [LabelText("地图ID")] 
    public int mapId;
    [LabelText("区域id")]
    public int areaId;

}