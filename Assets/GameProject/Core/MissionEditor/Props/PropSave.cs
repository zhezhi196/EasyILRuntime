using LitJson;
using Module;

public class PropSave
{
    public string lastRunlogicalName;    //道具最后执行的Logic
    public int saveStation; //道具最后的状态
    public string[] runlogicalArgs; //道具最后执行的logic的参数
    public string[] reciverCount;
    public string propsData; //道具自定义的一些存储参数
    public int interactiveCount;
    public int initStation; //道具初始化的状态
    public bool mapIsGet;  //道具是否已经获取了，地图用
    

    public string GetWriteStr(PropsCreator creator)
    {
        initStation = (int) creator.GetTrueSaveStation();
        // if (props != null)
        // {
        //     if(props.ContainStation(PropsStation.Destroyed))
        //     {
        //         initStation = (int) SaveStation.UnActive;
        //     }
        // }
        // else
        // {
        //     initStation = (int) SaveStation.Unload;
        // }


        return JsonMapper.ToJson(this);
    }
}