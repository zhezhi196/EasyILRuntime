using System;
using System.Collections.Generic;
using LitJson;
using Module;
using UnityEngine;

public class PostPlayerData: BattleSystem
{
    public RunTimeAction postPlayerData;
    public const string url = "api/v1/player/info";

    public override void OnNodeEnter(NodeBase node, EnterNodeType enterType)
    {
        base.OnNodeEnter(node, enterType);
        Post();
    }

    public void Post()
    {
        postPlayerData = new RunTimeAction(() =>
        {
            BattleController.Instance.NextFinishAction("postPlayerData");
            return;
            PlayerSaveData saveData = DataMgr.Instance.GetSqlService<PlayerSaveData>().tableList[0];
            PlayerCtrl ctrl = BattleController.GetCtrl<PlayerCtrl>();
            object[] playerData = new object[]
            {
          
            };
            string data = string.Join("|", playerData);

#if LOG_ENABLE
            string[] log = new string[]
            {
        
            };
            GameDebug.Log("封禁玩家数据:" + string.Join("\n", log));
#endif
        
            data = CompressTools.CompressString(data);
            HttpArgs args = new HttpArgs();
            args.AddArgs("id", GameInfo.serverUid);
            args.AddArgs("content", data);
            if (Channel.channel == ChannelType.googlePlay)
            {
                args.AddArgs("channel", 3);
            }
            else if (Channel.channel == ChannelType.AppStore)
            {
                args.AddArgs("channel", 2);
            }
            HttpCache.Instance.Post(url, args, str =>
            {
                JsonData jsonData = JsonMapper.ToObject(str);
                var code = jsonData["code"].ToString();
                if (code == "200")
                {
                    var dd = jsonData["data"];
                    string c = dd["c"].ToString();
                    string w = dd["w"].ToString();
                    PlayerSaveData oldData = DataMgr.Instance.GetSqlService<PlayerSaveData>().tableList[0];
                    oldData.c = c;
                    oldData.w = w;
                    DataMgr.Instance.GetSqlService<PlayerSaveData>().Update(oldData);
                }
            }, true, 0, 30);
        });

    }
}